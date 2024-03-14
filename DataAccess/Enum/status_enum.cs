using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Enum
{
    public enum StatusEnum
    {
        Unassigned = 1,
        Accepted = 2,
        Cancelled = 3,
        MDEnRoute = 4, //Active
        MDOnSite = 5, //Active
        Conclude = 6,
        CancelledByPatient = 7,
        Closed = 8, //Toclose
        Unpaid = 9,
        Clear = 10
    }
}