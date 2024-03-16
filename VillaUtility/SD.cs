namespace VillaUtility;

public static class SD
{
    public enum Method
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public static string AccessToken = "JwtAccessToken";
    public static string RefreshToken = "JwtRefreshToken";
    public static string Admin = "Admin";
    public static string Custrom = "Custom";

    public enum ContentType
    {
        Json,
        MultipartFormData
    }
}
