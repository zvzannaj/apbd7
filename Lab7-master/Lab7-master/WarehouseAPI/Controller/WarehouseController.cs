using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DataBase;
using WarehouseAPI.Model;
using WarehouseAPI.Model.DTOs;

namespace WarehouseAPI.Controller;

[ApiController]
[Route("/api/warehouse")]
public class WarehouseController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly IWarehouseDataBase _warehouseDataBase;


    public WarehouseController(IWarehouseDataBase warehouseDataBase)
    {
        _warehouseDataBase = warehouseDataBase;
    }


    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouseRequest request)
    {
        // Sprawdzanie czy ilość jest większa niż 0
        if (request.Amount <= 0)
        {
            return BadRequest("Amount must be greater than 0");
        }

        // Sprawdzanie czy produkt o podanym IdProduct istnieje
        Product product = await _warehouseDataBase.FindProductById(request.IdProduct);
        if (product == null)
        {
            return BadRequest("Product not found");
        }

        // Sprawdzanie czy magazyn o podanym IdWarehouse istnieje
        Warehouse warehouse = await _warehouseDataBase.FindWarehouseById(request.IdWarehouse);
        if (warehouse == null)
        {
            return BadRequest("Warehouse not found");
        }


        // Sprawdzanie czy istnieje zamówienie zakupu produktu
        Order order = await _warehouseDataBase.FindOrderByProductIdAndAmount(request.IdProduct, request.Amount);
        if (order == null)
        {
            return BadRequest("Order not found");
        }

        // Sprawdzanie czy zamówienie nie zostało zrealizowane
        if (order.FulfilledAt != null)
        {
            return BadRequest("Order already fulfilled");
        }

        // Aktualizacja kolumny FullfilledAt zamówienia na aktualną datę i godzinę
        order.FulfilledAt = DateTime.Now;


        // Wstawienie rekordu do tabeli Product_Warehouse
        Product_Warehouse productWarehouse = new Product_Warehouse
        {
            IdWarehouse = request.IdWarehouse,
            IdProduct = request.IdProduct,
            IdOrder = order.IdOrder,
            Amount = request.Amount,
            Price = product.Price * request.Amount,
            CreatedAt = DateTime.Now
        };

            _warehouseDataBase.AddProductToWarehouseWithStoredProcedure(productWarehouse);
            return Ok(productWarehouse.IdWarehouse);
    }

    [HttpPost("storedProcedure")]
    public async Task<IActionResult> AddProductToWarehouseWithStoredProcedure([FromBody] ProductWarehouseRequest request)
    {
        throw new NotImplementedException();
    }



}