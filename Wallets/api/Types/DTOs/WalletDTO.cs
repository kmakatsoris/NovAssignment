using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Wallets.Types.Models
{
    [DataContract]
    public class WalletDTO
    {
        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "Balance")]
        public decimal Balance { get; set; }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; } = "";
    }
}