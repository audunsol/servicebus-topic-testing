using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace service_bus_testing
{
    public class Config : IConfig
    {
        public Config()
        {
            Configuration = BuildConfiguration();
        }

        public string TopicConnectionString => Configuration["Topic:ConnectionString"];
        public string SubscriptionNameA => Configuration["Topic:SubscriptionA:Name"];
        public string SubscriptionNameB => Configuration["Topic:SubscriptionB:Name"];
        public string FilterPropA => Configuration["Topic:SubscriptionA:FilterProperty"];
        public string FilterPropB => Configuration["Topic:SubscriptionB:FilterProperty"];

        public IConfigurationRoot Configuration { get; }

        private IConfigurationRoot BuildConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");

            if (!string.IsNullOrEmpty(env))
            {
                builder.AddJsonFile($"appSettings.{env}.json");
            }

            return builder.Build();
        }
    }

}
