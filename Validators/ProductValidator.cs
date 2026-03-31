using FluentValidation;
using ECommerceWebAPI.DTOs;

public class ProductValidator : AbstractValidator<ProductCreateDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be more than zero.");
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        //RuleFor(x => x.Description).NotEmpty().When(x => x.Price > 1000);
    }
}