using FluentValidation;
using SampleProject.Infrastructure.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SampleProject.Infrastructure.Fluent
{
    public class GenericValidator<T> : AbstractValidator<T>
    {
        public GenericValidator()
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var propertyName = property.Name;

                // Creating a parameter expression for the property of T
                var parameter = Expression.Parameter(typeof(T), "x");

                // Creating an expression for the property
                var propertyExpression = Expression.Property(parameter, property);

                // Creating a strongly-typed lambda expression for RuleFor
                var lambda = Expression.Lambda(Expression.Convert(propertyExpression, typeof(object)), parameter);

                if (propertyType == typeof(int))
                {
                    RuleFor(Expression.Lambda<Func<T, int?>>(Expression.Convert(propertyExpression, typeof(int?)), parameter))
                        .Must(value => value == null || value >= 0)
                        .WithMessage($"{propertyName} must be greater than or equal to 0.");
                }
                else if (propertyType == typeof(string))
                {
                    RuleFor(Expression.Lambda<Func<T, string>>(propertyExpression, parameter))
                        .NotEmpty().WithMessage($"{propertyName} cannot be empty or whitespace.")
                        .Length(8, int.MaxValue).WithMessage($"{propertyName} must have at least 8 characters.")
                        .When(x => propertyName == "Email", ApplyConditionTo.CurrentValidator)
                        .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage($"{propertyName} must be a valid email address.")
                        .When(x => propertyName == "Password", ApplyConditionTo.CurrentValidator);
                        //.Matches(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").WithMessage($"{propertyName} must contain at least one uppercase letter, one number, and one special character.");
                }
                else if (propertyType == typeof(DateTime))
                {
                    RuleFor(Expression.Lambda<Func<T, DateTime?>>(Expression.Convert(propertyExpression, typeof(DateTime?)), parameter))
                        .Must(value => value == null || value != default(DateTime))
                        .WithMessage($"{propertyName} must have a valid date.")
                        .Must(value => value == null || value <= DateTime.Now)
                        .WithMessage($"{propertyName} cannot be in the future.");
                }
                else if (propertyType == typeof(decimal))
                {
                    RuleFor(Expression.Lambda<Func<T, decimal?>>(Expression.Convert(propertyExpression, typeof(decimal?)), parameter))
                        .Must(value => value == null || value >= 0)
                        .WithMessage($"{propertyName} must be greater than or equal to 0.")
                        .Must(value => value == null || Math.Round(value.Value, 2) == value)
                        .WithMessage($"{propertyName} must have at most 2 decimal places.");
                }
                else if (propertyType == typeof(double))
                {
                    RuleFor(Expression.Lambda<Func<T, double?>>(Expression.Convert(propertyExpression, typeof(double?)), parameter))
                        .Must(value => value == null || value >= 0)
                        .WithMessage($"{propertyName} must be greater than or equal to 0.")
                        .Must(value => value == null || (value >= 0.1 && value <= 1000))
                        .WithMessage($"{propertyName} must be between 0.1 and 1000.");
                }
                else if (propertyType == typeof(Guid))
                {
                    RuleFor(Expression.Lambda<Func<T, Guid?>>(Expression.Convert(propertyExpression, typeof(Guid?)), parameter))
                        .Must(value => value == null || value != Guid.Empty)
                        .WithMessage($"{propertyName} must be a valid GUID.");
                }
                else if (propertyType.IsEnum)
                {
                    RuleFor(Expression.Lambda<Func<T, Enum>>(propertyExpression, parameter))
                        .Must(value => value == null || Enum.IsDefined(propertyType, value))
                        .WithMessage($"{propertyName} has an invalid enum value.");
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                {
                    RuleFor(Expression.Lambda<Func<T, IEnumerable>>(propertyExpression, parameter))
                        .Must(value => value != null && value.Cast<object>().Any())
                        .WithMessage($"{propertyName} must contain at least one element.");
                }
                else if (propertyType.IsClass && propertyType != typeof(string))
                {
                    RuleFor(Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof(object)), parameter))
                        .NotNull().WithMessage($"{propertyName} cannot be null.");
                    var complexValidator = new GenericValidator<object>();
                    RuleFor(Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof(object)), parameter))
                        .SetValidator(complexValidator);
                }
            }
        }
    }
    public class Validate
    {
        public static void ValidateRequest<TRequest>(TRequest entity)
        {
            var validator = new GenericValidator<TRequest>();
            var results = validator.Validate(entity);

            if (!results.IsValid)
            {
                var errors = results.Errors.GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Select(x => x.ErrorMessage).ToArray() ?? new string[0]
                    );
                throw new BadRequestException("Request Error", errors);
            }
        }

    }

}
