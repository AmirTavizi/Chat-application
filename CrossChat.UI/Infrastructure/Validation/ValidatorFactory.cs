using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossChat.UI.Infrastructure.Validation
{
    public class ValidatorFactory : IValidatorFactory
    {
        public List<AbstractValidator<T>> GetAllValidators<T>()
        {
            var validators = from t in typeof(ValidatorFactory).Assembly.GetTypes()
                           where typeof(AbstractValidator<T>).IsAssignableFrom(t)
                           select (AbstractValidator<T>)Activator.CreateInstance(t);

            return validators.ToList();
        }

        public AbstractValidator<T> GetDefaultValidator<T>()
        {
            var validators = from t in typeof(ValidatorFactory).Assembly.GetTypes()
                             where typeof(AbstractValidator<T>).IsAssignableFrom(t)
                             select (AbstractValidator<T>)Activator.CreateInstance(t);

            return validators.FirstOrDefault();
        }
    }
}
