using System.Runtime.Serialization;

namespace Wallets.Types.DTOs
{
    [DataContract]
    public class BasicResponse
    {
        [DataMember(Name = "Success")]
        public bool Success { get; set; } = false;

        [DataMember(Name = "Message")]
        public string Message { get; set; } = "";
    }
}
