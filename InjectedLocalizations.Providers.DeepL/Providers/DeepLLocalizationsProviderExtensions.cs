using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeepL;
using DeepL.Model;
using JimenaTools.Extensions.Enumerables;

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
    }
}
