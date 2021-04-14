using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public static class TenantRelocationHealthUtils
	{
		public static void CheckTenantRelocationErrors()
		{
			try
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Following tenant relocation requests have failed with Permanent errors.");
				stringBuilder.AppendLine("TenantName, RelocationStatus, RelocationError");
				IEnumerable<TenantRelocationRequest> enumerable = PartitionDataAggregator.FindPagedRelocationRequestsWithUnclassifiedPermanentError();
				foreach (TenantRelocationRequest tenantRelocationRequest in enumerable)
				{
					num++;
					stringBuilder.AppendLine(string.Format("{0}, {1}, {2}", tenantRelocationRequest.Name, tenantRelocationRequest.RelocationStatus.ToString(), tenantRelocationRequest.RelocationLastError.ToString()));
					if (num >= 30)
					{
						stringBuilder.AppendLine(string.Format("\nAbove tenant list is the top {0} tenants where relocations have failed.  Please investigate.", 30));
						break;
					}
				}
				if (num > 0)
				{
					throw new ApplicationException(stringBuilder.ToString());
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.DirectoryTracer, TenantRelocationHealthUtils.traceContext, "CheckTenantRelocationErrors::Got Exception: {0}", ex.ToString(), null, "CheckTenantRelocationErrors", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TenantRelocationHealthUtils.cs", 73);
				throw;
			}
		}

		private const int MaximumTenantErrorCount = 30;

		private static TracingContext traceContext = TracingContext.Default;
	}
}
