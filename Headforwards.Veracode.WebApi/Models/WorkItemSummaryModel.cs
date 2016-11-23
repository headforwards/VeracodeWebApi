using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Headforwards.Axa.Ppp.WebApi.Models
{
    [DataContract()]
    public class WorkItemsSummaryModel
    {
        [DataMember()]
        public double Max { get; set; }

        [DataMember()]
        public double Min { get; set; }

        [DataMember()]
        public double Average { get; set; }

        [DataMember()]
        public double Total { get; set; }

        [DataMember()]
        public string Type { get; set; }

        [DataMember()]
        public int Count { get; set; }
    }
}