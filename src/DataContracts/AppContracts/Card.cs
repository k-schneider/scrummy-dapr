namespace Scrummy.AppContracts;

public record Card
{
    public string Id { get; init; } = null!;
    public string Label { get; init; } = null!;
    public double? NumericValue { get; init; }
    public string? Image { get; init; }

    // Needed for serialization
    private Card() { }

    public Card(string label, double? numericValue = null, string? image = null)
    {
        Id = Guid.NewGuid().ToString();
        Label = label;
        NumericValue = numericValue;
        Image = image;
    }
}
