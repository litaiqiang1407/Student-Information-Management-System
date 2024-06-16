namespace SIMS.Shared.Models
{
    public class TableColumn<TItem>
    {
        public string Header { get; set; }
        public Func<TItem, object> Value { get; set; }
        public bool IsFilter { get; set; } = false;
    }
}
