using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobOfflineTransientException : RelinquishJobTransientException
	{
		public RelinquishJobOfflineTransientException(DateTime pickupTime) : base(MrsStrings.JobHasBeenRelinquishedDueToTransientErrorDuringOfflineMove(pickupTime))
		{
			this.pickupTime = pickupTime;
		}

		public RelinquishJobOfflineTransientException(DateTime pickupTime, Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToTransientErrorDuringOfflineMove(pickupTime), innerException)
		{
			this.pickupTime = pickupTime;
		}

		protected RelinquishJobOfflineTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.pickupTime = (DateTime)info.GetValue("pickupTime", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("pickupTime", this.pickupTime);
		}

		public DateTime PickupTime
		{
			get
			{
				return this.pickupTime;
			}
		}

		private readonly DateTime pickupTime;
	}
}
