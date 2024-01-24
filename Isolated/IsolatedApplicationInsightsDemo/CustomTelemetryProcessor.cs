using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System;
namespace IsolatedApplicationInsightsDemo
{
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    public class CustomTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        // Link processors to each other in the chain.
        public CustomTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry telemetry)
        {
            if (telemetry is DependencyTelemetry dependency && dependency.Name == "Invoke")
            {
                // Do not send telemetry for the "Invoke" dependency.
                return;
            }
            // Call the next processor in the chain.
            this.Next.Process(telemetry);
        }
    }
}
