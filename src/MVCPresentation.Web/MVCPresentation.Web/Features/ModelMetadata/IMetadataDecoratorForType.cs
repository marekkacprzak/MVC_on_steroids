using System;

namespace MVCPresentation.Web.Features.ModelMetadata
{
    public interface IMetadataDecoratorForType
    {
        void DecorateTypeMetadata(System.Web.Mvc.ModelMetadataProvider provider, System.Web.Mvc.ModelMetadata metadata, Type modelType);
    }
}