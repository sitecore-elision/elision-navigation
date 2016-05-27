using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore;
using Sitecore.Links;

namespace Elision.Navigation
{
    public interface INavigationRepository<TNavItem> where TNavItem : class, INavigationItem<TNavItem>, new()
    {
        NavigationItems<TNavItem> GetBreadcrumbs(Item contextItem);
        NavigationItems<TNavItem> GetPrimaryMenu(Item contextItem, int levels = 2);
        TNavItem GetSecondaryMenu(Item contextItem);
        NavigationItems<TNavItem> GetLinkMenuItems(Item menuRoot, Item contextItem, int levels = 1);
    }

    public class NavigationRepository<TNavItem> : INavigationRepository<TNavItem> where TNavItem : class, INavigationItem<TNavItem>, new()
    {
        public NavigationItems<TNavItem> GetBreadcrumbs(Item contextItem)
        {
            var items = new NavigationItems<TNavItem>
            {
                Items = GetNavigationHierarchy(contextItem, true).Reverse().ToList()
            };

            for (var i = 0; i < items.Items.Count - 1; i++)
            {
                items.Items[i].Level = i;
                items.Items[i].IsActive = i == (items.Items.Count - 1);
            }

            return items;
        }

        public NavigationItems<TNavItem> GetPrimaryMenu(Item contextItem, int levels = 2)
        {
            var navigationRoot = GetNavigationRoot(contextItem);
            var navItems = this.GetChildNavigationItems(navigationRoot, contextItem, 0, levels - 1);

            AddRootToPrimaryMenu(navItems, contextItem, navigationRoot);
            return navItems;
        }

        public TNavItem GetSecondaryMenu(Item contextItem)
        {
            var rootItem = this.GetSecondaryMenuRoot(contextItem);
            return rootItem == null ? null : CreateNavigationItem(rootItem, contextItem, 0, 3);
        }

        public NavigationItems<TNavItem> GetLinkMenuItems(Item menuRoot, Item contextItem, int levels = 1)
        {
            if (menuRoot == null)
                throw new ArgumentNullException(nameof(menuRoot));

            return GetChildNavigationItems(menuRoot, contextItem, 0, levels - 1);
        }

        public virtual Item GetNavigationRoot(Item contextItem)
        {
            return contextItem?.GetSite()?.GetStartItem();
        }

        protected virtual void AddRootToPrimaryMenu(NavigationItems<TNavItem> navItems, Item contextItem, Item navigationRoot)
        {
            if (!IncludeInNavigation(navigationRoot))
                return;

            var navigationItem = this.CreateNavigationItem(navigationRoot, contextItem, 0, 0);

            navigationItem.IsActive = contextItem.ID == navigationRoot.ID;
            navItems?.Items?.Insert(0, navigationItem);
        }

        protected virtual bool IncludeInNavigation(Item item, bool forceShowInMenu = false)
        {
            if (item.InheritsFrom(Templates.MenuLink.TemplateId))
                return true;

            return /*item.HasContextLanguage() &&*/
                item.InheritsFrom(Templates._Navigable.TemplateId)
                && (forceShowInMenu || MainUtil.GetBool(item[Templates._Navigable.FieldNames.ShowInNavigation], false));
        }

        protected virtual Item GetSecondaryMenuRoot(Item contextItem)
        {
            return FindActivePrimaryMenuItem(contextItem);
        }

        protected virtual Item FindActivePrimaryMenuItem(Item contextItem)
        {
            var navigationRoot = GetNavigationRoot(contextItem);

            var primaryMenuItems = GetPrimaryMenu(contextItem);
            return primaryMenuItems?.Items?.FirstOrDefault(i => i.Item.ID != navigationRoot.ID && i.IsActive)?.Item;
        }

        protected virtual IEnumerable<TNavItem> GetNavigationHierarchy(Item contextItem, bool forceShowInMenu = false)
        {
            var item = contextItem;
            while (item != null)
            {
                if (this.IncludeInNavigation(item, forceShowInMenu))
                    yield return this.CreateNavigationItem(item, contextItem, 0);

                item = item.Parent;
            }
        }

        protected virtual TNavItem CreateNavigationItem(Item item, Item contextItem, int level, int maxLevel = -1)
        {
            return new TNavItem
            {
                Item = item,
                Url = (item.InheritsFrom(Templates.MenuLink.TemplateId) ? item.LinkFieldUrl(Templates.MenuLink.FieldIds.Link) : LinkManager.GetItemUrl(item)),
                Target = (item.InheritsFrom(Templates.MenuLink.TemplateId) ? item.LinkFieldTarget(Templates.MenuLink.FieldIds.Link) : ""),
                IsActive = IsItemActive(item, contextItem),
                Text = item[Templates._Navigable.FieldIds.NavigationText].Or(item.LinkFieldDescription(Templates.MenuLink.FieldNames.Link)).Or(item.DisplayName).Or(item.Name),
                Level = level,
                CssClass = item.LinkFieldClass(Templates.MenuLink.FieldNames.Link),
                Children = GetChildNavigationItems(item, contextItem, level + 1, maxLevel)
            };
        }

        protected virtual NavigationItems<TNavItem> GetChildNavigationItems(Item parentItem, Item contextItem, int level, int maxLevel)
        {
            if (maxLevel < 0 && (level > maxLevel || !parentItem.HasChildren))
            {
                return null;
            }
            var childItems =
                parentItem.Children.Where(item => IncludeInNavigation(item))
                    .Select(i => CreateNavigationItem(i, contextItem, level, maxLevel));
            return new NavigationItems<TNavItem>
            {
                Items = childItems.ToList()
            };
        }

        protected virtual bool IsItemActive(Item item, Item contextItem)
        {
            return contextItem.ID == item.ID || contextItem.Axes.GetAncestors().Any(a => a.ID == item.ID);
        }
    }
}