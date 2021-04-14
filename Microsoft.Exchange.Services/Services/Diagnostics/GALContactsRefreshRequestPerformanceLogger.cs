using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Services.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GALContactsRefreshRequestPerformanceLogger : IPerformanceDataLogger
	{
		internal GALContactsRefreshRequestPerformanceLogger(RequestDetailsLogger logger, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("logger", logger);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.logger = logger;
			this.tracer = tracer;
		}

		public void Log(string marker, string counter, TimeSpan dataPoint)
		{
			this.tracer.TracePerformance<string, string, double>((long)this.GetHashCode(), "{0}.{1}={2}ms", marker, counter, dataPoint.TotalMilliseconds);
			this.MapToMetadataAndLog(marker, counter, dataPoint.TotalMilliseconds);
		}

		public void Log(string marker, string counter, uint dataPoint)
		{
			this.tracer.TracePerformance<string, string, uint>((long)this.GetHashCode(), "{0}.{1}={2}", marker, counter, dataPoint);
			this.MapToMetadataAndLog(marker, counter, dataPoint);
		}

		public void Log(string marker, string counter, string dataPoint)
		{
			this.tracer.TracePerformance<string, string, string>((long)this.GetHashCode(), "{0}.{1}={2}", marker, counter, dataPoint);
			this.MapToMetadataAndLog(marker, counter, dataPoint);
		}

		private void MapToMetadataAndLog(string marker, string counter, object dataPoint)
		{
			GALContactsRefreshLoggingMetadata galcontactsRefreshLoggingMetadata;
			if (!GALContactsRefreshRequestPerformanceLogger.MarkerAndCounterToMetadataMap.TryGetValue(Tuple.Create<string, string>(marker, counter), out galcontactsRefreshLoggingMetadata))
			{
				return;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, galcontactsRefreshLoggingMetadata, dataPoint);
		}

		private const string ElapsedTime = "ElapsedTime";

		private const string CpuTime = "CpuTime";

		private const string LdapCount = "LdapCount";

		private const string LdapLatency = "LdapLatency";

		private const string StoreRpcCount = "StoreRpcCount";

		private const string StoreRpcLatency = "StoreRpcLatency";

		private static readonly Dictionary<Tuple<string, string>, GALContactsRefreshLoggingMetadata> MarkerAndCounterToMetadataMap = new Dictionary<Tuple<string, string>, GALContactsRefreshLoggingMetadata>
		{
			{
				Tuple.Create<string, string>("GALFolderInitialLoad", "ElapsedTime"),
				GALContactsRefreshLoggingMetadata.GALFolderInitialLoad
			}
		};

		private readonly RequestDetailsLogger logger;

		private readonly ITracer tracer;
	}
}
