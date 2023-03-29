using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Strings;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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

        [Fact]
        public void Can_get_DeepLString_from_parameterless_member()
        {
            IParsedMember parsedMember;
            string text; 

            parsedMember = FakeParsedMember.BuildFor<ISampleInterface>(nameof(ISampleInterface.The_file_already_exists)
                , new CapitalWordToken("The")
                , SpaceToken.Simple
                , new WordToken("file")
                , SpaceToken.Simple
                , new WordToken("already")
                , SpaceToken.Simple
                , new WordToken("exists")
                , EndToken.End);

            text = parsedMember
                .AsDeepLString()
                .Merged();

            text.Should().Be("The file already exists");
        }

        [Fact]
        public void Can_get_DeepLString_from_member_with_parameters()
        {
            IParsedMember parsedMember;
            string text;
            ParameterInfo[] parameters;

            parameters = typeof(ISampleInterface)
                .GetMethod(nameof(ISampleInterface.There_are_0_apples))
                .GetParameters();
                

            parsedMember = FakeParsedMember
                .BuildFor<ISampleInterface>(nameof(ISampleInterface.There_are_0_apples)
                    , new CapitalWordToken("There")
                    , SpaceToken.Simple
                    , new WordToken("are")
                    , SpaceToken.Simple
                    , new ParameterToken(parameters[0])
                    , SpaceToken.Simple
                    , new WordToken("apples")
                    , EndToken.End);

            text = parsedMember
                .AsDeepLString()
                .Merged();

            text.Should().Be("There are {count} apples");
        }
    }
}