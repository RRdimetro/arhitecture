using Microsoft.Data.Sqlite;
using System.Text;

class DatabaseManager
{
    private string _connectionString;

    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public void InitializeDatabase(string categoriesCsvPath, string productsCsvPath)
    {
        Console.WriteLine($"=== НАЧАЛО ИНИЦИАЛИЗАЦИИ ===");
        Console.WriteLine($"Путь к categories.csv: {categoriesCsvPath}");
        Console.WriteLine($"Файл существует: {File.Exists(categoriesCsvPath)}");
        Console.WriteLine($"Путь к products.csv: {productsCsvPath}");
        Console.WriteLine($"Файл существует: {File.Exists(productsCsvPath)}");

        CreateTables();

        // Очищаем таблицы перед загрузкой
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var clearCmd = conn.CreateCommand();
        clearCmd.CommandText = "DELETE FROM products; DELETE FROM categories;";
        int deleted = clearCmd.ExecuteNonQuery();
        Console.WriteLine($"Очищено записей: {deleted}");

        // Принудительно загружаем CSV
        if (File.Exists(categoriesCsvPath))
        {
            Console.WriteLine("Начинаю загрузку категорий...");
            int count = ImportCategoriesFromCsv(categoriesCsvPath);
            Console.WriteLine($"[OK] Загружено {count} категорий");
        }
        else
        {
            Console.WriteLine($"[ОШИБКА] Файл не найден: {categoriesCsvPath}");
        }

        if (File.Exists(productsCsvPath))
        {
            Console.WriteLine("Начинаю загрузку товаров...");
            int count = ImportProductsFromCsv(productsCsvPath);
            Console.WriteLine($"[OK] Загружено {count} товаров");
        }
        else
        {
            Console.WriteLine($"[ОШИБКА] Файл не найден: {productsCsvPath}");
        }

        // Проверяем, что загрузилось
        Console.WriteLine($"Проверка: в БД {GetAllCategories().Count} категорий");
        Console.WriteLine($"Проверка: в БД {GetAllProducts().Count} товаров");
        Console.WriteLine($"=== КОНЕЦ ИНИЦИАЛИЗАЦИИ ===");
    }

    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS categories (
                category_id INTEGER PRIMARY KEY AUTOINCREMENT,
                category_name TEXT NOT NULL
            );
            
            CREATE TABLE IF NOT EXISTS products (
                product_id INTEGER PRIMARY KEY AUTOINCREMENT,
                category_id INTEGER NOT NULL,
                product_name TEXT NOT NULL,
                price REAL NOT NULL,
                FOREIGN KEY (category_id) REFERENCES categories(category_id)
            );
        ";
        cmd.ExecuteNonQuery();
        Console.WriteLine("Таблицы созданы/проверены");
    }

    private int ImportCategoriesFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);
        Console.WriteLine($"Прочитано строк из CSV: {lines.Length}");

        int imported = 0;
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO categories (category_id, category_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
            imported++;
        }
        return imported;
    }

    private int ImportProductsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);
        Console.WriteLine($"Прочитано строк из CSV: {lines.Length}");

        int imported = 0;
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO products (product_id, category_id, product_name, price) VALUES (@id, @categoryId, @name, @price)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@categoryId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@price", decimal.Parse(parts[3]));
            cmd.ExecuteNonQuery();
            imported++;
        }
        return imported;
    }

    public List<Category> GetAllCategories()
    {
        var result = new List<Category>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT category_id, category_name FROM categories ORDER BY category_id";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Category(reader.GetInt32(0), reader.GetString(1)));
        }
        return result;
    }

    public List<Product> GetAllProducts()
    {
        var result = new List<Product>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT product_id, category_id, product_name, price FROM products ORDER BY product_id";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Product(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetDecimal(3)));
        }
        return result;
    }

    public Product GetProductById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT product_id, category_id, product_name, price FROM products WHERE product_id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Product(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetDecimal(3));
        }
        return null;
    }

    public void AddProduct(Product product)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO products (category_id, product_name, price) VALUES (@categoryId, @name, @price)";
        cmd.Parameters.AddWithValue("@categoryId", product.CategoryId);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.ExecuteNonQuery();
    }

    public void UpdateProduct(Product product)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE products SET category_id = @categoryId, product_name = @name, price = @price WHERE product_id = @id";
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@categoryId", product.CategoryId);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.ExecuteNonQuery();
    }

    public void DeleteProduct(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM products WHERE product_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        using var reader = cmd.ExecuteReader();

        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);

        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }

        return (columns, rows);
    }
}