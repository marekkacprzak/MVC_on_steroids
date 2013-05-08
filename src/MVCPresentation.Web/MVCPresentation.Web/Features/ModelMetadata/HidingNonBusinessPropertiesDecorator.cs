using System;
using System.ComponentModel;
using System.Web.Mvc;
using MVCPresentation.Web.Models;

namespace MVCPresentation.Web.Features.ModelMetadata
{
    public class HidingNonBusinessPropertiesDecorator : IMetadataDecoratorForProperty
    {
        public void DecorateMetadataForProperty(ModelMetadataProvider provider, System.Web.Mvc.ModelMetadata metadata, Type containerType, PropertyDescriptor property)
        {
            if (typeof(IEntity).IsAssignableFrom(containerType))
            {
                if (property.Name == "Id")
                {
                    metadata.ShowForDisplay = false;
                    metadata.TemplateHint = "HiddenInput";
                    metadata.HideSurroundingHtml = true;
                }
            }
        }
    }
}