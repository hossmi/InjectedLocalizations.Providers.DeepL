using System.Globalization;

namespace InjectedLocalizations.Configuration
{
    public interface IDeeplCultureMap
    {
        string MapToDeepl(CultureInfo culture);
    }
}
