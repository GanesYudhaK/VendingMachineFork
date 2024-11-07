using System;
using System.ComponentModel.DataAnnotations;

namespace VendingMachineApp.Models;

public class Transaction
{
    [Key]
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public int? UserId { get; set; }
        public int? IdProduct { get; set; }
        public DateTime Date { get; set; }
        public decimal? BalanceAfterTransaction { get; set; }
        public string? TransactionType { get; set; }

        public User? User { get; set; }
        public Product? Product { get; set; }
}
