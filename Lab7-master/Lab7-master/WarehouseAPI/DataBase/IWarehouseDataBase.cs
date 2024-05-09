using WarehouseAPI.Model;
using WarehouseAPI.Model.DTOs;

namespace WarehouseAPI.DataBase;

public interface IWarehouseDataBase
{
    Task<Product> FindProductById(int requestIdProduct);
    Task<Warehouse> FindWarehouseById(int requestIdWarehouse);
    Task<Order> FindOrderByProductIdAndAmount(int requestIdProduct, int requestAmount);
    void AddProductToWarehouse(Product_Warehouse productWarehouse);

    void AddProductToWarehouseWithStoredProcedure(Product_Warehouse productWarehouse);
}