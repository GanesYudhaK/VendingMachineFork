namespace VendingMachineApp.ModelViewModel;

public class ProductViewModels
{
    public int IdProduct { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public string? SearchString { get; set; }
    public List<ProductViewModels> Products { get; set; } = new List<ProductViewModels>();
}
