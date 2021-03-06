using Sitecore.Data.Items;

namespace Elision.Feature.Template.Navigation.SC.Integration.Models
{
    public interface INavigationItem<TNavItem> where TNavItem : INavigationItem<TNavItem>
    {
        Item Item { get; set; }
        string Url { get; set; }
        bool IsActive { get; set; }
        int Level { get; set; }
        NavigationItems<TNavItem> Children { get; set; }
        string Target { get; set; }
        string Text { get; set; }
        string CssClass { get; set; }
        bool HasChildren { get; }
    }
}