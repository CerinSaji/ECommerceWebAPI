using FluentValidation;
using ECommerceWebAPI.DTOs;

public class OrderRequestValidator : AbstractValidator<OrderRequestDto>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("A valid Customer ID is required.");
        
        RuleFor(x => x.Items).NotEmpty().WithMessage("An order must have at least one item.");

        RuleForEach(x => x.Items).ChildRules(item => {
            item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("A valid Product ID is required.");
            item.RuleFor(i => i.Quantity).InclusiveBetween(1, 100).WithMessage("Quantity must be between 1 and 100.");
        });
    }
}