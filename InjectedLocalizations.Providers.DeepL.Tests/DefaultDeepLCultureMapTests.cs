using FluentAssertions;
using InjectedLocalizations.Configuration;
using System.Collections.Generic;
using System.Globalization;
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

            cultureMap = new DefaultDeepLCultureMap(new Dictionary<CultureInfo, string>());

            result = cultureMap.MapToDeepl(new CultureInfo("es-ES"));

            result.Should().Be("es");
        }
        [Fact]
        public void Can_instance_DefaultDeepLCultureMap_with_dictionary()
        {
            DefaultDeepLCultureMap cultureMap;
            string enResult, esResult, japResult;

            cultureMap = new DefaultDeepLCultureMap(new Dictionary<CultureInfo, string>
            {
                { new CultureInfo("en"), "EN1" },
                { new CultureInfo("es-ES"), "ESP" },
            });

            enResult = cultureMap.MapToDeepl(new CultureInfo("en"));
            esResult = cultureMap.MapToDeepl(new CultureInfo("es-ES"));
            japResult = cultureMap.MapToDeepl(new CultureInfo("ja-JP"));

            enResult.Should().Be("EN1");
            esResult.Should().Be("ESP");
            japResult.Should().Be("ja");
        }
    }
}