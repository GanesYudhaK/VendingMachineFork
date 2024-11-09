namespace VendingMachineApp.ModelViewModel;

public class UserViewModel
{
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public decimal? Balance { get; set; }

    // Menyimpan query pencarian
    public string? SearchString { get; set; }
    
    // Menyimpan daftar hasil pencarian
    public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
}
