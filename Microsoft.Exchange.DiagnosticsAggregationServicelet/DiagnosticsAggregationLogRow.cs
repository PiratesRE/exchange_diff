using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal sealed class DiagnosticsAggregationLogRow : LogRowFormatter
	{
		public DiagnosticsAggregationLogRow(LogSchema schema, DiagnosticsAggregationEvent evt, long? sessionId, TimeSpan? duration, string clientHostName, string clientProcessName, int? clientProcessId, string serverHostName, string description) : base(schema)
		{
			base[1] = Environment.MachineName;
			base[2] = ((sessionId != null) ? sessionId.ToString() : string.Empty);
			base[3] = evt;
			base[4] = ((duration != null) ? duration.ToString() : string.Empty);
			base[5] = clientHostName;
			base[6] = clientProcessName;
			base[7] = ((clientProcessId != null) ? clientProcessId.ToString() : string.Empty);
			base[8] = serverHostName;
			base[9] = description;
		}

		public static readonly string[] Fields = Enum.GetNames(typeof(DiagnosticsAggregationLogRow.Field));

		internal enum Field
		{
			Time,
			HostName,
			SessionId,
			Event,
			Duration,
			ClientHostName,
			ClientProcessName,
			ClientProcessId,
			ServerHostName,
			Description
		}
	}
}
