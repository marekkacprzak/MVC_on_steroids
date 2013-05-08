using System;
using System.ComponentModel;

namespace MVCPresentation.Web.Features.ModelMetadata
{
    public interface IMetadataDecoratorForProperty
    {
        void DecorateMetadataForProperty(System.Web.Mvc.ModelMetadataProvider provider, System.Web.Mvc.ModelMetadata metadata, Type containerType, PropertyDescriptor property);
    }
}