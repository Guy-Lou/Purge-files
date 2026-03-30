//CREE DES FICHIER EDI TEST ET CHANGE LEUR DATE DE CREATION
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
namespace GestionPurge.Main;

internal class JeuTestEDI(IOptions<GestionPurgeOptions> options, ILogger<JeuTestEDI> logger)
{
    private readonly GestionPurgeOptions _options = options.Value;
    private readonly string _cheminEnregistrement = options.Value.CheminEDI;
    private readonly ILogger<JeuTestEDI> _logger = logger;

    public async Task PréparerJeuTestEDIAsync()
    {
        _logger.LogInformation("Préparation du jeu de test");
        PréparerFichiers();
    }

    private void PréparerFichiers()
    {
        if (Directory.Exists(_cheminEnregistrement))
            Directory.Delete(_cheminEnregistrement, recursive: true);
 
        foreach (var Entreprise in Enumerable.Range(1, 10))
        {
            var repertoirFichier = Path.Combine(_cheminEnregistrement, Entreprise.ToString(), "ARCHIVES");
            Directory.CreateDirectory(repertoirFichier);
            foreach (var nb_EDI in Enumerable.Range(1, 200))
            {
                int Nom_EDI;
                Nom_EDI = Random.Shared.Next(0, 10_000_001);
                string cheminEDI = Path.Combine(repertoirFichier, Nom_EDI.ToString());
                try
                {
                    using (var f = File.Create(cheminEDI))
                    {
                        f.WriteByte((byte)Random.Shared.Next());
                    }
                }
                catch (Exception e) 
                {
                    _logger.LogError($"{cheminEDI} 'a pas pu être créer :{e.Message}");
                }
                try
                {
                    var annéecréa = Random.Shared.Next(2011, 2025);
                    var moiscréa = Random.Shared.Next(1, 13);
                    var jourMax = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(annéecréa, moiscréa) + 1;
                    var jourcréa = Random.Shared.Next(1, jourMax);
                    DateTime datecréa = new DateTime(annéecréa, moiscréa, jourcréa);
                    File.SetCreationTime(cheminEDI, datecréa);
                }
                catch (Exception e)
                {
                    _logger.LogError($"La date de création de {cheminEDI} n'a pas pu être modifié : {e.Message}");
                }
            }
        }
    }
}
