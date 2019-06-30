using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridClientApplication.AuthRequirements
{
    public class SmithInRequirement:IAuthorizationRequirement
    {
        public SmithInRequirement()
        {

        }
    }
}
