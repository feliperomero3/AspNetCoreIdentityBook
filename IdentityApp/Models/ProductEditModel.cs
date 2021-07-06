using System;
using System.ComponentModel.DataAnnotations;
using IdentityApp.Entities;

namespace IdentityApp.Models
{
    public class ProductEditModel
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(typeof(decimal), "1", "9999999999999", ErrorMessage = "Price must be at least 1.")]
        public decimal Price { get; set; }

        public Product MapToProduct()
        {
            return new Product(Name, Description, Category, Price);
        }

        public static ProductEditModel MapFromProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return new ProductEditModel()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Price = product.Price
            };
        }
    }
}
