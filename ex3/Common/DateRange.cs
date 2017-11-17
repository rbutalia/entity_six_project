using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class DateRange
    {
        public DateRange(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

    }
}
