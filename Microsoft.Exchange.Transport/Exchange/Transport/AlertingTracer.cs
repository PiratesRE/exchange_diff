using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal sealed class AlertingTracer
	{
		public AlertingTracer(Trace tracer, string application)
		{
			this.tracer = tracer;
			this.application = application;
		}

		internal static bool Enabled
		{
			get
			{
				return AlertingTracer.enabled;
			}
			set
			{
				AlertingTracer.enabled = value;
			}
		}

		internal void TraceError(int traceId, string formatString, params object[] parameters)
		{
			try
			{
				if (AlertingTracer.Enabled)
				{
					string text = (parameters == null || parameters.Length == 0) ? formatString : string.Format(formatString, parameters);
					if (this.tracer != null)
					{
						this.tracer.TraceError((long)traceId, text);
					}
					SystemProbe.Trace(this.application, SystemProbe.Status.Fail, text, new object[0]);
					new EventNotificationItem(ExchangeComponent.Transport.Name, ExchangeComponent.Transport.Name, null, text, ResultSeverityLevel.Error).Publish(false);
				}
			}
			catch (FormatException)
			{
				SystemProbe.Trace(this.application, SystemProbe.Status.Fail, "Error logging", new object[0]);
			}
		}

		private static bool enabled = true;

		private readonly Trace tracer;

		private readonly string application;
	}
}
