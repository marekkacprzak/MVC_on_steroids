using System.Web.Http;
using System.Web.Mvc;
using MVCPresentation.Web.Models;
using NHibernate;

namespace MVCPresentation.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ISession _session;

        public ProductController(ISession session)
        {
            _session = session;
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Add()
         {
             return View();
         }

        public ActionResult List()
        {
            var products = _session.QueryOver<Product>().List();
            return View(products);
        }
    }
}