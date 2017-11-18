using System;
using System.Collections.Generic;

namespace ex3.Domain.History
{
    [Schema("History")]
    public class Consumer //:IHistory
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public ICollection<OrderTotal> OrderTotals { get; set; }
    }
    [Schema("History")]
    public class OrderTotal //:IHistory
    {
        public Guid Key { get; set; }
        public decimal Total { get; set; }
        public Guid ConsumerId { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Schema : Attribute
    {
        public string SchemaName { get; set; }

        public Schema(string schemaName)
        {
            SchemaName = schemaName;
        }
    }
}
