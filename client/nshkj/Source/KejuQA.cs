using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace nshkj
{
    public class KejuQA
    {
        [JsonProperty("topic")]
        public string Topic
        {
            get;
            set;
        }

        [JsonProperty("options")]
        public List<string> Options
        {
            get;
            set;
        }

        [JsonIgnore()]
        public int Score
        {
            get;
            set;
        }

        [JsonIgnore()]
        public string Answer
        {
            get;
            set;
        }
    }
}
