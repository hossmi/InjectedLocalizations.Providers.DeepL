using System;
using System.Collections.Generic;
using System.Globalization;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Strings;

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

            configuration.SourceCultureMap ??= new Dictionary<CultureInfo, string>();
            configuration.TargetCultureMap ??= BuildDefaultTargetCultureMapValues().ToDictionary();
            options.AddService(typeof(IDeeplLocalizatonsProviderConfiguration), configuration);

            cultureMap = new DefaultDeepLCultureMap(configuration.SourceCultureMap, configuration.TargetCultureMap);
            options.AddService(typeof(IDeeplCultureMap), cultureMap);
            
            options.SetProvider<DeepLLocalizationsProvider>(priority);

            return options;
        }

        public static IEnumerable<KeyValuePair<CultureInfo, string>> BuildDefaultTargetCultureMapValues()
        {
            yield return new KeyValuePair<CultureInfo, string>(new CultureInfo("en-US"), "EN-US");
            yield return new KeyValuePair<CultureInfo, string>(new CultureInfo("en-GB"), "EN-GB");
            yield return new KeyValuePair<CultureInfo, string>(new CultureInfo("es-ES"), "ES");
        }

        private class PrvConfiguration : IDeeplLocalizatonsProviderConfiguration, IDeeplOptions
        {
            public string ApiKey { get; set; }
            public string Url { get; set; }
            public IReadOnlyDictionary<CultureInfo, string> TargetCultureMap { get; set; }
            public IReadOnlyDictionary<CultureInfo, string> SourceCultureMap { get; set; }
        }
    }
}
