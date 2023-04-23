using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using InjectedLocalizations.Configuration;
using Xunit;

namespace InjectedLocalizations
{
    public class DefaultDeepLCultureMapTests
    {
        [Fact]
        public void Can_instance_DefaultDeepLCultureMap_with_empty_dictionary()
        {
            DefaultDeepLCultureMap cultureMap;
            string result;

            cultureMap = new DefaultDeepLCultureMap(new Dictionary<CultureInfo, string>(), new Dictionary<CultureInfo, string>());

            result = cultureMap.MapAsTargetLanguage(new CultureInfo("es-ES"));
            result.Should().Be("es");

            result = cultureMap.MapAsSourceLanguage(new CultureInfo("es-ES"));
            result.Should().Be("es");
        }
        [Fact]
        public void Can_instance_DefaultDeepLCultureMap_with_dictionary()
        {
            DefaultDeepLCultureMap cultureMap;
            string enResult, esResult, japResult;

            cultureMap = new DefaultDeepLCultureMap(new Dictionary<CultureInfo, string>
            {
                { new CultureInfo("en"), "EN_source" },
                { new CultureInfo("es-ES"), "ESP_source" },
            }, new Dictionary<CultureInfo, string>
            {
                { new CultureInfo("en"), "EN_target" },
                { new CultureInfo("es-ES"), "ESP_target" },
            });

            enResult = cultureMap.MapAsTargetLanguage(new CultureInfo("en"));
            esResult = cultureMap.MapAsTargetLanguage(new CultureInfo("es-ES"));
            japResult = cultureMap.MapAsTargetLanguage(new CultureInfo("ja-JP"));
            enResult.Should().Be("EN_target");
            esResult.Should().Be("ESP_target");
            japResult.Should().Be("ja");

            enResult = cultureMap.MapAsSourceLanguage(new CultureInfo("en"));
            esResult = cultureMap.MapAsSourceLanguage(new CultureInfo("es-ES"));
            japResult = cultureMap.MapAsSourceLanguage(new CultureInfo("ja-JP"));
            enResult.Should().Be("EN_source");
            esResult.Should().Be("ESP_source");
            japResult.Should().Be("ja");
        }
    }
}