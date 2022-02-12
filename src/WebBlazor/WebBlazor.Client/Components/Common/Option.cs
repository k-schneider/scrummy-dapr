namespace Scrummy.WebBlazor.Client.Components.Common;

public class Option : ComponentBase, IDisposable
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter()]
    public Select? Select { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        if (Select != null)
        {
            Select.AddOption(this);
        }
        return base.OnInitializedAsync();
    }

    void IDisposable.Dispose()
    {
        if (Select != null)
        {
            Select.RemoveOption(this);
        }
    }
}
