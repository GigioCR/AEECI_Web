using System;
using System.ComponentModel.DataAnnotations;

namespace AecciWeb.Models
{
    public class StoreStatus
    {
        [Key]
        public int Id { get; set; }

        // true = abierto, false = cerrado
        [Required]
        public bool IsOpen { get; set; }
    }
}