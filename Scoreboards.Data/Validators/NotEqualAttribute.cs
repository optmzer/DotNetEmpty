using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;


/**
 * Throws error if 2 text fields are equal
 */ 

namespace Scoreboards.Data.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotEqualAttribute : ValidationAttribute, IClientModelValidator
    {
        private const string DefaultErrorMessage = "The value of {0} cannot be the same as the value of the {1}.";

        private readonly string _otherProperty;

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
            var error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-equalto", error);
            context.Attributes.Add("data-val-equalto-other", "*.User_Id_02");
        }
    }
}
