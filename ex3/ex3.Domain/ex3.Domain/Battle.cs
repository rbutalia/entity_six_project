
using Common;
using System.Collections.Generic;

namespace ex3.Domain
{
    public class Battle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateRange StartEndDates { get; set; }
        public List<Ninja> Ninjas { get; set; }
    }
}
