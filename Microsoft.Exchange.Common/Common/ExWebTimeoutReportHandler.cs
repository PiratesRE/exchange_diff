using System;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Common
{
	internal class ExWebTimeoutReportHandler
	{
		internal ExWebTimeoutReportHandler()
		{
		}

		internal void Process(HttpContext context)
		{
			ExTraceGlobals.WebHealthTracer.TraceDebug((long)this.GetHashCode(), "ExWebTimeoutReportHandler::ProcessHealth()");
			try
			{
				if (ExMonitoringRequestTracker.Instance.IsKnownMonitoringRequest(context.Request))
				{
					context.Response.StatusCode = 200;
				}
				else
				{
					context.Response.StatusCode = 403;
				}
			}
			catch (Exception ex)
			{
				Exception ex2;
				Exception ex = ex2;
				ExTraceGlobals.WebHealthTracer.TraceError<Exception>((long)this.GetHashCode(), "ExWebTimeoutReportHandler::Process encountered error {0}", ex);
				ThreadPool.QueueUserWorkItem(delegate(object o)
				{
					ExWatson.SendReport(ex, ReportOptions.DoNotCollectDumps | ReportOptions.DoNotFreezeThreads, string.Empty);
				});
			}
		}
	}
}
