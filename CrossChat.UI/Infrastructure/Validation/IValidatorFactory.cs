using System.Collections.Generic;
using FluentValidation;

namespace CrossChat.UI.Infrastructure.Validation
{
    public interface IValidatorFactory
    {
        List<AbstractValidator<T>> GetAllValidators<T>();
        AbstractValidator<T> GetDefaultValidator<T>();
    }
}