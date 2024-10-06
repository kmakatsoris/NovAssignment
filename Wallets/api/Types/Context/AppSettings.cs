using System.Runtime.Serialization;

namespace Wallets.Types.Context
{
    [DataContract]
    public class AppSettings
    {
        [DataMember(Name = "AllowedHosts")]
        public string AllowedHosts { get; set; } = "";

        [DataMember(Name = "ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
    }

    [DataContract]
    public class ConnectionStrings
    {
        [DataMember(Name = "MySqlConnection")]
        public string MySqlConnection { get; set; } = "";
    }
}