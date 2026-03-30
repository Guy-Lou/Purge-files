namespace GestionPurge.Main;

internal interface IPlannifiable
{
    Task ExécuterAsync(CancellationToken annulation);
}
