using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MVCPresentation.Web.Features.JSON
{
    public class JsonResultSerializedWithNewtonSerializer : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var json = filterContext.Result as JsonResult;
            if (json != null)
            {
                var response = IfOnlyMvcHadItExtractedInJsonResult(filterContext, json);
                if (json.Data == null)
                    return;

                using (var writer = new StringWriter())
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, json.Data);
                    response.Write(writer.ToString());
                }
            }
        }

        private static HttpResponseBase IfOnlyMvcHadItExtractedInJsonResult(ControllerContext filterContext,
                                                                            JsonResult json)
        {
            if (json.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET",
                              StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON Get disallowed");
            var response = filterContext.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(json.ContentType) ? "application/json" : json.ContentType;
            if (json.ContentEncoding != null)
                response.ContentEncoding = json.ContentEncoding;
            return response;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}