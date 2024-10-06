using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wallets.Types.Models
{
    public class CurrencyRates
    {
        [Key, Column(Order = 0)]
        public string Currency { get; set; } = "";

        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal Rate { get; set; }
    }

    public class DecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                decimal.TryParse(reader.GetString(), out decimal value))
            {
                return value;
            }
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}

