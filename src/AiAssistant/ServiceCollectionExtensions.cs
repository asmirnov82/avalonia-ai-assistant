using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiAssistance.Models;
using AiAssistant.Models;
using AiAssistant.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiAssistant
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationModel, ApplicationModel>();
        }

        public static void AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
        }

        public static void AddAppSettings(this IServiceCollection services, string fileName)
        {
            // Создаем конфигурацию, загружаем appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(fileName, optional: true, reloadOnChange: false)
                .Build();

            var appSettings = configuration.GetSection("ModelParams").Get<LlmConfig>();
            if (appSettings != null)
            {
                services.AddSingleton(appSettings);
            }

            var connectionsSettings = configuration.GetSection("InferenceParams").Get<InferenceConfig>();
            if (connectionsSettings != null)
            {
                services.AddSingleton(connectionsSettings);
            }
        }
    }
}
