namespace VendingMachineApp.Models;
using Microsoft.AspNetCore.Identity;

public class Account  : IdentityUser
{
    public string? FullName { get; set; }
}
