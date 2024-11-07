using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VendingMachineApp.Models;

public class User
{
    [Key]
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public decimal? Balance { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}