//DEFINIE LES VARIABLE UTILISABLE DANS LES DIFFERENT PROGRAMMES
using System.ComponentModel.DataAnnotations;

namespace GestionPurge.Main.Plannification;

internal class GestionPlannificationOptions
{
    [Required(ErrorMessage = "Le mode de gestion (horaires ou cycles) doit être défini")]
    public ModeGestion? ModeGestion { get; set; }
}

internal class GestionPlannificationHorairesOptions
{
    [Required(ErrorMessage = "L'heure de début de la purge n' pas été correctement définie")]
    public TimeOnly HeureDébut { set; get; }

    [Required(ErrorMessage = "Le delai maximum pour finre de la purge n' pas été correctement définie")]
    public TimeSpan DuréeMax { set; get; }

    [Required(ErrorMessage = "Le delai entre deux purges n'a pas été correctement défini")]
    public TimeSpan DelaiPurge { set; get; }

    [Required(ErrorMessage = "Le jour de purge n'a pas été correctement défini")]
    public int JourSemaine { set; get; }
}

internal class GestionPlannificationCyclesOptions
{
    [Required(ErrorMessage = "Le delai entre deux purges n'a pas été correctement défini")]
    public TimeSpan DelaiPurge { set; get; }

    [Required(ErrorMessage = "Le comportement si le delai est dépassé n'a pas été correctement definie")]
    public ComportementDélaiDépassé ComportementDélaiDépassé { get; set; }
}