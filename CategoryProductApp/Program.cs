using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dbPath = "store.db";
string baseDir = Directory.GetCurrentDirectory();
string categoriesCsv = Path.Combine(baseDir, "Data", "categories.csv");
string productsCsv = Path.Combine(baseDir, "Data", "products.csv");

Console.WriteLine($"Базовая директория: {baseDir}");
Console.WriteLine($"Полный путь к categories.csv: {categoriesCsv}");
Console.WriteLine($"categories.csv существует: {File.Exists(categoriesCsv)}");
Console.WriteLine($"products.csv существует: {File.Exists(productsCsv)}");

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(categoriesCsv, productsCsv);

string choice;
do
{
    Console.Clear();
    Console.WriteLine("=== УПРАВЛЕНИЕ ТОВАРАМИ ===");
    Console.WriteLine("1 - Показать все категории");
    Console.WriteLine("2 - Показать все товары");
    Console.WriteLine("3 - Добавить товар");
    Console.WriteLine("4 - Редактировать товар");
    Console.WriteLine("5 - Удалить товар");
    Console.WriteLine("6 - Отчёты");
    Console.WriteLine("0 - Выход");
    Console.Write("Ваш выбор: ");

    choice = Console.ReadLine()?.Trim() ?? "";

    switch (choice)
    {
        case "1": ShowCategories(db); break;
        case "2": ShowProducts(db); break;
        case "3": AddProduct(db); break;
        case "4": EditProduct(db); break;
        case "5": DeleteProduct(db); break;
        case "6": ReportsMenu(db); break;
        case "0": Console.WriteLine("До свидания!"); break;
        default: Console.WriteLine("Неверный выбор!"); break;
    }

    if (choice != "0")
    {
        Console.WriteLine("\nНажмите любую клавишу...");
        Console.ReadKey();
    }
} while (choice != "0");

static void ShowCategories(DatabaseManager db)
{
    Console.WriteLine("\n--- Категории ---");
    var categories = db.GetAllCategories();
    Console.WriteLine($"Найдено категорий: {categories.Count}");
    foreach (var c in categories)
        Console.WriteLine(c);
    if (categories.Count == 0)
        Console.WriteLine("(Нет данных)");
}

static void ShowProducts(DatabaseManager db)
{
    Console.WriteLine("\n--- Товары ---");
    var products = db.GetAllProducts();
    Console.WriteLine($"Найдено товаров: {products.Count}");
    foreach (var p in products)
        Console.WriteLine(p);
    if (products.Count == 0)
        Console.WriteLine("(Нет данных)");
}

static void AddProduct(DatabaseManager db)
{
    Console.WriteLine("\n--- Добавление товара ---");
    foreach (var c in db.GetAllCategories()) Console.WriteLine(c);

    Console.Write("ID категории: ");
    if (!int.TryParse(Console.ReadLine(), out int catId)) { Console.WriteLine("Ошибка!"); return; }

    Console.Write("Название: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrEmpty(name)) { Console.WriteLine("Ошибка!"); return; }

    Console.Write("Цена: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal price)) { Console.WriteLine("Ошибка!"); return; }

    try { db.AddProduct(new Product(0, catId, name, price)); Console.WriteLine("Добавлено!"); }
    catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
}

static void EditProduct(DatabaseManager db)
{
    Console.Write("ID товара: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Ошибка!"); return; }

    var p = db.GetProductById(id);
    if (p == null) { Console.WriteLine("Не найден!"); return; }

    Console.WriteLine($"Текущий: {p}");
    Console.Write($"Название ({p.Name}): ");
    string name = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(name)) p.Name = name;

    Console.Write($"Цена ({p.Price}): ");
    string priceStr = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(priceStr) && decimal.TryParse(priceStr, out decimal price)) p.Price = price;

    db.UpdateProduct(p);
    Console.WriteLine("Обновлено!");
}

static void DeleteProduct(DatabaseManager db)
{
    Console.Write("ID товара: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Ошибка!"); return; }

    var p = db.GetProductById(id);
    if (p == null) { Console.WriteLine("Не найден!"); return; }

    Console.Write($"Удалить '{p.Name}'? (да/нет): ");
    if ((Console.ReadLine()?.Trim().ToLower() ?? "") == "да") { db.DeleteProduct(id); Console.WriteLine("Удалено!"); }
    else Console.WriteLine("Отменено.");
}

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.Clear();
        Console.WriteLine("--- ОТЧЁТЫ ---");
        Console.WriteLine("1 - Товары с категориями");
        Console.WriteLine("2 - Количество товаров по категориям");
        Console.WriteLine("3 - Средняя цена по категориям");
        Console.WriteLine("0 - Назад");
        Console.Write("Ваш выбор: ");

        choice = Console.ReadLine()?.Trim() ?? "";
        switch (choice)
        {
            case "1":
                new ReportBuilder(db)
                    .Query("SELECT p.product_name, c.category_name, p.price FROM products p JOIN categories c ON p.category_id = c.category_id ORDER BY p.product_name")
                    .Title("Товары с категориями")
                    .Header("Товар", "Категория", "Цена")
                    .ColumnWidths(35, 20, 10)
                    .Numbered()
                    .Print();
                break;
            case "2":
                new ReportBuilder(db)
                    .Query("SELECT c.category_name, COUNT(*) FROM products p JOIN categories c ON p.category_id = c.category_id GROUP BY c.category_name ORDER BY c.category_name")
                    .Title("Количество товаров")
                    .Header("Категория", "Кол-во")
                    .ColumnWidths(25, 10)
                    .Print();
                break;
            case "3":
                new ReportBuilder(db)
                    .Query("SELECT c.category_name, ROUND(AVG(p.price), 2) FROM products p JOIN categories c ON p.category_id = c.category_id GROUP BY c.category_name ORDER BY AVG(p.price) DESC")
                    .Title("Средняя цена")
                    .Header("Категория", "Ср. цена")
                    .ColumnWidths(25, 15)
                    .Print();
                break;
        }
        if (choice != "0") { Console.WriteLine("\nНажмите любую клавишу..."); Console.ReadKey(); }
    } while (choice != "0");
}