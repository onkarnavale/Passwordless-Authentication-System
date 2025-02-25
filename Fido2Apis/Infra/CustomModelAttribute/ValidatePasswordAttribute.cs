using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Fido2Apis.Infra.CustomModelAttribute
{
    public class ValidatePasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null && value.ToString() is string stringValue)
            {
                if (stringValue.Length > 0)
                {
                    bool isError = false;
                    string ErrorMessage = "Password must contain";

                    if (stringValue.Length < 8 || stringValue.Length > 15) 
                    { 
                        isError = true; 
                        ErrorMessage = ErrorMessage + $" minimum 8 characters and maximum 15 characters,"; 
                    }

                    if (!Regex.IsMatch(stringValue, @"[A-Z]")) 
                    { 
                        isError = true; 
                        ErrorMessage = ErrorMessage + " one uppercase letter,"; 
                    }

                    if (!Regex.IsMatch(stringValue, @"[a-z]")) 
                    { 
                        isError = true; ErrorMessage = ErrorMessage + " one lowercase letter,"; 
                    }
                    
                    if (!Regex.IsMatch(stringValue, @".[!@#&$%{}].")) 
                    { 
                        isError = true; 
                        ErrorMessage = ErrorMessage + " one special character,"; 
                    }

                    
                    if (!Regex.IsMatch(stringValue, @"[0-9]")) 
                    { 
                        isError = true; 
                        ErrorMessage = ErrorMessage + " one numerical digit,"; 
                    }

                    if (isError) 
                    { 
                        return new ValidationResult(ErrorMessage.Substring(0, ErrorMessage.Length - 1)); 
                    } 
                    else 
                    { 
                        return ValidationResult.Success; 
                    }
                }
                else 
                { 
                    return ValidationResult.Success; 
                }
            }
            return new ValidationResult("Password validation failed");
        }
    }
}