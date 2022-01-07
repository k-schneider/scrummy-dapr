namespace Scrummy.GameService.Api;

public static class GameId
{
    private static readonly string[] _words;
    private static readonly Random _random = new();

    static GameId()
    {
        _words = ReadAllResourceLines("Scrummy.GameService.Api.GameIdWords.txt");
    }

    public static string Generate()
    {
        var word1 = _words[_random.Next(0, _words.Length - 1)];
        var word2 = _words[_random.Next(0, _words.Length - 1)];
        var word3 = _words[_random.Next(0, _words.Length - 1)];
        return $"{word1}-{word2}-{word3}";
    }

    private static string[] ReadAllResourceLines(string resourceName)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return EnumerateLines(reader).ToArray();
    }

    private static IEnumerable<string> EnumerateLines(TextReader reader)
    {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }
}
