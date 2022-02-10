namespace Scrummy.AppContracts;

public record Card
{
    public string Id { get; init; } = null!;
    public string? Label { get; init; }
    public double? NumericValue { get; init; }
    public string? Image { get; init; }

    // Needed for serialization
    private Card() { }

    public Card(string? label = null, double? numericValue = null, string? image = null)
    {
        if (string.IsNullOrEmpty(label) && !numericValue.HasValue && string.IsNullOrEmpty(image))
        {
            throw new ArgumentException("At least one of the card properties must be set.");
        }

        Id = Guid.NewGuid().ToString();
        Label = label;
        NumericValue = numericValue;
        Image = image;
    }
}
