using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using DeepL;
using DeepL.Model;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Strings;
using JimenaTools.Extensions.Validations;
using Microsoft.Extensions.Logging;

namespace InjectedLocalizations.Providers
{
    public class DeepLLocalizationsProvider : AbstractLocalizationsProvider
    {
        private static readonly CultureInfo englishCulture = new CultureInfo("en");
        private readonly ITranslator translator;
        private readonly ILogger<DeepLLocalizationsProvider> logger;
        private readonly IDeeplLocalizatonsProviderConfiguration configuration;
        private readonly IDeeplCultureMap deeplCultureMap;
        private readonly ILocalizationAttributeMemberParser parser;

        public DeepLLocalizationsProvider(ILogger<DeepLLocalizationsProvider> logger
            , IDeeplLocalizatonsProviderConfiguration configuration
            , IDeeplCultureMap deeplCultureMap
            , ILocalizationAttributeMemberParser parser
            ) : base()
        {
            this.configuration = configuration.ShouldBeNotNull(nameof(configuration));
            this.logger = logger.ShouldBeNotNull(nameof(logger));
            this.deeplCultureMap = deeplCultureMap.ShouldBeNotNull(nameof(deeplCultureMap));
            this.parser = parser.ShouldBeNotNull(nameof(parser));

            this.translator = new Translator(this.configuration.ApiKey, new TranslatorOptions
            {
                ServerUrl = this.configuration.Url,
            }) ?? throw new DeepLApiException();
        }

        public override bool CanIssueToWriters => true;
        public override bool CanWriteFromIssuers => false;

        public override void Write(ILocalizationResponse response) => throw new NotWritableProviderLocalizationException(this);

        protected override IReadOnlyDictionary<MemberInfo, string> GetLocalizationsOrNull(ILocalizationRequest request)
        {
            GlossaryInfo glossary;
            CancellationTokenSource tokenSource;
            Dictionary<MemberInfo, string> localizations;
            TextTranslateOptions translateOptions;
            IEnumerable<IParsedMember> parsedMembers;
            string requestDeeplCulture, sourceDeeplCulture;

            if (request.Culture.Equals(englishCulture))
                return null;

            sourceDeeplCulture = this.deeplCultureMap.MapToDeepl(englishCulture);
            requestDeeplCulture = this.deeplCultureMap.MapToDeepl(request.Culture);

            tokenSource = new CancellationTokenSource();
            parsedMembers = request
                .InterfaceType
                .GetMethodsAndProperties()
                .Where(m => !m.IsILocalizationsCultureProperty())
                .Select(this.parser.Parse);

            try
            {
                //glossary = this.translator
                //    .GetOrCreateGlossary(sourceDeeplCulture, requestDeeplCulture, tokenSource.Token)
                //    .Result;

                localizations = new Dictionary<MemberInfo, string>();
                translateOptions = new TextTranslateOptions
                {
                    //GlossaryId = glossary.GlossaryId,
                };

                foreach (IParsedMember parsedMember in parsedMembers)
                {
                    string deeplString, interpolatedTranslatedText;
                    string[] parameterNamesArray, translatedNamesArray;
                    TextResult result;
                    IEnumerable<PrvTranslatedParameter> translatedParameters;

                    parameterNamesArray = parsedMember
                        .Where(m => m is ParameterToken)
                        .Cast<ParameterToken>()
                        .Select(p => $"{{{p.Value}}}")
                        .ToArray();

                    if (parameterNamesArray.Length > 0)
                    {
                        result = this.translator
                            .TranslateTextAsync(parameterNamesArray.AsJoinedWith(Environment.NewLine)
                                , sourceDeeplCulture
                                , requestDeeplCulture
                                , translateOptions
                                , tokenSource.Token)
                            .Result;

                        translatedNamesArray = result
                            .Text
                            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                            .ToArray();

                        translatedParameters = parameterNamesArray
                            .Select((p, i) => new PrvTranslatedParameter
                            {
                                Translated = translatedNamesArray[i],
                                Parameter = p,
                            });
                    }
                    else
                        translatedParameters = Enumerable.Empty<PrvTranslatedParameter>();

                    deeplString = parsedMember
                        .Where(p => p is IPrintableToken)
                        .Cast<IPrintableToken>()
                        .Select(p =>
                        {
                            if (p is ParameterToken parameterToken)
                                return $"{{{parameterToken.Value}}}";
                            else
                                return p.Value;
                        })
                        .AsJoined();

                    result = this.translator
                        .TranslateTextAsync(deeplString
                            , sourceDeeplCulture
                            , requestDeeplCulture
                            , translateOptions
                            , tokenSource.Token)
                        .Result;

                    interpolatedTranslatedText = result.Text;

                    translatedParameters
                        .AndApply(p => interpolatedTranslatedText = interpolatedTranslatedText.Replace(p.Translated, p.Parameter));

                    localizations.Add(parsedMember.Member, interpolatedTranslatedText);
                }

                return localizations;
            }
            catch (DeepLException ex)
            {
                this.logger.LogError(ex, $"Troubles during {nameof(GetLocalizationsOrNull)}!");
                return null;
            }
        }

        private class PrvTranslatedParameter
        {
            public string Translated { get; set; }
            public string Parameter { get; set; }
        }
    }
}
