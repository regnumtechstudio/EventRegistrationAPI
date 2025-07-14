using Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public interface IeventMaster
    {
        Task<eventMaster> eventGet(eventMaster eventmaster);
        Task<IList<eventMaster>> eventGetList(eventMaster eventmaster);
    }
}
