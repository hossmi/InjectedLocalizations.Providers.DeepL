using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using DeepL;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.MemberParsing;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Validations;
using Microsoft.Extensions.Logging;

namespace InjectedLocalizations.Providers
{
    public class DeepLLocalizationsProvider : AbstractLocalizationsProvider
    {
        private static readonly CultureInfo englishCulture = new CultureInfo("en-US");
        private readonly string sourceEnglishCulture;
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
            this.sourceEnglishCulture = this.deeplCultureMap.MapAsSourceLanguage(englishCulture);
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
            CancellationTokenSource tokenSource;
            IEnumerable<IParsedMember> parsedMembers;
            string requestDeeplCulture;

            if (request.Culture.Equals(englishCulture))
                return null;

            tokenSource = new CancellationTokenSource();
            requestDeeplCulture = this.deeplCultureMap.MapAsTargetLanguage(request.Culture);
            parsedMembers = request
                .InterfaceType
                .GetMethodsAndProperties()
                .Where(m => !m.IsILocalizationsCultureProperty())
                .Select(this.parser.Parse);

            try
            {
                return parsedMembers
                    .Select(parsedMember =>
                    {
                        string text = this.translator
                            .TranslateMember(parsedMember
                                , this.sourceEnglishCulture
                                , requestDeeplCulture
                                , tokenSource.Token)
                            .Result;

                        return new KeyValuePair<MemberInfo, string>(parsedMember.Member, text);
                    })
                    .ToDictionary();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Troubles during {nameof(GetLocalizationsOrNull)}!");
                return null;
            }
        }
    }
}
