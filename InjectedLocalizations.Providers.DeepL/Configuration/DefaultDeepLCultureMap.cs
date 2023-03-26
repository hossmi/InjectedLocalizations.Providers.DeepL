using System.Collections.Generic;
using System.Globalization;

namespace InjectedLocalizations.Configuration
{
    public class DefaultDeepLCultureMap : IDeeplCultureMap
    {
        private readonly IReadOnlyDictionary<CultureInfo, string> map;

        public DefaultDeepLCultureMap(IReadOnlyDictionary<CultureInfo, string> map)
        {
            this.map = map;
        }

        public string MapToDeepl(CultureInfo culture)
        {
            if (this.map.TryGetValue(culture, out string map))
                return map;

            return culture.TwoLetterISOLanguageName;
        }
    }
}
