using System.Threading;
using DeepL;
using DeepL.Model;
using NSubstitute;
using Xunit;
using InjectedLocalizations.Providers;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.Models;
using InjectedLocalizations.MemberParsing.Tokens;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace InjectedLocalizations
{
    public class GetTranslatedParametersTests
    {
        [Fact]
        public void Can_get_translated_parameters()
        {
            ITranslator translator;
            IParsedMember parsedMember;
            ParameterInfo[] parameters;
            TranslatedParameter[] translatedParameters;

            translator = Substitute.For<ITranslator>();
            translator
                .TranslateTextAsync(Arg.Is("{count}")
                    , Arg.Is("en")
                    , Arg.Is("es")
                    , Arg.Any<TextTranslateOptions>()
                    , Arg.Any<CancellationToken>())
                .Returns(new TextResult("{conteo}", "language"));

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

            translatedParameters = translator
                .GetTranslatedParameters("en", "es", parsedMember, default)
                .ToArray();

            translatedParameters.Should().HaveCount(1);
            translatedParameters[0].Parameter.Should().Be("{count}");
            translatedParameters[0].Translated.Should().Be("{conteo}");
        }
    }
}