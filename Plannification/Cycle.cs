using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GestionPurge.Main.Plannification;

internal class Cycles(IOptions<GestionPlannificationCyclesOptions> options, ILogger<Cycles> logger)
    : IPlannificateur
{
    private readonly GestionPlannificationCyclesOptions _options = options.Value;
    private readonly ILogger<Cycles> _logger = logger;
    private readonly HashSet<IPlannifiable> _plannifiables = new();

    delegate void TraiterDélaiDépassé(ref TimeSpan t);


    public void AjouterPlannifiable(IPlannifiable plannifiable)
    {
        _plannifiables.Add(plannifiable);
    }
        

    public async Task DémarrerPlannificationAsync(CancellationToken cancellationToken)
    {
        
        var dateHeurDerExec = DateTime.Now.AddSeconds(60-DateTime.Now.Second);
        await Task.Delay(_options.DelaiPurge - (DateTime.Now - dateHeurDerExec));

        TraiterDélaiDépassé traiterDélaiDépassé = _options.ComportementDélaiDépassé switch
        {
            ComportementDélaiDépassé.Erreur => Solution1,
            ComportementDélaiDépassé.ContinuerImmédiatement => Solution2,
            ComportementDélaiDépassé.ContinuerAprèsProchainCycle => Solution3,
            _ => throw new ArgumentException("Le comportement en cas de dépassement du délai de cycle n'a pas été défini")
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            dateHeurDerExec = DateTime.Now;
            foreach (var p in _plannifiables)
            {
                await p.ExécuterAsync(cancellationToken);
            }
            
            
            var délaiAttenteFinal = _options.DelaiPurge - (DateTime.Now - dateHeurDerExec);

            /*
            délai : 20s
            tache : 30s
            on doit attendre 10s, et on attend en fait 20s

            délai : 20s
            tache : 50s
            on doit attendre 10s, et on attend en fait 40s

            */

            if (délaiAttenteFinal < TimeSpan.Zero)
            {
                traiterDélaiDépassé(ref délaiAttenteFinal);
            }

            await Task.Delay(délaiAttenteFinal);
        }
    }

    private void Solution1(ref TimeSpan délaiAttente)
    {
        _logger.LogError($"Le délai est trop coup par rapport au temps d'éxécution du programme.");
        délaiAttente = TimeSpan.Zero;
    }
    private void Solution2(ref TimeSpan délaiAttente)
    {
        délaiAttente = TimeSpan.Zero;
    }

    private void Solution3(ref TimeSpan délaiAttente)
    {
        délaiAttente = _options.DelaiPurge - TimeSpan.FromSeconds(Math.Abs(délaiAttente.TotalSeconds) % _options.DelaiPurge.TotalSeconds);
    }
}

