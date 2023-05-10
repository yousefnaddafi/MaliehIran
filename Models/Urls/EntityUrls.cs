using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models.Urls
{
    public class EntityUrls
    {
        public static string MediaBaseUrl = "wwwroot/Media/Gallery/";
        public static string ProfileMediaUrl { get; set; } = MediaBaseUrl + "Profile";
        public static string Group { get; set; } = MediaBaseUrl + "Group";
        public static string DMMediaUrl { get; set; } = MediaBaseUrl + "ChatMedia";
    }
}
