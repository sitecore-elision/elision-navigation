using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;

namespace Elision.Navigation.Areas.Elision.Controllers
{
    public class NavigationController : SitecoreController
    {
        private readonly INavigationRepository<NavigationItem> _navigationRepository;

        public NavigationController(INavigationRepository<NavigationItem> navigationRepository)
        {
            _navigationRepository = navigationRepository;
        }

        public ActionResult Primary(Item pageContextItem, int? navigationLevels, Item renderingDataSourceItem)
        {
            var model = _navigationRepository.GetPrimaryMenu(pageContextItem, navigationLevels.GetValueOrDefault(0), renderingDataSourceItem);
            return View(model);
        }

        public ActionResult Secondary(Item pageContextItem)
        {
            var model = _navigationRepository.GetSecondaryMenu(pageContextItem);
            return View(model);
        }

        public ActionResult Breadcrumbs(Item pageContextItem)
        {
            var model = _navigationRepository.GetBreadcrumbs(pageContextItem);
            return View(model);
        }
    }
}