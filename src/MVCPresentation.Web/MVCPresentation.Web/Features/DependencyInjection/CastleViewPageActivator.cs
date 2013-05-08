using System;
using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Proxy;

namespace MVCPresentation.Web.Features.DependencyInjection
{
    /// <summary>
    /// The view page activator autoregistering created types in the <see cref="IKernel"/>
    /// and using it to resolve them.
    /// </summary>
    public sealed class CastleViewPageActivator : IViewPageActivator
    {
        private readonly IKernel _root;
        private DefaultKernel _childKernel;

        public CastleViewPageActivator(IKernel root)
        {
            _root = root;

            CreateNewChildKernel();
        }

        private void CreateNewChildKernel()
        {
            lock (_root)
            {
                if (_childKernel != null)
                {
                    _root.RemoveChildKernel(_childKernel);
                    _childKernel.Dispose();
                }

                _childKernel = InstantiateKernel();
                _root.AddChildKernel(_childKernel);
            }
        }

        private static DefaultKernel InstantiateKernel()
        {
            var k = new DefaultKernel(new DefaultProxyFactory());
            return k;
        }

        public object Create(ControllerContext controllerContext, Type type)
        {
            if (_childKernel.HasComponent(type) == false)
            {
                lock (_root)
                {
                    if (_childKernel.HasComponent(type) == false)
                    {
                        // try find with name (based on knowledge how Castle works internally)
                        // if it's found, it means that a view was changed after application started up
                        var registrationName = type.FullName;

                        if (_childKernel.HasComponent(registrationName))
                        {
                            CreateNewChildKernel();
                        }

                        RegisterView(type);
                    }
                }
            }

            return _childKernel.Resolve(type);
        }

        private void RegisterView(Type type)
        {
            _childKernel.Register(Component.For(type).ImplementedBy(type).LifestyleTransient());
        }
    }
}