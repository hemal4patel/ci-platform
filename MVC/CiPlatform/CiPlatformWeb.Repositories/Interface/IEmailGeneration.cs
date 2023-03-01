﻿using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IEmailGeneration
    {
        
        User CheckUser (ForgotPasswordValidation obj);
        void GenerateEmail (ForgotPasswordValidation obj);

    }
}
