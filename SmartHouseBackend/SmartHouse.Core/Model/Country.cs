﻿namespace SmartHouse.Core.Model
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<City> Cities { get; set; } = new();
    }
}
