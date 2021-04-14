using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal sealed class SubActivityScopeLogger
	{
		private SubActivityScopeLogger(RequestDetailsLogger logger, Enum subActivityId)
		{
			this.logger = logger;
			this.subActivityId = subActivityId;
			this.log.Append("[");
		}

		internal static SubActivityScopeLogger Create(RequestDetailsLogger logger, Enum subActivityId)
		{
			return new SubActivityScopeLogger(logger, subActivityId);
		}

		internal void LogNext(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw new ArgumentNullException("tag");
			}
			this.log.Append(tag);
			this.log.Append(":");
			this.log.Append(this.stopwatch.ElapsedMilliseconds);
			this.log.Append(",");
		}

		internal void LogEnd()
		{
			this.LogNext("End");
			this.log.Append("]");
			if (this.logger != null && this.stopwatch.ElapsedMilliseconds > 1000L)
			{
				this.logger.Set(this.subActivityId, this.log.ToString());
			}
		}

		private const uint Threshold = 1000U;

		private StringBuilder log = new StringBuilder(128);

		private Stopwatch stopwatch = Stopwatch.StartNew();

		private RequestDetailsLogger logger;

		private Enum subActivityId;
	}
}
