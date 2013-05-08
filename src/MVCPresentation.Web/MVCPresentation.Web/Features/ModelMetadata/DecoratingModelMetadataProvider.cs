using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace MVCPresentation.Web.Features.ModelMetadata
{
    public class DecoratingModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly IMetadataDecoratorForProperty[] _property;
        private readonly IMetadataDecoratorForType[] _type;

        public DecoratingModelMetadataProvider(IMetadataDecoratorForProperty[] property, IMetadataDecoratorForType[] type)
        {
            _property = property;
            _type = type;
        }

        protected override System.Web.Mvc.ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor)
        {
            var baseMetadata = base.GetMetadataForProperty(modelAccessor, containerType, propertyDescriptor);
            foreach (var decorator in _property)
            {
                decorator.DecorateMetadataForProperty(this, baseMetadata, containerType, propertyDescriptor);
            }
            return baseMetadata;
        }

        public override System.Web.Mvc.ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            var baseMetadata = base.GetMetadataForType(modelAccessor, modelType);
            foreach (var decorator in _type)
            {
                decorator.DecorateTypeMetadata(this, baseMetadata, modelType);
            }
            return baseMetadata;
        }
    }
}