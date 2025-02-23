using Microsoft.Data.SqlClient;
using Dapper;

var connectionString = "Data Source = localhost; Initial Catalog = Autopark; Trust Server Certificate = True; Integrated Security = True";

AddCar(connectionString, "BMW", "F90", 2024, 40000);
AddCar(connectionString, "Audi", "A6", 2023, 38000);
UpdateCarPrice(connectionString, 1, 45000);
DeleteCar(connectionString, 2);
ShowAllCars(connectionString);
GetCarsByBrand(connectionString, "BMW");

//Задание 1
static void AddCar(string connectionString, string brand, string model, int year, decimal price)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string sql = "INSERT INTO Cars (Brand, Model, Year, Price) VALUES (@Brand, @Model, @Year, @Price)";
        
        var parameters = new { Brand = brand, Model = model, Year = year, Price = price };
        
        connection.Execute(sql, parameters);
        Console.WriteLine("Car added!");
    }
}

//Задание 2
static void UpdateCarPrice(string connectionString, int carId, decimal newPrice)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string sql = "UPDATE Cars SET Price = @NewPrice WHERE Id = @CarId";
        var parameters = new { NewPrice = newPrice, CarId = carId };
        
        int rowsAffected = connection.Execute(sql, parameters);

        if (rowsAffected > 0)
        {
            Console.WriteLine($"Car with ID {carId} updated successfully!");
        }
        else
        {
            Console.WriteLine($"Car with ID {carId} does not exist!");
        }
    }
}

//Задание 3
static void DeleteCar(string connectionString, int carId)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string sql = "DELETE FROM Cars WHERE Id = @CarId";
        var parameters = new { CarId = carId };
        
        int rowsAffected = connection.Execute(sql, parameters);
        
        if (rowsAffected > 0)
            Console.WriteLine($"Car with ID {carId} deleted successfully!");
        else
            Console.WriteLine($"Car with ID {carId} not found.");
    }
}

//Задание 4
static void ShowAllCars(string connectionString)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string sql = "SELECT * FROM Cars";
        var cars = connection.Query<Car>(sql);

        Console.WriteLine("List of cars:");
        foreach (var car in cars)
        {
            Console.WriteLine($"{car.Id}: {car.Brand} {car.Model}, {car.Year}, ${car.Price}");
        }
    }
}

//Задание 5
static void GetCarsByBrand(string connectionString, string brandName)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string sql = "SELECT * FROM Cars WHERE Brand = @BrandName";
        
        var cars = connection.Query<Car>(sql, new { BrandName = brandName }).ToList();
        
        if (cars.Count > 0)
        {
            Console.WriteLine($"Cars of brand {brandName}:");
            foreach (var car in cars)
            {
                Console.WriteLine($"{car.Id}: {car.Brand} {car.Model}, {car.Year}, ${car.Price}");
            }
        }
        else
        {
            Console.WriteLine($"No cars found for brand {brandName}.");
        }
    }
}


class Car
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
}