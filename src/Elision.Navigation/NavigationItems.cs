using System.Collections.Generic;

namespace Elision.Navigation
{
    public class NavigationItems<TNavItem> where TNavItem : INavigationItem<TNavItem>
    {
        public IList<TNavItem> Items { get; set; }
    }
}