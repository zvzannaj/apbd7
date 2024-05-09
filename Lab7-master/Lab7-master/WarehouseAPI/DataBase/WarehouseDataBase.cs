using Microsoft.Data.SqlClient;
using WarehouseAPI.Model;


namespace WarehouseAPI.DataBase;

public class WarehouseDataBase : IWarehouseDataBase
{
    private readonly IConfiguration _configuration;

    public WarehouseDataBase(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    private SqlConnection GetConnection()
    {
        SqlConnection connection = new(_configuration.GetConnectionString("Default"));
        connection.Open();
        return connection;
    }

    public async Task<Product> FindProductById(int requestIdProduct)
    {
        using (SqlConnection connection = GetConnection())
        {
            string query = "SELECT * FROM Product WHERE IdProduct = @IdProduct";
            using (SqlCommand command = new(query, connection))
            {
                command.Parameters.AddWithValue("@IdProduct", requestIdProduct);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Product
                    {
                        IdProduct = Convert.ToInt32(reader["IdProduct"]),
                        Name = Convert.ToString(reader["Name"]),
                        Description = Convert.ToString(reader["Description"]),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                }
            }
        }
        return null;
    }

    public async Task<Warehouse> FindWarehouseById(int requestIdWarehouse)
    {
        using (SqlConnection connection = GetConnection())
        {
            string query = "SELECT * FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            using (SqlCommand command = new(query, connection))
            {
                command.Parameters.AddWithValue("@IdWarehouse", requestIdWarehouse);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Warehouse
                    {
                        IdWarehouse = Convert.ToInt32(reader["IdWarehouse"]),
                        Name = Convert.ToString(reader["Name"]),
                        Address = Convert.ToString(reader["Address"])
                    };
                }
            }
        }
        return null;
    }

    public async Task<Order> FindOrderByProductIdAndAmount(int requestIdProduct, int requestAmount)
    {
        using (SqlConnection connection = GetConnection())
        {
            string query = "SELECT * FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount";
            using (SqlCommand command = new(query, connection))
            {
                command.Parameters.AddWithValue("@IdProduct", requestIdProduct);
                command.Parameters.AddWithValue("@Amount", requestAmount);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Order
                    {
                        IdOrder = Convert.ToInt32(reader["IdOrder"]),
                        IdProduct = Convert.ToInt32(reader["IdProduct"]),
                        Amount = Convert.ToInt32(reader["Amount"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        FulfilledAt = reader["FulfilledAt"] != DBNull.Value ? Convert.ToDateTime(reader["FulfilledAt"]) : (DateTime?)null
                    };
                }
            }
        }
        return null;
    }

    public async void AddProductToWarehouse(Product_Warehouse productWarehouse)
    {
        using (SqlConnection connection = GetConnection())
        {
            string query = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                                VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);
                                SELECT SCOPE_IDENTITY();";
            using (SqlCommand command = new(query, connection))
            {
                command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
                command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
                command.Parameters.AddWithValue("@IdOrder", productWarehouse.IdOrder);
                command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
                command.Parameters.AddWithValue("@Price", productWarehouse.Price);
                command.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
                object result = await command.ExecuteScalarAsync();
                int generatedId = Convert.ToInt32(result);
            }
        }
    }

    public void AddProductToWarehouseWithStoredProcedure(Product_Warehouse productWarehouse)
    {

    }
}