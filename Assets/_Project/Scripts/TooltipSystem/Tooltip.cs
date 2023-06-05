namespace TooltipSystem
{
    public struct Tooltip
    {
        public string Text;

        public bool ShouldShow;

        public Tooltip(string tooltipText, bool shouldShow = true)
        {
            Text = tooltipText;
            ShouldShow = shouldShow;
        }

        public static readonly Tooltip Empty = new(string.Empty, false);

        public static implicit operator Tooltip(string tooltipText) => new(tooltipText, true);

        public static implicit operator string(Tooltip tooltip) => tooltip.Text;
    }
}
