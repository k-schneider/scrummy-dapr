namespace Scrummy.BlazorClient;

public static class Extensions
{
    public static async Task<T> ReadAsAsync<T>(this HttpContent content) =>
        (await JsonSerializer.DeserializeAsync<T>(
            await content.ReadAsStreamAsync(),
            new JsonSerializerOptions(JsonSerializerDefaults.Web)))!;
}
