using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GestionPurge.Main.Plannification;

internal class Horaires(IOptions<GestionPlannificationHorairesOptions> options, ILogger<Horaires> logger)
    : IPlannificateur
{
    private readonly GestionPlannificationHorairesOptions _options = options.Value;
    private readonly int _JourSemaine = options.Value.JourSemaine;
    private readonly TimeOnly _HeureDébut = options.Value.HeureDébut;
    private readonly TimeSpan _DuréeMax = options.Value.DuréeMax;
    private readonly ILogger<Horaires> _logger = logger;
    private readonly HashSet<IPlannifiable> _plannifiables = new();

    public void AjouterPlannifiable(IPlannifiable plannifiable)
    {
        _plannifiables.Add(plannifiable);
    }

    public async Task DémarrerPlannificationAsync(CancellationToken cancellationToken)
    {
        /* Plage de 16h à 19h le jeudi
         * Cas 1 : avant 16h le jeudi : on attends jusqu'à date / heure de début
         * Cas 2 : entre 16h et 19h le jeudi : on lance immédiatement
         * Cas 3 : après 19h le jeudi : on attends jusqu'à +7j après date heure début
         */
        var dateHeureDébut = DateTime.Today.AddDays(_JourSemaine - (int)DateTime.Today.DayOfWeek).Add(_HeureDébut.ToTimeSpan());
        var dateHeureFin = dateHeureDébut.Add(_DuréeMax);
        TimeSpan délaiAttente;
        TimeSpan délaiPurge = _DuréeMax;
        if (dateHeureDébut < DateTime.Now)
        {
            if (dateHeureFin < DateTime.Now) // cas 3
            {
                dateHeureDébut = dateHeureDébut.AddDays(7);
                dateHeureFin = dateHeureFin.AddDays(7);
                délaiAttente = dateHeureDébut - DateTime.Now;
            }
            else
            {
                // cas 2
                délaiAttente = default;
                délaiPurge = dateHeureFin - DateTime.Now;
            }
        }
        else
        {
            // cas 1
            délaiAttente = dateHeureDébut - DateTime.Now;
            délaiPurge = _DuréeMax;
        }
        
        if(délaiAttente != default)
        {
            await Task.Delay(délaiAttente);
        }

        var cts = new CancellationTokenSource();

        List<Task> plannifiésLancés = new();

        foreach(var p in _plannifiables)
        {
            try
            {
                var t = p.ExécuterAsync(cts.Token);
                plannifiésLancés.Add(t);
            }
            catch (Exception e)
            {

                _logger.LogError(e,)
            }
            
        }

        // attendre heure fin
        await Task.Delay(délaiPurge);
        cts.Cancel();

        await Task.WhenAll(plannifiésLancés);
    } 
}
