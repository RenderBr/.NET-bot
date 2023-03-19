using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI
{
    public interface IConfiguration
    {
        string ApiKey { get; set;  }
    }
}
