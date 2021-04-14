using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.Diagnostics
{
	internal class EntitiesDiagnosticsOptions
	{
		public const ReportOptions WatsonReportOptions = ReportOptions.DoNotCollectDumps | ReportOptions.DoNotLogProcessAndThreadIds | ReportOptions.DoNotFreezeThreads;
	}
}
