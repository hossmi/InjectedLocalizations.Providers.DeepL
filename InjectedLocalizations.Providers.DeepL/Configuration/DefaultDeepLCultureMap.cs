using System.Collections.Generic;
using System.Globalization;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Configuration
{
    public class DefaultDeepLCultureMap : IDeeplCultureMap
    {
        private readonly IReadOnlyDictionary<CultureInfo, string> targetMaps;
        private readonly IReadOnlyDictionary<CultureInfo, string> sourceMaps;

        public DefaultDeepLCultureMap(IReadOnlyDictionary<CultureInfo, string> sourceMaps, IReadOnlyDictionary<CultureInfo, string> targetMaps)
        {
            this.sourceMaps = sourceMaps.ShouldBeNotNull(nameof(sourceMaps));
            this.targetMaps = targetMaps.ShouldBeNotNull(nameof(targetMaps));
        }

        public string MapAsSourceLanguage(CultureInfo culture)
        {
            if (this.sourceMaps.TryGetValue(culture, out string map))
                return map;

            return culture.TwoLetterISOLanguageName;
        }

        public string MapAsTargetLanguage(CultureInfo culture)
        {
            if (this.targetMaps.TryGetValue(culture, out string map))
                return map;

            return culture.TwoLetterISOLanguageName;
        }
    }
}
