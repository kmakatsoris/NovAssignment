using System.Runtime.Serialization;

namespace Wallets.Types.DTOs
{
    [DataContract]
    public class BalanceResponse
    {
        [DataMember(Name = "Success")]
        public bool Success { get; set; } = false;

        [DataMember(Name = "Balance")]
        public decimal Balance { get; set; }
    }
}
