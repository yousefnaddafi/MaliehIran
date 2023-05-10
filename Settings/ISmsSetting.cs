using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Settings
{
    public interface ISmsSetting
    {
        string UserApiKey { get; set; }
        string SecretKey { get; set; }
        string LineNumber { get; set; }
    }

    public class SmsSetting : ISmsSetting
    {
        public string UserApiKey { get; set; }
        public string SecretKey { get; set; }
        public string LineNumber { get; set; }
    }
}
