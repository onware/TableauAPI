using System.Collections.Generic;

namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// Different versions of server have different URL formats
    /// </summary>
    public enum ServerVersion
    {
        /// <summary>
        /// Tableau Server 9.x
        /// </summary>
        Server8_3,
        Server9,
        Server9_1,
        Server9_2,
        Server9_3,
        Server10_0,
        Server10_1,
        Server10_2,
        Server10_3,
        Server10_4,
        Server10_5,
        Server2018_1,
        Server2018_2,
        Server2018_3,
        Server2019_1,
        Server2019_2,
        Server2019_3,
        Server2019_4,
        Server2020_1,
        Server2020_2,
        Server2020_3,
        Server2020_4,
        Server2021_1
    }

    public class ServerVersionLookup
    {
        public static string APIVersion(ServerVersion version )
        {
            var mappingA = new Dictionary<ServerVersion, string>()
    {
        { ServerVersion.Server8_3, "2.0" },
        { ServerVersion.Server9, "2.0" },
        { ServerVersion.Server9_1, "2.0" },
        { ServerVersion.Server9_2, "2.1" },
        { ServerVersion.Server9_3, "2.2" },
        { ServerVersion.Server10_0, "2.3" },
        { ServerVersion.Server10_1, "2.4" },
        { ServerVersion.Server10_2, "2.5" },
        { ServerVersion.Server10_3, "2.6" },
        { ServerVersion.Server10_4, "2.7" },
        { ServerVersion.Server10_5, "2.8" },
        { ServerVersion.Server2018_1, "3.0" },
        { ServerVersion.Server2018_2, "3.1" },
        { ServerVersion.Server2018_3, "3.2" },
        { ServerVersion.Server2019_1, "3.3" },
        { ServerVersion.Server2019_2, "3.4" },
        { ServerVersion.Server2019_3, "3.5" },
        { ServerVersion.Server2019_4, "3.6" },
        { ServerVersion.Server2020_1, "3.7" },
        { ServerVersion.Server2020_2, "3.8" },
        { ServerVersion.Server2020_3, "3.9" },
        { ServerVersion.Server2020_4, "3.10" },
        { ServerVersion.Server2021_1, "3.11" }
    };

            return mappingA[version];

        }
    }
}
