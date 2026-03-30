namespace GestionPurge.Main;

internal interface IPlannificateur
{
    void AjouterPlannifiable(IPlannifiable plannifiable);
    Task DémarrerPlannificationAsync(CancellationToken annulation);
}