namespace SIMS.Shared.Models
{
    public class Filters<TItem>
    {
        public string Header { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public string SelectedValue { get; set; }
    }
}
