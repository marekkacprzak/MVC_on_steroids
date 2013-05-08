using System.Web.Mvc;

//[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MVCPresentation.Web.Features.JSON.JsonStart), "PreStart")]

namespace MVCPresentation.Web.Features.JSON
{
    public class JsonStart
    {
        public static void PreStart()
        {
            GlobalFilters.Filters.Add(new JsonResultSerializedWithNewtonSerializer());
        }
    }
}