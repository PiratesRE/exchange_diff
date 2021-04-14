using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmRefreshConfigTimeoutException : AmCommonTransientException
	{
		public AmRefreshConfigTimeoutException(int timeoutSecs) : base(ReplayStrings.AmRefreshConfigTimeoutError(timeoutSecs))
		{
			this.timeoutSecs = timeoutSecs;
		}

		public AmRefreshConfigTimeoutException(int timeoutSecs, Exception innerException) : base(ReplayStrings.AmRefreshConfigTimeoutError(timeoutSecs), innerException)
		{
			this.timeoutSecs = timeoutSecs;
		}

		protected AmRefreshConfigTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.timeoutSecs = (int)info.GetValue("timeoutSecs", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("timeoutSecs", this.timeoutSecs);
		}

		public int TimeoutSecs
		{
			get
			{
				return this.timeoutSecs;
			}
		}

		private readonly int timeoutSecs;
	}
}
