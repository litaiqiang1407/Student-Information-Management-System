// ComponentFunction.cs
using Microsoft.AspNetCore.Components;

namespace SIMS.Shared.Functions
{
    public class DisplayComponent : ComponentBase
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        public bool Display(string[] uris)
        {
            var uri = Navigation.ToBaseRelativePath(Navigation.Uri).TrimEnd('/');
            foreach (var item in uris)
            {
                if (uri == item)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
