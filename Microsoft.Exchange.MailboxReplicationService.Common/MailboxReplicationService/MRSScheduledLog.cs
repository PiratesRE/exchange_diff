using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSScheduledLog<T> : ObjectLog<T>
	{
		protected MRSScheduledLog(ObjectLogSchema schema, ObjectLogConfiguration configuration) : base(schema, configuration)
		{
			this.lastLoggingTimeUtc = DateTime.UtcNow;
		}

		protected virtual bool IsLogEnabled
		{
			get
			{
				return false;
			}
		}

		protected virtual TimeSpan ScheduledLoggingPeriod
		{
			get
			{
				return MRSScheduledLog<T>.LoggingPeriod;
			}
		}

		public virtual bool LogIsNeeded()
		{
			bool result = false;
			TimeSpan t = DateTime.UtcNow - this.lastLoggingTimeUtc;
			if (this.IsLogEnabled && t >= this.ScheduledLoggingPeriod)
			{
				result = true;
				this.lastLoggingTimeUtc = DateTime.UtcNow;
			}
			return result;
		}

		private static readonly TimeSpan LoggingPeriod = new TimeSpan(1, 0, 0);

		protected DateTime lastLoggingTimeUtc;
	}
}
