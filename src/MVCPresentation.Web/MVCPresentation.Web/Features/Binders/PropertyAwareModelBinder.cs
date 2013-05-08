using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace MVCPresentation.Web.Features.Binders
{
    /// <summary>
    /// Just a sketch of propertyAware model binder, which may bind properties based on their properties, not only their types.
    /// </summary>
    public class PropertyAwareModelBinder : DefaultModelBinder
    {
        private static readonly IPropertyAwareModelBinderProvider[] NullObject = new IPropertyAwareModelBinderProvider[0];
        private readonly IPropertyAwareModelBinderProvider[] _propertyAwareBinderProviders;

        public PropertyAwareModelBinder(IPropertyAwareModelBinderProvider[] propertyAwareBinders)
        {
            _propertyAwareBinderProviders = propertyAwareBinders ?? NullObject;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var subPropertyName = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
            if (!bindingContext.ValueProvider.ContainsPrefix(subPropertyName))
                return;
            var binder = GetBinder(propertyDescriptor);
            var obj = propertyDescriptor.GetValue(bindingContext.Model);
            var modelMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            modelMetadata.Model = obj;
            var bindingContext1 = new ModelBindingContext
                {
                    ModelMetadata = modelMetadata,
                    ModelName = subPropertyName,
                    ModelState = bindingContext.ModelState,
                    ValueProvider = bindingContext.ValueProvider
                };
            var propertyValue = GetPropertyValue(controllerContext, bindingContext1, propertyDescriptor, binder);
            modelMetadata.Model = propertyValue;
            var modelState = bindingContext.ModelState[subPropertyName];
            if (modelState == null || modelState.Errors.Count == 0)
            {
                if (!OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, propertyValue))
                    return;
                SetProperty(controllerContext, bindingContext, propertyDescriptor, propertyValue);
                OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, propertyValue);
            }
            else
            {
                SetProperty(controllerContext, bindingContext, propertyDescriptor, propertyValue);
                foreach (var modelError in modelState.Errors.Where(err =>
                    {
                        if (string.IsNullOrEmpty(err.ErrorMessage))
                            return err.Exception != null;
                        return false;
                    }).ToList())
                {
                    for (var exception = modelError.Exception; exception != null; exception = exception.InnerException)
                    {
                        if (exception is FormatException)
                        {
                            var displayName = modelMetadata.GetDisplayName();
                            var errorMessage = string.Format("{0}{1}", new object[] { modelState.Value.AttemptedValue, displayName });
                            modelState.Errors.Remove(modelError);
                            modelState.Errors.Add(errorMessage);
                            break;
                        }
                    }
                }
            }
        }

        private IModelBinder GetBinder(PropertyDescriptor propertyDescriptor)
        {
            var propertyBinderProvider = _propertyAwareBinderProviders.FirstOrDefault(provider => provider.GetBinder(propertyDescriptor) != null);
            if (propertyBinderProvider != null)
                return propertyBinderProvider.GetBinder(propertyDescriptor);

            return Binders.GetBinder(propertyDescriptor.PropertyType);
        }
    }

    public interface IPropertyAwareModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Gets a model binder for the specified property.
        /// </summary>
        /// <returns>
        /// </returns>
        IModelBinder GetBinder(PropertyDescriptor property);
    }
}