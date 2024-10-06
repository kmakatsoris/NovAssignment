using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wallets.Types.Models
{
    public class CurrencyRates
    {
        [Key, Column(Order = 0)]
        public string Currency { get; set; } = "";

        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }

        public decimal Rate { get; set; }
    }
}

