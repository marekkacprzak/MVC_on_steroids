using System;
using System.Web.Mvc;
using Castle.MicroKernel;

namespace MVCPresentation.Web.Features.DependencyInjection
{
    public class CastleResolvingControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        public CastleResolvingControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                throw new ArgumentException("The controller type is null. You probably entered wrong url.");
            return (IController)_kernel.Resolve(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }
    }
}