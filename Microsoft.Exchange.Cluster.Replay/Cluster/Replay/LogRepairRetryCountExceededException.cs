using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogRepairRetryCountExceededException : LocalizedException
	{
		public LogRepairRetryCountExceededException(long retryCount) : base(ReplayStrings.LogRepairRetryCountExceeded(retryCount))
		{
			this.retryCount = retryCount;
		}

		public LogRepairRetryCountExceededException(long retryCount, Exception innerException) : base(ReplayStrings.LogRepairRetryCountExceeded(retryCount), innerException)
		{
			this.retryCount = retryCount;
		}

		protected LogRepairRetryCountExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.retryCount = (long)info.GetValue("retryCount", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("retryCount", this.retryCount);
		}

		public long RetryCount
		{
			get
			{
				return this.retryCount;
			}
		}

		private readonly long retryCount;
	}
}
