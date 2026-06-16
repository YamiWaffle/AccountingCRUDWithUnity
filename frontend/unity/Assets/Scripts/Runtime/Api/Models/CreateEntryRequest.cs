using System;
using Newtonsoft.Json;

namespace AccountingApp.Api.Models
{
    [Serializable]
    public class CreateEntryRequest
    {
        [JsonProperty("amount")]
        public double Amount;
        
        [JsonProperty("category")]
        public string Category;
        
        [JsonProperty("note")]
        public string Note;
        
        [JsonProperty("date")]
        public string Date;
    }
}
