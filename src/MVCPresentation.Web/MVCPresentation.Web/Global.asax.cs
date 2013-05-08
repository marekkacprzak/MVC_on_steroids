using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.Proxy;
using MVCPresentation.Web.Features.DependencyInjection;
using MVCPresentation.Web.Features.ModelMetadata;
using MVCPresentation.Web.Models;
using MVCPresentation.Web.Persistence;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using Configuration = NHibernate.Cfg.Configuration;
using ModelMetadataProvider = System.Web.Mvc.ModelMetadataProvider;

namespace MVCPresentation.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        private static IWindsorContainer _container;

        private static readonly object SynchRoot = new object();

        public static IWindsorContainer Container
        {
            get { return _container; }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            if (_container == null)
            {
                lock (SynchRoot)
                {
                    if (_container == null)
                    {
                        _container = GetContainer();
                    }
                }
            }
        }


        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        private static WindsorContainer GetContainer()
        {
            var kernel = new DefaultKernel(new DefaultProxyFactory());
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel,true));

            var container = new WindsorContainer(kernel, new DefaultComponentInstaller());
            var decoratorTypes = new[] {typeof (IMetadataDecoratorForProperty), typeof (IMetadataDecoratorForType)};
            container.Register(
                Component
                    .For<IKernel>()
                    .Instance(kernel)
                    .LifestyleSingleton(),
                Classes.FromThisAssembly()
                       .Pick()
                       .If(t => typeof (IController).IsAssignableFrom(t))
                       .WithServiceSelf()
                       .WithServices(typeof (IController))
                       .Configure(c => c.LifestyleTransient()),
                Component.For<IControllerFactory>()
                         .ImplementedBy<CastleResolvingControllerFactory>()
                         .LifestyleTransient(),
                Component.For<FluentNhConfigurator>()
                         .ImplementedBy<FluentNhConfigurator>()
                         .LifestyleSingleton(),
                Component.For<Configuration>()
                         .UsingFactoryMethod(k => k.Resolve<FluentNhConfigurator>().GetConfiguration())
                         .LifestyleSingleton(),
                Component.For<ISessionFactory,ISessionFactoryImplementor>()
                         .UsingFactoryMethod(k => k.Resolve<Configuration>().BuildSessionFactory())
                         .LifestyleSingleton(),
                Component.For<IDbConnection>()
                         .UsingFactoryMethod(k =>
                             {
                                 var factory = k.Resolve<ISessionFactoryImplementor>();
                                 var cfg = k.Resolve<Configuration>();
                                 var connection = factory.ConnectionProvider.GetConnection();
                                 
                                 new SchemaExport(cfg).Execute(false,true,false,connection, new StringWriter());

                                 using (var session = factory.OpenSession(connection))
                                 {
                                     using (var tran = session.BeginTransaction())
                                     {
                                         var productType = new ProductType() { Name = "Food" };
                                         session.Save(productType);
                                         session.Save(new ProductType() { Name = "Drink" });
                                         session.Save(new Product() { Name = "Pizza", ProductType = productType });

                                         tran.Commit();
                                     }
                                 }

                                 return connection;
                             })
                         .LifestyleSingleton(),
                Component.For<ISession>()
                         .UsingFactoryMethod(k => k.Resolve<ISessionFactory>().OpenSession(k.Resolve<IDbConnection>())),
                Classes.FromThisAssembly()
                       .Pick()
                       .If(t => decoratorTypes.Any(decorator => decorator.IsAssignableFrom(t)))
                       .WithServiceAllInterfaces()
                       .LifestyleSingleton(),

                Component.For<ModelMetadataProvider>()
                         .ImplementedBy<DecoratingModelMetadataProvider>()
                         .IsDefault()
                         .LifestyleSingleton(),

                Classes.FromAssemblyContaining<IEntity>()
                .Pick()
                .If(t => typeof(Glossary).IsAssignableFrom(t))
                .WithServiceSelf()
                .LifestyleTransient()

                );

            return container;
        }

        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
        }
    }
}