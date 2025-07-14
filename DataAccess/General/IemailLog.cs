using Models.common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public interface IemailLog
    {
        Task<genericResponse> add(emailLogModel emailLog);
    }
}
