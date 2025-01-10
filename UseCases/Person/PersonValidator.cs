using Daylon.Crud.Api.Model;
using Daylon.Crud.Api.Request;
using FluentValidation;

namespace Daylon.Crud.Api.UseCases.Person
{
    public class PersonValidator : AbstractValidator<RequestRegisterPersonJson>
    {

        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").
                MaximumLength(70).WithMessage("Name must be less than 70 characters");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required").
                MaximumLength(100).WithMessage("Name must be less than 100 characters");

            RuleFor(x => x.Age).NotEmpty().WithMessage("Age is required");
        }
    }
}
