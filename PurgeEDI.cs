using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GestionPurge.Main
{
    internal class PurgeEDI(IOptions<GestionPurgeOptions> options, JeuTestEDI jeuTestEDI, ILogger<PurgeEDI> logger)
        : IPlannifiable
    {
        private readonly GestionPurgeOptions _options = options.Value;
        private readonly ILogger<PurgeEDI> _logger = logger;
        private readonly JeuTestEDI _jeuTesEDI = jeuTestEDI;
        private readonly TimeSpan _PéremptionEDI = options.Value.PéremptionEDIJours ?? throw new ArgumentException("Le temp de pérenption n'a pas été défini !!");
        private readonly float _tailleClusterGED = options.Value.TailleClusterGED ?? throw new ArgumentException("La taille de cluster des fichiers EDI n'a pas été défini !!");

        public Task ExécuterAsync(CancellationToken annulation)
        {
            long totalTailleDisque = 0;
            var nbEDISupp = 0;
            foreach (var Entreprise in Directory.EnumerateDirectories(_options.CheminEDI))
            {
                foreach (var CheminEDI in Directory.EnumerateFiles(Path.Combine(Entreprise, "ARCHIVES")))
                {
                    DateTime dateCréa = File.GetCreationTime(CheminEDI);
                    DateTime dateJ = DateTime.Today;
                    TimeSpan VieEDI = dateJ - dateCréa;
                    var fi = new FileInfo(CheminEDI);
                    var tailleRéelle = fi.Length;
                    try
                    {
                        if (VieEDI >= _PéremptionEDI)
                        {
                            _logger.LogInformation($"{CheminEDI} Créé le {dateCréa}");
                            File.Delete(CheminEDI);
                            var tailleDisque = ((int)Math.Ceiling(tailleRéelle / _tailleClusterGED)) * 4096;
                            totalTailleDisque += tailleDisque;
                            nbEDISupp += 1;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"{CheminEDI} n'a pas pu être traité : {e.Message}");
                    }
                }
            } 
            _logger.LogInformation($"Vous avez supprimé {nbEDISupp} et libéré {totalTailleDisque/1024.0:0.00}Ko");

            return Task.CompletedTask;
        }
    }
}
