﻿namespace SportsStore.Entities
{
    public class Product
    {
        private Product()
        {
        }

        public Product(string name, string description, string category, decimal price) : this()
        {
            Name = name;
            Category = category;
            Description = description;
            Price = price;
        }

        public long ProductId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public decimal Price { get; private set; }
    }
}
