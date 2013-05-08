using System.Web.Mvc;
using Castle.Windsor;

namespace MVCPresentation.Web.Features.Binders
{
    public class DependencyInjectionBinder : IModelBinder
    {
        private readonly IWindsorContainer _container;

        public DependencyInjectionBinder(IWindsorContainer container)
        {
            _container = container;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return _container.Resolve(bindingContext.ModelType);
        }
    }
}