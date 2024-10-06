using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wallets.Types.Models
{
    public class RateLimiting
    {
        public string IP { get; set; } = "";
        public int NumberOfCalls { get; set; }
    }

}

