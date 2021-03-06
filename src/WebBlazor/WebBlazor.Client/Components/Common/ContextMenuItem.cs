namespace Scrummy.WebBlazor.Client.Components.Common;

public class ContextMenuItem : ComponentBase, IDisposable
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public bool NoSpanWrap { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [CascadingParameter()]
    public ContextMenu? ContextMenu { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        if (ContextMenu != null)
        {
            ContextMenu.AddItem(this);
        }
        return base.OnInitializedAsync();
    }

    void IDisposable.Dispose()
    {
        if (ContextMenu != null)
        {
            ContextMenu.RemoveItem(this);
        }
    }
}
