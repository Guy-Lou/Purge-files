//CREE DES FICHIER GED ET UNE BDD
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace GestionPurge.Main;

internal class JeuTestGED(IBDDServiceProvider serviceProvider, IOptions<GestionPurgeOptions> options, ILogger<JeuTestGED> logger)
{
    private readonly List<int> _oidnouveauxFichier = new();
    private readonly ServiceGed _serviceGed = serviceProvider.GetServiceGed();
    private readonly GestionPurgeOptions _options = options.Value;
    private readonly string _cheminEnregistrement = options.Value.CheminGed;
    private readonly ILogger<JeuTestGED> _logger = logger;

    public async Task PréparerJeuTestGEDAsync()
    {
        _logger.LogInformation("Préparation du jeu de test");
        _oidnouveauxFichier.Clear();
        PréparerFichiers();
        //_oidnouveauxFichier.Add(5);
        await PréparerBDDAsync();
    }


    private void PréparerFichiers()
    {
        if (Directory.Exists(_cheminEnregistrement))
            Directory.Delete(_cheminEnregistrement, recursive: true);
        var nbFichierCréer = 0;

        foreach (var mois in Enumerable.Range(1, 4))
        {
            foreach (var x in Enumerable.Range(1, 5))
            {
                string cheminFinal = Path.Combine(_cheminEnregistrement, mois.ToString("00"), x.ToString("000"));
                Directory.CreateDirectory(cheminFinal);
                
                foreach (var NbFichier in Enumerable.Range(1, 200))
                {
                    int Nom_Fichier;
                    do
                    {
                        Nom_Fichier = Random.Shared.Next(0, 10_000_001);
                    }
                    while (_oidnouveauxFichier.Contains(Nom_Fichier));
                    string chemin_fichier = Path.Combine(cheminFinal, Nom_Fichier.ToString("00000000"));
                    try
                    {
                        using var f = File.Create(chemin_fichier);
                        f.WriteByte((byte)Random.Shared.Next());
                    }
                    catch(Exception e)
                    {
                        _logger.LogError($"{chemin_fichier} n'a pas pu être créé : {e.Message}");                    
                    }
                    _oidnouveauxFichier.Add(Nom_Fichier);
                    nbFichierCréer += 1;
                }
            }
        }
    }

    private async Task PréparerBDDAsync()
    {
        await PurgerBDDAsync();

        // On supprime 10% des fichiers générés
        for (int i = 0; i < _oidnouveauxFichier.Count; i++)
        {
            var z = Random.Shared.Next(0, 10);
            if (z == 0)
            {
                _oidnouveauxFichier.RemoveAt(i);
            }
        }

        await AjouterFichiersAsync(_oidnouveauxFichier);
    }


    private async Task PurgerBDDAsync()
    {
        using var ctx = GetContext();
        await ctx.Database.ExecuteSqlRawAsync("IF (SELECT COUNT(*) FROM admsi.GED_FICHIER) < 50000 BEGIN TRUNCATE TABLE admsi.GED_FICHIER END");
    }

    private async Task AjouterFichiersAsync(List<int> oids)
    {
        using var ctx = GetContext();
        await ctx.GED_FICHIER.AddRangeAsync(oids.Select(o => new GED_FICHIER() { oid = o, NomFic = o.ToString() , DateImport = DateTime.Now}));
        await ctx.SaveChangesAsync();
    }

    private powerhermesEntities GetContext()
        => ((IContextProvider<powerhermesEntities>)_serviceGed
            .GetType()
            .GetField("_contextProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(_serviceGed)!)!
            .GetContext();
}
