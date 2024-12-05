using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

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

    // Constructeur privé pour éviter la création d'instances en dehors de la classe
    private SecureTokenStorage()
    {
        var provider = DataProtectionProvider.Create("Notblet");
        protector = provider.CreateProtector("TokenProtection");
    }

    public void SaveToken(string token)
    {
        // Créer le dossier s'il n'existe pas déjà
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());

        // Chiffrer le token avant de le stocker
        var protectedToken = protector.Protect(token);

        // Sauvegarder le token chiffré
        File.WriteAllText(filePath, protectedToken);
    }

    public string _GetToken()
    {
        if (File.Exists(filePath))
        {
            var protectedToken = File.ReadAllText(filePath);
            var readableToken =  protector.Unprotect(protectedToken); // Déchiffrer le token
            _token = readableToken;
            return readableToken;
        }
        throw new FileNotFoundException("Token file not found.");
    }

    public void DeleteToken()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _token = null;
        }
    }
}
