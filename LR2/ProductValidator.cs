using FluentValidation;

// Клас для валідації продуктів
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        // Назва продукту не повинна бути порожньою і має бути довшою за 3 символи
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Product name cannot be empty")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters long");

        // Ціна продукту повинна бути більшою за 0
        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
