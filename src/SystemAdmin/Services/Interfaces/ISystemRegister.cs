using Altinn.Platform.Authentication.Core.SystemRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAdmin.Services.Interfaces
{
    internal interface ISystemRegister
    {

        Task CreateSystem(SystemRegisterRequest request);
    }
}
