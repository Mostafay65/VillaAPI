using System.Net;

namespace VillaMVC.Models;

public class APIResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool Success { get; set; } = false;
    public List<string>? ErrorMessages { get; set; }
    public Object? Result { get; set; } = null;
}