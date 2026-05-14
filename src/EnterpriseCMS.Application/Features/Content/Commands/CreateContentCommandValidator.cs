using FluentValidation;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class CreateContentCommandValidator : AbstractValidator<CreateContentCommand>
{
    public CreateContentCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Blocks).NotNull();
    }
}
