using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IProgressController
	{
		bool IsStopRequested { get; }

		bool IsDocumentIdHintFlightingEnabled { get; }

		void ReportProgress(ProgressRecord progressRecord);
	}
}
