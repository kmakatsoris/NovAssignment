using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wallets.Types.Models
{
    public class Wallet
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }
    }
}