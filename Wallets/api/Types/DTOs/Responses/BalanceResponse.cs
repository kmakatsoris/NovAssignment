using System.Runtime.Serialization;

namespace Wallets.Types.DTOs
{
    [DataContract]
    public class BalanceResponse
    {
        [DataMember(Name = "Success")]
        public BasicResponse Success { get; set; } = new BasicResponse();

        [DataMember(Name = "Balance")]
        public decimal Balance { get; set; }
    }
}
