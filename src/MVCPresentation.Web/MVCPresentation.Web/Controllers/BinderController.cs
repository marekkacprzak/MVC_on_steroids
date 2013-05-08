using System.Web.Mvc;
using NHibernate;

namespace MVCPresentation.Web.Controllers
{
    public class BinderController : Controller
    {
        public ContentResult SessionFactory(ISessionFactory factory)
        {
            return Content("I've got all I need!");
        }
    }
}