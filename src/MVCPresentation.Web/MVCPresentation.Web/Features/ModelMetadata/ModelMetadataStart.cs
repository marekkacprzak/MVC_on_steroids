
using System.Web.Mvc;
using MVCPresentation.Web.Features.ModelMetadata;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(ModelMetadataStart), "PostStart")]

namespace MVCPresentation.Web.Features.ModelMetadata
{
    public class ModelMetadataStart
    {
         public static void PostStart()
         {
            ModelMetadataProviders.Current = MvcApplication.Container.Resolve<ModelMetadataProvider>();

         }
    }
}