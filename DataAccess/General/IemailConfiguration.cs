﻿using Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public interface IemailConfiguration
    {
        Task<emailConfigurationModel> get(emailConfigurationModel emailConfiguration);
    }
}
