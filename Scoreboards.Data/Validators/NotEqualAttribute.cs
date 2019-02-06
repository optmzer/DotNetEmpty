using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

/**
 * Validates 2 text/select fields and
 * Throws error if 2 text fields are equal
 */

namespace Scoreboards.Data.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotEqualAttribute : ValidationAttribute, IClientModelValidator
    {
        private const string DefaultErrorMessage = "The value of {0} cannot be the same as the value of the {1}.";

        public string _otherProperty { get; private set; }

        public NotEqualAttribute(string otherProperty) : base(DefaultErrorMessage)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value != null)
            {
                var currentVal = (string)value;

                var other_property = validationContext.ObjectType.GetProperty(_otherProperty);

                if (other_property == null)
                    throw new ArgumentException("Property with this name not found");

                var otherPropertyVal = (string)other_property.GetValue(validationContext.ObjectInstance, null);

                // Srings are equal throw error
                if(String.Equals(currentVal, otherPropertyVal))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
            //return base.IsValid(value, validationContext); // Automatically added
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-notequal", FormatErrorMessage(context.ModelMetadata.Name));

            var otherProp = _otherProperty.ToString(CultureInfo.InvariantCulture);
            MergeAttribute(context.Attributes, "data-val-notequal-other", otherProp);
        }

        /**
         * Adds atributes to the html element
         */ 
        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, _otherProperty);
        }
    }
}
