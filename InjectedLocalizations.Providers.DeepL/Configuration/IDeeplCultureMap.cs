using System.Globalization;

namespace InjectedLocalizations.Configuration
{
    public interface IDeeplCultureMap
    {
        string MapAsSourceLanguage(CultureInfo culture);
        string MapAsTargetLanguage(CultureInfo culture);
    }
}
