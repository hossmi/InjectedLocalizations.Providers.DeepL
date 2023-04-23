using System.Globalization;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Models
{
    public class FakeCurrentCultureProvider : ICurrentCultureProvider
    {
        public CultureInfo CurrentCulture { get; set; }

        public bool TryGetCurrent(out CultureInfo culture)
        {
            culture = this.CurrentCulture;
            return culture != null;
        }
    }
}