
using System.Web.Mvc;
using MVCPresentation.Web.Features.DependencyInjection;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(DependencyInjectionStart), "PostStart")]

namespace MVCPresentation.Web.Features.DependencyInjection
{
    public class DependencyInjectionStart
    {
         public static void PostStart()
         {
             ControllerBuilder.Current.SetControllerFactory(MvcApplication.Container.Resolve<IControllerFactory>());

             ViewEngines.Engines.Clear();
             ViewEngines.Engines.Add(new RazorViewEngine(new CastleViewPageActivator(MvcApplication.Container.Kernel)));
         }
    }
}