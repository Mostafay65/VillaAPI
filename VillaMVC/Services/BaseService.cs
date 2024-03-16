using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using VillaMVC.Models;
using VillaMVC.Models;
using VillaMVC.Service.IService;
using VillaUtility;

namespace VillaMVC.Service;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenProvider _tokenProvider;
    public APIResponse _Response { get; set; }
    public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
        _Response = new APIResponse();
    }
    public async Task<T> SendAsync<T>(APIRequest apiRequest)
    {
        try
        {
            HttpClient clint = _httpClientFactory.CreateClient("VillaAPI");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = apiRequest.Method;
            message.RequestUri = new Uri(apiRequest.URL);
            if (apiRequest.ContentType == SD.ContentType.MultipartFormData)
            {
                message.Headers.Add("Accept", "*/*");
                var content = new MultipartFormDataContent();
                foreach (var prop in apiRequest.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(apiRequest.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file is not null)
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                    }
                    else
                        content.Add(new StringContent(value is null?"" : value.ToString()), prop.Name);
                }
                message.Content = content; 
            }
            else 
            {
                message.Headers.Add("Accept", "application/json");
                if (apiRequest.Data is not null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
            }
            if ( _tokenProvider.GetToken() is not null)
            {
                clint.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_tokenProvider.GetToken().AccessToken);
            }
            HttpResponseMessage responseMessage = await clint.SendAsync(message);
            var response = await responseMessage.Content.ReadAsStringAsync();
            var ResponceEntity = JsonConvert.DeserializeObject<T>(response);
            return ResponceEntity;
        }
        catch (Exception e)
        {
            APIResponse response = new APIResponse()
            {
                ErrorMessages = new List<string>() { e.Message.ToString() },
                StatusCode = HttpStatusCode.BadRequest
            };
            string jsonObject = JsonConvert.SerializeObject(response);
            return JsonConvert.DeserializeObject<T>(jsonObject);
        }
    }
}