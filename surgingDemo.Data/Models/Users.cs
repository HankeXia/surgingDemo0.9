using System;
using System.Collections.Generic;

namespace surgingDemo.Data.Models
{
    public partial class Users
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
