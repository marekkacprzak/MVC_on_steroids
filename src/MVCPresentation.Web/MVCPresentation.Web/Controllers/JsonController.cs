using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace MVCPresentation.Web.Controllers
{
    public class JsonController : Controller
    {
        private readonly Stopwatch _sw = new Stopwatch();

        public JsonResult GetBigJson()
        {
            var data = new List<SomeDTO>();
            for (var i = 0; i < 10000; i++)
            {
                data.Add(CreateDto());
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            _sw.Start();
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            _sw.Stop();

            filterContext.HttpContext.Response.AddHeader("X-Duration", _sw.Elapsed.ToString());
        }

        private static SomeDTO CreateDto()
        {
            return new SomeDTO
                {
                    Date = DateTime.Now,
                    Id = 10,
                    IsVisible = true,
                    Name = "test dto"
                };
        }

        public class SomeDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public bool IsVisible { get; set; }
        }
    }
}