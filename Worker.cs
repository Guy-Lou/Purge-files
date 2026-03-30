using GestionPurge.Main.Plannification;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GestionPurge.Main;

internal class Worker(ILogger<Worker> logger, PurgeGed purgeGed, PurgeEDI purgeEDI, IOptions<GestionPurgeOptions> optionsPurge, IOptions<GestionPlannificationOptions> optionsPlannif, Horaires horaires, Cycles cycles)
    : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly PurgeGed _purgeGed = purgeGed;
    private readonly PurgeEDI _purgeEDI = purgeEDI;
    private readonly GestionPurgeOptions _optionsPurge = optionsPurge.Value;
    private readonly GestionPlannificationOptions _optionsPlannif = optionsPlannif.Value;
    private readonly Horaires _horaires = horaires;
    private readonly Cycles _cycles = cycles;

    protected override async Task ExecuteAsync(CancellationToken arrêt)
    {
        _logger.LogInformation("Démarrage du gestionnaire de purge");
        IPlannificateur plannif = _optionsPlannif.ModeGestion switch
        {
            ModeGestion.Horaires => _horaires,
            ModeGestion.Cycles => _cycles,
            _ => throw new ArgumentException("Le mode dexecution de la purge n'a pas été défini")
        };

        plannif.AjouterPlannifiable(_purgeEDI);
        plannif.AjouterPlannifiable(_purgeGed);

        try
        {
            await plannif.DémarrerPlannificationAsync(arrêt);
        }
        catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
        {
            // arrêt normal
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur inattendue dans le gestionnaire de purge, celui-ci va s'arrêter !");
        }
        

        _logger.LogInformation("Arrêt du gestionnaire de purge");
    }

}
