using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflowTagsApi.BLL.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public double PercentCount { get; set; }
    }
}
