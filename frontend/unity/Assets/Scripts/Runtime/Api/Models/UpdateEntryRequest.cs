using System;
using Newtonsoft.Json;

namespace AccountingApp.Api.Models
{
    [Serializable]
    public class UpdateEntryRequest
    {
        [JsonProperty("id")]
        public int Id;
        
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
