using Notblet.Constants;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Windows;

public class ApiService
{
    // Instance unique de ApiService
    private static readonly Lazy<ApiService> _instance = new Lazy<ApiService>(() => new ApiService());

    // Propriété pour accéder à l'instance unique
    public static ApiService Instance => _instance.Value;

    // HttpClient unique pour toute l'application
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiBaseUrl = ApiConstants.BaseApiUrl;

    // Constructeur privé pour éviter la création d'instances en dehors de la classe
    private ApiService()
    {
        _httpClient.BaseAddress = new Uri(_apiBaseUrl);
    }

    // Fonction GET avec token facultatif
    public async Task<string> GetDataAsync(string endpoint, int? id = null, string token = null)
    {
        try
        {
            // Si un token est fourni, l'ajouter dans les en-têtes
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            string _endpoint = id == null ? endpoint : $"{endpoint}/{id}";
            HttpResponseMessage response = await _httpClient.GetAsync(_endpoint);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            else
            {
                return $"Erreur {response.StatusCode}: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Une erreur est survenue : {ex.Message}";
        }
    }

    // Fonction POST avec token facultatif
    public async Task<string> PostDataAsync(string endpoint, string jsonData, string token = null)
    {
        try
        {
            // Si un token est fourni, l'ajouter dans les en-têtes
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            else
            {
                throw new Exception($"Erreur {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Une erreur inconnue est survenue", ex);
        }
    }

    // Fonction PUT avec token facultatif
    public async Task<string> PutDataAsync(string endpoint, string jsonData, string token = null)
    {
        try
        {
            // Si un token est fourni, l'ajouter dans les en-têtes
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            else
            {
                throw new Exception($"Erreur {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Une erreur inconnue est survenue", ex);
        }
    }

    // Fonction DELETE avec token facultatif
    public async Task<string> DeleteDataAsync(string endpoint, int id, string token = null)
    {
        try
        {
            // Si un token est fourni, l'ajouter dans les en-têtes
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint + '/' + id);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            else
            {
                throw new Exception($"Erreur {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Une erreur inconnue est survenue", ex);
        }
    }
}
