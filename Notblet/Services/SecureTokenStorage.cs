using System.IO;
using Microsoft.AspNetCore.DataProtection;
using NLog;

public class SecureTokenStorage
{
    // Instance unique de SecureTokenStorage
    private static readonly Lazy<SecureTokenStorage> _instance = new Lazy<SecureTokenStorage>(() => new SecureTokenStorage());

    // Propriété pour accéder à l'instance unique
    public static SecureTokenStorage Instance => _instance.Value;

    private string? _token;
    public string token => _token ?? _GetToken();

    private readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Notblet", "secure_token.txt");
    private readonly IDataProtector protector;

    // Logger pour la classe
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // Constructeur privé pour éviter la création d'instances en dehors de la classe
    private SecureTokenStorage()
    {
        var provider = DataProtectionProvider.Create("Notblet");
        protector = provider.CreateProtector("TokenProtection");
        Logger.Info("Initialisation de SecureTokenStorage avec DataProtectionProvider.");
    }

    public void SaveToken(string token)
    {
        try
        {
            Logger.Info("Début de la sauvegarde du token.");
            // Créer le dossier s'il n'existe pas déjà
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Impossible de déterminer le chemin du fichier."));
            Logger.Info("Répertoire pour le fichier de token vérifié/créé : {0}", Path.GetDirectoryName(filePath));

            // Chiffrer le token avant de le stocker
            var protectedToken = protector.Protect(token);
            Logger.Info("Token chiffré avec succès.");

            // Sauvegarder le token chiffré
            File.WriteAllText(filePath, protectedToken);
            Logger.Info("Token sauvegardé dans le fichier : {0}", filePath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Erreur lors de la sauvegarde du token.");
            throw;
        }
    }

    public string _GetToken()
    {
        try
        {
            Logger.Info("Début de la récupération du token.");
            if (File.Exists(filePath))
            {
                Logger.Info("Fichier de token trouvé : {0}", filePath);
                var protectedToken = File.ReadAllText(filePath);
                Logger.Info("Token chiffré lu depuis le fichier.");

                var readableToken = protector.Unprotect(protectedToken); // Déchiffrer le token
                Logger.Info("Token déchiffré avec succès.");
                _token = readableToken;
                return readableToken;
            }
            else
            {
                Logger.Warn("Fichier de token introuvable.");
                throw new FileNotFoundException("Token file not found.");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Erreur lors de la récupération du token.");
            throw;
        }
    }

    public void DeleteToken()
    {
        try
        {
            Logger.Info("Début de la suppression du token.");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Logger.Info("Fichier de token supprimé : {0}", filePath);
                _token = null;
            }
            else
            {
                Logger.Warn("Aucun fichier de token à supprimer.");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Erreur lors de la suppression du token.");
            throw;
        }
    }
}
