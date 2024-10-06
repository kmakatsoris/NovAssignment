using System.Runtime.Serialization;
using Wallets.Types.Enumerations;

namespace Wallets.Types.DTOs
{
    [DataContract]
    public class CurrencyRateDTO
    {
        [DataMember(Name = "Currency")]
        public CurrencyEnum Currency { get; set; }

        [DataMember(Name = "Rate")]
        public decimal Rate { get; set; }

        [DataMember(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
