using DeepL;
using DeepL.Model;
using FluentAssertions;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using NSubstitute;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace InjectedLocalizations
{
    public class TranslateMemberTests
    {
        [Fact]
        public async Task Can_translate_The_file_already_exists_member()
        {
            ITranslator translator;
            IParsedMember parsedMember;
            string translatedMember, sourceEnglishCulture, requestDeeplCulture;

            parsedMember = FakeParsedMember.BuildFor<ISampleInterface>(nameof(ISampleInterface.The_file_already_exists)
                , new CapitalWordToken("The")
                , SpaceToken.Simple
                , new WordToken("file")
                , SpaceToken.Simple
                , new WordToken("already")
                , SpaceToken.Simple
                , new WordToken("exists")
                , EndToken.End);

            translator = Substitute.For<ITranslator>();
            sourceEnglishCulture = "en";
            requestDeeplCulture = "es";
            translator
                .TranslateTextAsync(Arg.Is("The file already exists")
                    , Arg.Is(sourceEnglishCulture)
                    , Arg.Is(requestDeeplCulture)
                    , Arg.Any<TextTranslateOptions>()
                    , Arg.Any<CancellationToken>())
                .Returns(new TextResult("El archivo ya existe", "en"));

            translatedMember = await translator.TranslateMember(parsedMember, sourceEnglishCulture, requestDeeplCulture, default);

            translatedMember.Should().Be("El archivo ya existe");
        }

        [Fact]
        public async Task Can_translate_There_are_0_apples_member()
        {
            ITranslator translator;
            IParsedMember parsedMember;
            ParameterInfo[] parameters;
            string translatedMember, sourceEnglishCulture, requestDeeplCulture;

            translator = Substitute.For<ITranslator>();
            sourceEnglishCulture = "en";
            requestDeeplCulture = "es";

            translator
                .TranslateTextAsync(Arg.Is("{count}"), Arg.Is(sourceEnglishCulture), Arg.Is(requestDeeplCulture), Arg.Any<TextTranslateOptions>(), Arg.Any<CancellationToken>())
                .Returns(new TextResult("{conteo}", "en"));

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

            translator
                .TranslateTextAsync(Arg.Is("There are {count} apples")
                    , Arg.Is(sourceEnglishCulture)
                    , Arg.Is(requestDeeplCulture)
                    , Arg.Any<TextTranslateOptions>()
                    , Arg.Any<CancellationToken>())
                .Returns(new TextResult("Hay {conteo} manzanas", "en"));

            translatedMember = await translator.TranslateMember(parsedMember, sourceEnglishCulture, requestDeeplCulture, default);
            
            translatedMember.Should().Be("Hay {count} manzanas");
        }
    }
}