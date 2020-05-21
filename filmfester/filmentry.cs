using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace filmfester
{
    class filmentries
    {
        [JsonProperty("entries")]
        public string[] entries { get; set; }

        public filmentries()
        {
        }
    }
}
