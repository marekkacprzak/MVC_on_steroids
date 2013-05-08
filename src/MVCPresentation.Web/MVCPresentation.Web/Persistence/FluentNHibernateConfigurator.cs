using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MVCPresentation.Web.Models;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace MVCPresentation.Web.Persistence
{
    public class FluentNhConfigurator
    {
        public const string ConnectionStringName = "db";

        public Configuration GetConfiguration()
        {
            return Fluently.Configure(new Configuration())
                           .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                           .Database(SQLiteConfiguration.Standard.InMemory()
                                                        .Dialect<SQLiteDialect>())
                           .ExposeConfiguration(c => c.SetProperty("hbm2ddl.keywords", "auto-quote"))
                           .Mappings(m =>
                                     m.AutoMappings.Add(
                                         AutoMap.Assemblies(new AppAutomappingCfg(),
                                                            typeof (FluentConfiguration).Assembly,
                                                            typeof (IEntity).Assembly)
                                                .UseOverridesFromAssemblyOf<FluentNhConfigurator>()
                                                .Conventions.AddFromAssemblyOf<FluentNhConfigurator>()))
                           .BuildConfiguration();
        }
    }
}