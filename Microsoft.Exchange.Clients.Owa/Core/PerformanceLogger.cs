using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PerformanceLogger
	{
		internal PerformanceLogger(string iisCountLabel, string iisLatencyLabel, string breadcrumbsLabel)
		{
			this.iisCount = iisCountLabel;
			this.iisLatency = iisLatencyLabel;
			this.breadLabel = breadcrumbsLabel;
		}

		internal void AppendIISLogsEntry(StringBuilder iis, uint count, long latencyInMilliseconds)
		{
			iis.Append(this.iisCount).Append(count);
			iis.Append(this.iisLatency).Append(latencyInMilliseconds);
		}

		internal void AppendBreadcrumbEntry(StringBuilder breadcrumb, uint count, long latencyInMilliseconds)
		{
			breadcrumb.Append(this.breadLabel).Append(count);
			breadcrumb.Append(" (").Append(latencyInMilliseconds).Append(" ms)");
		}

		internal const string LeftParenthesis = " (";

		internal const string UnitsAndRightParenthesis = " ms)";

		private readonly string iisCount;

		private readonly string iisLatency;

		private readonly string breadLabel;
	}
}
