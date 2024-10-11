using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NLog;
using AutoMapper;
using Polly;
using FluentValidation;
using FluentValidation.Results; 
using System.ComponentModel.DataAnnotations; 

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

class Program
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
        // 1. Робота з Newtonsoft.Json - Серіалізація і десеріалізація
        var product = new Product { Id = 1, Name = "Laptop", Price = 1200.50m };
        string json = JsonConvert.SerializeObject(product);
        Console.WriteLine($"Serialized JSON: {json}");

        var deserializedProduct = JsonConvert.DeserializeObject<Product>(json);
        Console.WriteLine($"Deserialized Product: {deserializedProduct.Name} - {deserializedProduct.Price}");

        logger.Info("JSON serialization and deserialization complete");

        // 2. Робота з FluentValidation - Валідація об'єктів
        var validator = new ProductValidator();
        FluentValidation.Results.ValidationResult results = validator.Validate(product);

        if (results.IsValid)
        {
            Console.WriteLine("Product is valid!");
        }
        else
        {
            foreach (var failure in results.Errors)
            {
                Console.WriteLine($"Validation failed: {failure.PropertyName} - {failure.ErrorMessage}");
                logger.Error($"Validation failed: {failure.PropertyName} - {failure.ErrorMessage}");
            }
        }

        // 3. Робота з NLog - Логування
        logger.Info("Application started");
        logger.Warn("This is a warning message");
        logger.Error("This is an error message");

        // 4. Робота з AutoMapper - Маппінг об'єктів
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDto>());
        var mapper = config.CreateMapper();

        var productDto = mapper.Map<ProductDto>(product);
        Console.WriteLine($"Mapped DTO: {productDto.Name} - {productDto.Price}");

        logger.Info("AutoMapper mapping complete");

        // 5. Робота з Polly - Повторні спроби при помилках
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(3, (exception, retryCount) =>
            {
                Console.WriteLine($"Retry {retryCount} due to: {exception.Message}");
                logger.Warn($"Retry {retryCount} due to: {exception.Message}");
            });

        try
        {
            retryPolicy.Execute(() =>
            {
                Console.WriteLine("Trying operation...");
                throw new Exception("Operation failed!");
            });
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Operation failed after retries");
        }

        logger.Info("Application finished");
    }
}
