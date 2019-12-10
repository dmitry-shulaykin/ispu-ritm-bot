using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string StudentsCollectionName { get; set; }
    }
}
