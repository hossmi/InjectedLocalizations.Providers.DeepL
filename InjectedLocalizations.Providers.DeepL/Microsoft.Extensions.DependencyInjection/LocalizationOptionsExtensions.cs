using System;
using System.Collections.Generic;
using System.Globalization;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Strings;
using JimenaTools.Extensions.Validations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalizationOptionsExtensions
    {
        public static ILocalizationOptions UseDeepLProvider(this ILocalizationOptions options
            , int priority
            , Action<IDeeplOptions> deeplOptions)
        {
            PrvConfiguration configuration;
            IDeeplCultureMap cultureMap; 

            configuration = new PrvConfiguration();
            deeplOptions(configuration);

            if (configuration.ApiKey.IsNullEmptyOrWhiteSpace())
                throw new DeepLConfigurationException(nameof(configuration.ApiKey));

            if(configuration.Url.IsNullEmptyOrWhiteSpace())
                throw new DeepLConfigurationException(nameof(configuration.Url));

            cultureMap = new PrvCultureMap();

            options.AddService(typeof(IDeeplLocalizatonsProviderConfiguration), configuration);
            options.AddService(typeof(IDeeplCultureMap), cultureMap);
            options.SetProvider<DeepLLocalizationsProvider>(priority);

            return options;
        }

        private class PrvConfiguration : IDeeplLocalizatonsProviderConfiguration, IDeeplOptions
        {
            public string ApiKey { get; set; }
            public string Url { get; set; }
        }

        private class PrvCultureMap : IDeeplCultureMap
        {
            private readonly IReadOnlyDictionary<CultureInfo, string> cultureToDeeplMap;

            public PrvCultureMap()
            {
                this.cultureToDeeplMap = new Dictionary<CultureInfo, string>
                {
                    { new CultureInfo("en"), "EN" },
                    { new CultureInfo("es"), "ES" },
                    { new CultureInfo("pt"), "PT-PT" },
                };
            }

            public string MapToDeepl(CultureInfo culture)
            {
                if(this.cultureToDeeplMap.TryGetValue(culture, out string map))
                    return map;

                return culture.TwoLetterISOLanguageName;
            }
        }
    }
}
