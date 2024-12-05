using Notblet.Constants;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using NLog;

public class ApiService
{
    // Instance unique de ApiService
    private static readonly Lazy<ApiService> _instance = new Lazy<ApiService>(() => new ApiService());

    // Propriété pour accéder à l'instance unique
    public static ApiService Instance => _instance.Value;

    // HttpClient unique pour toute l'application
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiBaseUrl = ApiConstants.BaseApiUrl;

    // Logger pour la classe
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // Constructeur privé pour éviter la création d'instances en dehors de la classe
    private ApiService()
    {
        _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        Logger.Info("HttpClient configuré avec l'URL de base : {0}", _apiBaseUrl);
    }

    // Fonction GET avec token facultatif
    public async Task<string> GetDataAsync(string endpoint, int? id = null, string token = null)
    {
        try
        {
            Logger.Info("Appel GET sur l'endpoint : {0}, avec ID : {1}", endpoint, id);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Logger.Info("Token ajouté à la requête GET.");
            }

            string _endpoint = id == null ? endpoint : $"{endpoint}/{id}";
            HttpResponseMessage response = await _httpClient.GetAsync(_endpoint);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                Logger.Info("Réponse GET réussie avec statut : {0}", response.StatusCode);
                return responseData;
            }
            else
            {
                Logger.Warn("Erreur lors de l'appel GET : {0} - {1}", response.StatusCode, response.ReasonPhrase);
                return $"Erreur {response.StatusCode}: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Exception lors de l'appel GET");
            return $"Une erreur est survenue : {ex.Message}";
        }
    }

    // Fonction POST avec token facultatif
    public async Task<string> PostDataAsync(string endpoint, string jsonData, string token = null)
    {
        try
        {
            Logger.Info("Appel POST sur l'endpoint : {0}", endpoint);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Logger.Info("Token ajouté à la requête POST.");
            }

            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                Logger.Info("Réponse POST réussie avec statut : {0}", response.StatusCode);
                return responseData;
            }
            else
            {
                Logger.Warn("Erreur lors de l'appel POST : {0} - {1}", response.StatusCode, response.ReasonPhrase);
                throw new Exception($"Erreur {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Exception lors de l'appel POST");
            throw;
        }
    }

    // Fonction PUT avec token facultatif
    public async Task<string> PutDataAsync(string endpoint, string jsonData, string token = null)
    {
        try
        {
            Logger.Info("Appel PUT sur l'endpoint : {0}", endpoint);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Logger.Info("Token ajouté à la requête PUT.");
            }

            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                Logger.Info("Réponse PUT réussie avec statut : {0}", response.StatusCode);
                return responseData;
            }
            else
            {
                Logger.Warn("Erreur lors de l'appel PUT : {0} - {1}", response.StatusCode, response.ReasonPhrase);
                throw new Exception($"Erreur {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Exception lors de l'appel PUT");
            throw;
        }
    }

    // Fonction DELETE avec token facultatif
    public async Task<string> DeleteDataAsync(string endpoint, int id, string token = null)
    {
        try
        {
            Logger.Info("Appel DELETE sur l'endpoint : {0}/{1}", endpoint, id);

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Logger.Info("Token ajouté à la requête DELETE.");
            }

            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint + '/' + id);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                Logger.Info("Réponse DELETE réussie avec statut : {0}", response.StatusCode);
                return responseData;
            }
            else
            {
                Logger.Warn("Erreur lors de l'appel DELETE : {0} - {1}", response.StatusCode, response.ReasonPhrase);
                throw new Exception($"Erreur {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Exception lors de l'appel DELETE");
            throw;
        }
    }
}
