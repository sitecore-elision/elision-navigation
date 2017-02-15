using Elision.Feature.Template.Navigation.SC.Integration;
using Elision.Feature.Template.Navigation.SC.Integration.Models;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Elision.Feature.Template.Navigation
{
    public class RegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<INavigationRepository<NavigationItem>, NavigationRepository<NavigationItem>>();
        }
    }
}
