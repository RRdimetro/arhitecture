class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; }

    private decimal _price;

    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Цена не может быть отрицательной");
            _price = value;
        }
    }

    public Product(int id, int categoryId, string name, decimal price)
    {
        Id = id;
        CategoryId = categoryId;
        Name = name;
        Price = price;
    }

    public Product() : this(0, 0, "", 0) { }

    public override string ToString() => $"[{Id}] {Name}, цена: {Price:F2} руб.";
}