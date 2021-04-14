using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SyncTooSlowException : NonPromotableTransientException
	{
		public SyncTooSlowException(TimeSpan syncDurationThreshold) : base(Strings.SyncTooSlowException(syncDurationThreshold))
		{
			this.syncDurationThreshold = syncDurationThreshold;
		}

		public SyncTooSlowException(TimeSpan syncDurationThreshold, Exception innerException) : base(Strings.SyncTooSlowException(syncDurationThreshold), innerException)
		{
			this.syncDurationThreshold = syncDurationThreshold;
		}

		protected SyncTooSlowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.syncDurationThreshold = (TimeSpan)info.GetValue("syncDurationThreshold", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("syncDurationThreshold", this.syncDurationThreshold);
		}

		public TimeSpan SyncDurationThreshold
		{
			get
			{
				return this.syncDurationThreshold;
			}
		}

		private readonly TimeSpan syncDurationThreshold;
	}
}
