﻿using Models.common;
using Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public interface IemailService
    {
        Task<genericResponse> SendEMailAsync(emailServiceModel mailRequest);
    }
}
