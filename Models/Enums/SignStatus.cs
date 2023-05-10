using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models.Enums
{
    public enum SignStatus
    {
        MobileAuth = 1,
        EmailAuth,
        WrongUserPass,
        CodeExpire,
        WrongCode,
        NoUserFound,
        Success,
        UserExists,
        PassFormatNotMatched,
        UserNameExists,
        UserNotActive
    }
}
