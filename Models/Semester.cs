using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class Semester
    {
        public int Number { get; set; }

        public List<Subject> Subjects {get; set;}
    }
}
