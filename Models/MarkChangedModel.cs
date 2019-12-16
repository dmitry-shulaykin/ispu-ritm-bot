using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class MarkChangedModel
    {
        public string Student { get; set; }

        public int Semestr { get; set; }

        public string SubjectName { get; set; }

        public string Value { get; set; }

        public string PrevValue { get; set; }

        public string Type { get; set; }
    }
}
