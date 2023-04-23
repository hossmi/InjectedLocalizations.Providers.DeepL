using System;
using System.Globalization;
using FluentAssertions;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace InjectedLocalizations
{
    public class IntegrationTests
    {
        public static string DeeplApiKey => throw new ArgumentException("Put your DeepL key");
        public static string DeeplApiUrl => throw new ArgumentException("Put the DeepL api url");

        [Fact]
        public void Everything_is_fine()
        {
            IServiceCollection services;
            IServiceProvider serviceProvider;
            ISampleInterface sampleInterface;
            FakeCurrentCultureProvider cultureProvider;

            services = new ServiceCollection();
            services.AddInterfacedLocalizations(options =>
            {
                options.SetCulture(new CultureInfo("en-US"), true);
                options.SetCulture(new CultureInfo("es-ES"));

                options.UseDeepLProvider(100, deeplOptions =>
                {
                    deeplOptions.ApiKey = DeeplApiKey;
                    deeplOptions.Url = DeeplApiUrl;
                });

                options.SetCurrentCultureProvider<FakeCurrentCultureProvider>();
            });

            services.AddLogging();
            serviceProvider = services.BuildServiceProvider();
            
            cultureProvider = serviceProvider
                .GetService<ICurrentCultureProvider>()
                .As<FakeCurrentCultureProvider>();

            cultureProvider.CurrentCulture = new CultureInfo("en-US");
            sampleInterface = serviceProvider.GetService<ISampleInterface>();
            sampleInterface.There_are_0_apples(5).Should().Be("There are 5 apples");
            sampleInterface.The_file_already_exists.Should().Be("The file already exists");

            cultureProvider.CurrentCulture = new CultureInfo("es-ES");
            sampleInterface.There_are_0_apples(5).Should().Be("Hay 5 manzanas");
            sampleInterface.The_file_already_exists.Should().Be("El fichero ya existe");
        }
    }
}