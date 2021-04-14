using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public class ErrorRecord
	{
		public ExportErrorType ErrorType { get; internal set; }

		public DateTime Time { get; internal set; }

		public ExportRecord Item { get; internal set; }

		public string DiagnosticMessage { get; internal set; }

		public string SourceId { get; internal set; }
	}
}
