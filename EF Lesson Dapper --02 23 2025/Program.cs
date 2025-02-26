using System.Diagnostics;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;

var connectionString = "Data Source = ITPC6\\MSSQLSERVER01; Initial Catalog = Autopark; User ID=eldaniz; Password=eldaniz123; Trust Server Certificate = True";

AddCarWithOwner(connectionString, "Toyota", "Camry", 2020, 25000.50m, "John Doe");
AddCarWithOwner(connectionString, "BMW", "X5", 2019, 45000.99m, "Jane Smith");
AddCarWithOwner(connectionString, "Honda", "Civic", 2021, 20000.00m, "John Doe");

Console.WriteLine("=== All cars with owners ===");
GetCarsWithOwners(connectionString);

Console.WriteLine("\n=== John Doe's cars ===");
GetCarsByOwner(connectionString, "John Doe");

UpdateOwnerByCarId(connectionString, 1, "Michael Johnson");

DeleteCarWithOwner(connectionString, 2);

Console.WriteLine("\n=== Updated data after changes. ===");
GetCarsWithOwners(connectionString);



//Задание 2
static void AddCarWithOwner(string connectionString, string brand, string model, int year, decimal price, string ownerName)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string insertCar = "INSERT INTO Cars (Brand, Model, Year, Price) OUTPUT INSERTED.Id VALUES (@Brand, @Model, @Year, @Price)";
        int carId = connection.ExecuteScalar<int>(insertCar, new { Brand = brand, Model = model, Year = year, Price = price });

        string insertOwner = "INSERT INTO Owners (Name, CarId) VALUES (@Name, @CarId)";
        connection.Execute(insertOwner, new { Name = ownerName, CarId = carId });

        Console.WriteLine("Car and Owner added successfully!");
    }
}


//Задание 3
static void UpdateOwnerByCarId(string connectionString, int carId, string newOwnerName)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string updateOwner = "UPDATE Owners SET Name = @NewOwnerName WHERE CarId = @CarId";
        int rowsAffected = connection.Execute(updateOwner, new { NewOwnerName = newOwnerName, CarId = carId });

        if (rowsAffected > 0)
            Console.WriteLine($"Owner for Car ID {carId} updated successfully!");
        else
            Console.WriteLine($"No owner found for Car ID {carId}.");
    }
}

//Задание 4
static void DeleteCarWithOwner(string connectionString, int carId)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string deleteOwner = "DELETE FROM Owners WHERE CarId = @CarId";
        string deleteCar = "DELETE FROM Cars WHERE Id = @CarId";

        connection.Execute(deleteOwner, new { CarId = carId });
        int rowsAffected = connection.Execute(deleteCar, new { CarId = carId });

        if (rowsAffected > 0)
            Console.WriteLine($"Car and associated owner with ID {carId} deleted successfully!");
        else
            Console.WriteLine($"Car with ID {carId} not found.");
    }
}



//Задание 5
static List<CarWithOwner> GetCarsWithOwners(string connectionString)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string query = @"SELECT c.Id, c.Brand, c.Model, c.Year, c.Price, o.Name AS OwnerName 
                         FROM Cars c
                         INNER JOIN Owners o ON c.Id = o.CarId";

        return connection.Query<CarWithOwner>(query).ToList();
    }
}

//Задание 6
static List<CarWithOwner> GetCarsByOwner(string connectionString, string ownerName)
{
    using (var connection = new SqlConnection(connectionString))
    {
        string query = @"SELECT c.Id, c.Brand, c.Model, c.Year, c.Price, o.Name AS OwnerName 
                         FROM Cars c
                         INNER JOIN Owners o ON c.Id = o.CarId
                         WHERE o.Name = @OwnerName";

        var cars = connection.Query<CarWithOwner>(query, new { OwnerName = ownerName }).ToList();
        foreach (var car in cars)
        {
            Console.WriteLine($"{car.Brand} {car.Model} ({car.Year}) - {car.Price}$, Owner: {car.OwnerName}");
        }
        return cars;
    }
}




class CarWithOwner
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string OwnerName { get; set; }
}