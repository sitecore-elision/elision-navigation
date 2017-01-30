using System.Linq;
using Sitecore.Data.Items;

namespace Elision.Feature.Template.Navigation
{
    public class NavigationItem : INavigationItem<NavigationItem>
    {
        public Item Item { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public int Level { get; set; }
        public NavigationItems<NavigationItem> Children { get; set; }
        public string Target { get; set; }
        public string Text { get; set; }
        public string CssClass { get; set; }

        public bool HasChildren => Children?.Items != null && Children.Items.Any();
    }
}