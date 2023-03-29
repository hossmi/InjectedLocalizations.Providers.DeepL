using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeepL;
using DeepL.Model;
using InjectedLocalizations.MemberParsing.Tokens;
using InjectedLocalizations.MemberParsing;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Strings;
using System.Reflection;

namespace InjectedLocalizations.Providers
{
    public static class DeepLLocalizationsProviderExtensions
    {
        public static async Task<GlossaryInfo> GetOrCreateGlossary(this ITranslator translator
            , string sourceCulture, string targetCulture, CancellationToken cancellationToken)
        {
            GlossaryInfo[] glossaries;
            GlossaryInfo targetGlossary;
            string glossaryName;
            IDictionary<string, string> glossary;

            glossaries = await translator.ListGlossariesAsync(cancellationToken);
            glossaryName = BuildGlossaryName(targetCulture);
            targetGlossary = glossaries.SingleOrDefault(g => g.Name == glossaryName);

            if (targetGlossary != null)
                return targetGlossary;

            glossary = new Dictionary<string, string>();

            Enumerable
                .Range(0, 100)
                .Select(p => $"p{p}")
                .AndApply(p => glossary.Add(p, p));

            targetGlossary = await translator.CreateGlossaryAsync(glossaryName
                , sourceCulture
                , targetCulture
                , new GlossaryEntries(glossary)
                , cancellationToken);

            return targetGlossary;
        }

        public static string BuildGlossaryName(string targetCulture)
        {
            return $"Parameters_EN_to_{targetCulture.Replace("-", "")}";
        }

        public static IEnumerable<TranslatedParameter> GetTranslatedParameters(this ITranslator translator
            , string sourceEnglishCulture
            , string requestDeeplCulture
            , IParsedMember parsedMember
            , CancellationToken cancellationToken)
        {
            string[] parameterNames, translatedNamesArray;

            parameterNames = parsedMember
                .Where(m => m is ParameterToken)
                .Cast<ParameterToken>()
                .Select(p => $"{{{p.Value}}}")
                .ToArray();

            if (parameterNames.Length == 0)
                return Enumerable.Empty<TranslatedParameter>();

            translatedNamesArray = translator
                .TranslateTextAsync(parameterNames.MergedWith(Environment.NewLine)
                    , sourceEnglishCulture
                    , requestDeeplCulture
                    , cancellationToken: cancellationToken)
                .Result
                .Text
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            return parameterNames
                .Select((p, i) => new TranslatedParameter
                {
                    Translated = translatedNamesArray[i],
                    Parameter = p,
                });
        }

        public static IEnumerable<string> AsDeepLString(this IParsedMember parsedMember)
        {
            foreach (IToken token in parsedMember)
                if (token is IPrintableToken printable)
                    if (printable is ParameterToken parameter)
                        yield return $"{{{parameter.Value}}}";
                    else
                        yield return printable.Value;
        }

        public static string TranslateMember(this ITranslator translator
            , IParsedMember parsedMember
            , string sourceEnglishCulture
            , string requestDeeplCulture
            , CancellationToken cancellationToken)
        {
            IEnumerable<TranslatedParameter> translatedParameters;
            string deeplString, interpolatedTranslatedText;

            translatedParameters = translator.GetTranslatedParameters(
                sourceEnglishCulture
                , requestDeeplCulture
                , parsedMember
                , cancellationToken);

            deeplString = parsedMember
                .AsDeepLString()
                .Merged();

            interpolatedTranslatedText = translator
                .TranslateTextAsync(deeplString
                    , sourceEnglishCulture
                    , requestDeeplCulture
                    , cancellationToken: cancellationToken)
                .Result
                .Text;

            translatedParameters
                .AndApply(p => interpolatedTranslatedText = interpolatedTranslatedText
                    .Replace(p.Translated, p.Parameter));

            return interpolatedTranslatedText;
        }
    }
}
