using Core.Repositories;
using Domain.Repositories;
using FluentValidation;
using Shared.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Validations
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator(IUserRepository _userRepository)
        {
            RuleFor(c => c.Name)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("The Name is required");

            RuleFor(c => c.Name)
                .Must(name => name.Length >= 6)
                .WithMessage("The name must be at least 6 characters");

            RuleFor(u => u.Name)
             .Must(name => !_userRepository.ContainsAnotherUserWithSameName(name))
             .WithMessage("A User with the same Name already exists");
        }
    }
}
