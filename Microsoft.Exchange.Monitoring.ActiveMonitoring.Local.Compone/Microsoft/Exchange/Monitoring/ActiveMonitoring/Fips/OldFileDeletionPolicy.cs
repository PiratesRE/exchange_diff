using System;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	internal class OldFileDeletionPolicy : IFileDeletionPolicy
	{
		public OldFileDeletionPolicy(TimeSpan lifeTime)
		{
			this.lifeTime = lifeTime;
		}

		public bool ShouldDelete(string filePath)
		{
			bool result = false;
			try
			{
				if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
				{
					FileInfo fileInfo = new FileInfo(filePath);
					if (fileInfo.CreationTime.ToUniversalTime() <= DateTime.UtcNow.AddMilliseconds(-1.0 * this.lifeTime.TotalMilliseconds))
					{
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.FIPSTracer, this.traceContext, "The file {0} could not be deleted. Exception: {1}", filePath, ex.Message, null, "ShouldDelete", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\OldFileDeletionPolicy.cs", 74);
			}
			return result;
		}

		private readonly TimeSpan lifeTime;

		private TracingContext traceContext = TracingContext.Default;
	}
}
