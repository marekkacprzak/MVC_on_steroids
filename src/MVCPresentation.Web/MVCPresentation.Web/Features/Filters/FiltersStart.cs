using System.Web.Mvc;

//[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(MVCPresentation.Web.Features.JSON.FiltersStart), "PreStart")]
namespace MVCPresentation.Web.Features.Filters
{
    public class FiltersStart
    {
        public static void PostStart()
        {
            FilterProviders.Providers.Add(new TransactionFilterProvider(MvcApplication.Container));
        }
    }
}