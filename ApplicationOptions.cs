using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification
{
    public class ApplicationOptions
    {
        public string AdminLogin { get; set; }
        public string AdminPassword { get; set; }
        public string Key { get; set; }
        public string LoginUrl { get; set; }
        public string GradesUrl { get; set; }
    }
}
