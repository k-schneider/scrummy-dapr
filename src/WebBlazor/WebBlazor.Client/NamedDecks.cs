public static class NamedDecks
{
    public static NamedDeck ModifiedFibonacci = new NamedDeck("modfib", "Modified Fibonacci",
        new[]
        {
            new Card("0", 0),
            new Card("1/2", 0.5),
            new Card("1", 1),
            new Card("2", 2),
            new Card("3", 3),
            new Card("5", 5),
            new Card("8", 8),
            new Card("13", 13),
            new Card("20", 20),
            new Card("40", 40),
            new Card("100", 100),
            new Card("Infinite", image: "img/infinite.png"),
            new Card("?"),
            new Card("Coffee", image: "img/coffee.png"),
        });

    public static NamedDeck StandardFibonacci = new NamedDeck("stdfib", "Standard Fibonacci",
        new[]
        {
            new Card("0", 0),
            new Card("1", 1),
            new Card("2", 2),
            new Card("3", 3),
            new Card("5", 5),
            new Card("8", 8),
            new Card("13", 13),
            new Card("21", 21),
            new Card("34", 34),
            new Card("55", 55),
            new Card("89", 89),
            new Card("Infinite", image: "img/infinite.png"),
            new Card("?"),
            new Card("Coffee", image: "img/coffee.png"),
        });

    public static NamedDeck TShirtSizes = new NamedDeck("tshirt", "T-Shirt Sizes",
        new[]
        {
            new Card("XS"),
            new Card("SM"),
            new Card("M"),
            new Card("L"),
            new Card("XL"),
        });

    public static IEnumerable<NamedDeck> All => new[]
    {
        ModifiedFibonacci,
        StandardFibonacci,
        TShirtSizes
    };
}

public class NamedDeck
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Card> Cards { get; set; }

    public NamedDeck(string id, string name, IEnumerable<Card> cards)
    {
        Id = id;
        Name = name;
        Cards = cards;
    }
}
