class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Category(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Category() : this(0, "") { }

    public override string ToString() => $"[{Id}] {Name}";
}