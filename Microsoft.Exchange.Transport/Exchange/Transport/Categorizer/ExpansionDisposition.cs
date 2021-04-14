using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal enum ExpansionDisposition
	{
		Expanded,
		NonreportableLoopDetected,
		ReportableLoopDetected
	}
}
