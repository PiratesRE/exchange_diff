using System;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.LogAnalyzer.Extensions.OABDownloadLog
{
	public abstract class OABDownloadLogAnalyzer : LogAnalyzer
	{
		protected OABDownloadLogAnalyzer(IJob job) : base(job)
		{
		}

		public sealed override SessionLogAnalyzer CreateSessionLogAnalyzer()
		{
			return null;
		}
	}
}
