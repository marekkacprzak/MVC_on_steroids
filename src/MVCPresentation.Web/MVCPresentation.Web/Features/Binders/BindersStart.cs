using System.Web.Mvc;
using System.Linq;
using Castle.Core;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(MVCPresentation.Web.Features.Binders.BindersStart), "PostStart")]

namespace MVCPresentation.Web.Features.Binders
{
    public class BindersStart
    {
        public static void PostStart()
        {
            var container = MvcApplication.Container;

            var singletonsOrPerWebRequests =
                container.Kernel.GetAssignableHandlers(typeof (object))
                         .Select(h => h.ComponentModel)
                         .Distinct()
                         .Where(
                             cm =>
                             cm.LifestyleType == LifestyleType.Singleton ||
                             cm.LifestyleType == LifestyleType.PerWebRequest)
                         .SelectMany(cm => cm.Services)
                         .Distinct();

            var binder = new DependencyInjectionBinder(container);

            foreach (var service in singletonsOrPerWebRequests)
            {
                ModelBinders.Binders.Add(service, binder);
            }
        }
    }
}