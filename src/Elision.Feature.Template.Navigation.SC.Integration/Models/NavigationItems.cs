using System.Collections.Generic;

namespace Elision.Feature.Template.Navigation.SC.Integration.Models
{
    public class NavigationItems<TNavItem> where TNavItem : INavigationItem<TNavItem>
    {
        public IList<TNavItem> Items { get; set; }
    }
}