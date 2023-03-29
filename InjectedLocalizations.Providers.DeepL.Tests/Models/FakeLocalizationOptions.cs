using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Attributes;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Models
{
    public class FakeLocalizationOptions : ILocalizationOptions
    {
        public bool ReadOnly { get; set; }
        public Type ProviderType { get; private set; } = null;
        public int ProviderPriority { get; private set; } = 0;
        public IDictionary<Type, object> Services { get; private set; } = new Dictionary<Type, object>();

        public void AddService(Type serviceType, object instance)
        {
            this.Services[serviceType] = instance;
        }

        public void SetAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public void SetBuilder<TAssemblyBuilder>() where TAssemblyBuilder : IAssemblyBuilder
        {
            throw new NotImplementedException();
        }

        public void SetCache<TCacheService>() where TCacheService : ILocalizationsCacheService
        {
            throw new NotImplementedException();
        }

        public void SetCulture(CultureInfo culture, bool defaultCulture)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentCultureProvider<TCurrentCultureProvider>() where TCurrentCultureProvider : ICurrentCultureProvider
        {
            throw new NotImplementedException();
        }

        public void SetMemberParser<TAttribute>(Func<IMemberParser> createMemberParser) where TAttribute : Attribute, ILocalizationsAttribute
        {
            throw new NotImplementedException();
        }

        public void SetProvider<TProvider>(int priority) where TProvider : ILocalizationsProvider
        {
            this.ProviderType.Should().BeNull("it can be setted once.");
            this.ProviderType = typeof(TProvider);
            
            this.ProviderPriority.Should().Be(0, "it can be setted once.");
            this.ProviderPriority = priority;
        }

        public void SetScopedServiceLifetime()
        {
            throw new NotImplementedException();
        }

        public void SetSingletonServiceLifetime()
        {
            throw new NotImplementedException();
        }

        public void SetTransientServiceLifetime()
        {
            throw new NotImplementedException();
        }
    }
}