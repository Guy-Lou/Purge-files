using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GestionPurge.Main;

internal class PurgeGed(IOptions<GestionPurgeOptions> options, IBDDServiceProvider serviceProvider, JeuTestGED jeuTest, ILogger<PurgeGed> logger)
    : IPlannifiable
{
    private readonly ServiceGed _serviceGed = serviceProvider.GetServiceGed();
    private readonly GestionPurgeOptions _options = options.Value;
    private readonly JeuTestGED _jeuTest = jeuTest;
    private readonly ILogger<PurgeGed> _logger = logger;
    private readonly float _tailleClusterGED = options.Value.TailleClusterGED ?? throw new ArgumentException("La taille de cluster des fichiers GED n'a pas été défini !!");

    public async Task ExécuterAsync(CancellationToken annulation)
    {
        // Balayer les données pour voir ce qui est à purger

        long totalTailleDisque = 0;
        var nbFichierSupp = 0;

        foreach (var mois in Directory.EnumerateDirectories(_options.CheminGed))
            foreach (var x in Directory.EnumerateDirectories(mois))
                foreach (var chemin_Fichier in Directory.EnumerateFiles(x))
                {
                    try
                    {
                        if (!await VérifierExistanceBDDAsync(chemin_Fichier))
                        {
                            // ce qui est à purger = ce que la méthode VérifierExistanceBDD renvoie faux

                            var fi = new FileInfo(chemin_Fichier);
                            var dateCréa = fi.CreationTime;
                            var tailleRéelle = fi.Length;

                            // à chaque suppression, écrire le chemin du fichier et sa date de création sur la console (Console.WriteLine(""))

                            _logger.LogInformation($"{chemin_Fichier} Créé le {dateCréa}");
                            var tailleDisque = ((int)Math.Ceiling(tailleRéelle / _tailleClusterGED)) * 4096;
                            totalTailleDisque += tailleDisque;
                            File.Delete(chemin_Fichier);
                            nbFichierSupp += 1;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"{chemin_Fichier} n'a pas pu être traité : {e.Message}");
                    }
                }
        // à la fin de la procédure, afficher un résumé du nombre de fichier supprimés et leur taille réelle et leur taille sur le disque

        _logger.LogInformation($"{nbFichierSupp} fichiers ont été supprimé. Vous avez libéré {totalTailleDisque / 1024.0:0.00} ko");
    }


    private async Task<bool> VérifierExistanceBDDAsync(string cheminFichier)
    {
        var nomFichier = Path.GetFileName(cheminFichier);
        if (int.TryParse(nomFichier, out int oidFichier))
        {
            var fic = (await _serviceGed.ListerDocumentsAsync(f => f.oid == oidFichier, f => true)).FirstOrDefault();
            return fic;
        }
        throw new Exception("Le nom dee fichier ne correspond pas au format attendu (entier)");
    }
}
