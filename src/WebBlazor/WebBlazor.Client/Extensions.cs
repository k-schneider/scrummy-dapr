namespace Scrummy.WebBlazor.Client;

public static class Extensions
{
    public static string GetError(this ApiException exc)
    {
        var result = "Unknown error";

        switch (exc.StatusCode)
        {
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.NotFound:
                result = string.IsNullOrWhiteSpace(exc.Content) ? result : exc.Content;
                break;
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                result = "Server is not responding";
                break;
        }

        return result;
    }
}
