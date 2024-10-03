using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altinn.SystemAdmin.Maskinporten.Models
{
    public class TokenRequestException : ApplicationException
    {
        public TokenRequestException(string message)
            : base(message)
        {
        }
    }
}
