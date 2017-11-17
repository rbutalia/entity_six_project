using System;
using System.Collections.Generic;
using System.Text;

namespace ex3.Domain
{
    public class Ninja
    {
        public Ninja(string name, MilitaryRole militaryRole)
        {
            Name = name;
            MilitaryRole = militaryRole;
        }

        public Ninja() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public NinjaType NinjaType { get; set; }
        public bool ServedInOniwaban { get; set; }
        public MilitaryRole MilitaryRole { get; set; }
        public Clan Clan { get; set; }
        public int? ClanId { get; set; }
        public List<Equipment> EquipmentOwned { get; set; }
        public List<Battle> Battles { get; set; }
        public Person TrueIdentity { get; set; }
    }

    public class NinjaFamily
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Ninja> Ninjas { get; set; }
    }


    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Ninja Ninja { get; set; }
    }
}
