using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DbHTFirstLookupTimeoutException : DatabaseHealthTrackerException
	{
		public DbHTFirstLookupTimeoutException(int timeoutMs) : base(ReplayStrings.DbHTFirstLookupTimeoutException(timeoutMs))
		{
			this.timeoutMs = timeoutMs;
		}

		public DbHTFirstLookupTimeoutException(int timeoutMs, Exception innerException) : base(ReplayStrings.DbHTFirstLookupTimeoutException(timeoutMs), innerException)
		{
			this.timeoutMs = timeoutMs;
		}

		protected DbHTFirstLookupTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.timeoutMs = (int)info.GetValue("timeoutMs", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("timeoutMs", this.timeoutMs);
		}

		public int TimeoutMs
		{
			get
			{
				return this.timeoutMs;
			}
		}

		private readonly int timeoutMs;
	}
}
