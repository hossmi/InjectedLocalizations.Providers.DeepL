using System.Collections.Generic;
using System.Globalization;

namespace InjectedLocalizations.Configuration
{
    public interface IDeeplOptions
    {
        string ApiKey { get; set; }
        string Url { get; set; }
        IReadOnlyDictionary<CultureInfo, string> CultureMap { get; set; }
    }
}
