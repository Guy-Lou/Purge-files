//DEFINIE LES VARIABLE UTILISABLE DANS LES DIFFERENT PROGRAMMES
using System.ComponentModel.DataAnnotations;

namespace GestionPurge.Main;
internal class GestionPurgeOptions
{
    [Required(ErrorMessage = "Le chemin d'accès à la GED n'a pas été défini")]
    public string CheminGed { set; get; } = null!;

    [Required(ErrorMessage = "La taille du cluster pour la GED n'a pas été défiie")]
    public int? TailleClusterGED { set; get; }

    [Required(ErrorMessage = "le chemin d'accès aux EDIs n'a pas été définie")]
    public string CheminEDI { set; get; } = null!;

    [Required(ErrorMessage = "La date de péremption des EDI n'a pas été définie")]
    public TimeSpan? PéremptionEDIJours { set; get; }

    [Required(ErrorMessage = "Le delai entre les purges n'a pa été correctement défini")]
    public TimeSpan? delaiPurge {  set; get; }
    
}