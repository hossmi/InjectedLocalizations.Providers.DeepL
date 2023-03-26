using InjectedLocalizations.Configuration;
using InjectedLocalizations.Models;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations
{
    public class UseDeepLProviderTests
    {
        [Fact]
        public void Can_configure_DeepLProvider()
        {
            FakeLocalizationOptions mainOptions;

            mainOptions = new FakeLocalizationOptions();
            mainOptions.UseDeepLProvider(10, options =>
            {
                options.ApiKey = "test";
                options.Url = "test";
                options.CultureMap = new Dictionary<CultureInfo, string>
                {
                    { new CultureInfo("en"), "EN" },
                    { new CultureInfo("es"), "ES" },
                };
            });

            mainOptions.Services.Keys.Should().Contain(typeof(IDeeplLocalizatonsProviderConfiguration));
            mainOptions.Services.Keys.Should().Contain(typeof(IDeeplCultureMap));
            mainOptions.ProviderType.Should().Be(typeof(DeepLLocalizationsProvider));
            mainOptions.ProviderPriority.Should().Be(10);
        }
    }
}