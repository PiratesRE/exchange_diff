using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal interface IReportAnnotation
	{
		string ReportTitle { get; }

		IEnumerable<string> Xaxis { get; }

		IEnumerable<string> Yaxis { get; }
	}
}
