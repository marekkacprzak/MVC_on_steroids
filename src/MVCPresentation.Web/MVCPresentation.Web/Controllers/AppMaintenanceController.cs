using System.Web.Mvc;
using MVCPresentation.Web.Models;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Configuration = NHibernate.Cfg.Configuration;

namespace MVCPresentation.Web.Controllers
{
    public class AppMaintenanceController : Controller
    {
        private readonly Configuration _configuration;
        private readonly ISession _session;
        public const string ActionExecutedKey = "ActionExecutedKey";

        public AppMaintenanceController(Configuration configuration, ISession session)
        {
            _configuration = configuration;
            _session = session;
        }

        public ViewResult Index()
         {
             return View();
         }

        public ActionResult CreateDatabase()
        {
            new SchemaExport(_configuration).Create(false, true);
            ViewData[ActionExecutedKey] = "The database has been created";
            return View("Index");
        }


        public ActionResult AddSampleData()
        {
            using (var tran = _session.BeginTransaction())
            {
                var productType = new ProductType() {Name = "Food"};
                _session.Save(productType);
                _session.Save(new ProductType() { Name = "Drink" });
                _session.Save(new Product() { Name = "Pizza",ProductType = productType});

                ViewData[ActionExecutedKey] = "Data added";

                tran.Commit();
            }

            return View("Index");
        }
    }
}