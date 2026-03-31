using FluentValidation;
using ECommerceWebAPI.DTOs;

public class OrderRequestValidator : AbstractValidator<OrderRequestDto>
{
    public OrderRequestValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("A valid Customer ID is required.");
        
        RuleFor(x => x.Items).NotEmpty().WithMessage("An order must have at least one item.");

        RuleForEach(x => x.Items).ChildRules(item => {
            item.RuleFor(i => i.ProductId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).InclusiveBetween(1, 100).WithMessage("Quantity must be between 1 and 100.");
        });
    }
}