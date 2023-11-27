using InProcApplicationInsightsDemo;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
 
namespace InProcApplicationInsightsDemo
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ITelemetryModule, MyModule>();
        }
    }

    internal class MyModule : ITelemetryModule
    {
        public void Initialize(TelemetryConfiguration configuration)
        {
            configuration.TelemetryProcessorChainBuilder.Use(next => new SamplingProcessor(next));
        }
    }

    internal class SamplingProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public SamplingProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is ISupportProperties propItem)
            {
                propItem.Properties["Processor"] = "SamplingProcessor";
            }

            // Ensure this is logged always
            if (item is TraceTelemetry trace && trace.Message.Contains("Important:"))
            {
                trace.ProactiveSamplingDecision = SamplingDecision.SampledIn;
            }
            _next.Process(item);
        }
    }
}
