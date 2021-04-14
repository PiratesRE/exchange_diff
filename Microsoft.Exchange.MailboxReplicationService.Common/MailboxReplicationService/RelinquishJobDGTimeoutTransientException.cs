using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobDGTimeoutTransientException : RelinquishJobTransientException
	{
		public RelinquishJobDGTimeoutTransientException(DateTime pickupTime) : base(MrsStrings.JobHasBeenRelinquishedDueToDataGuaranteeTimeout(pickupTime))
		{
			this.pickupTime = pickupTime;
		}

		public RelinquishJobDGTimeoutTransientException(DateTime pickupTime, Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToDataGuaranteeTimeout(pickupTime), innerException)
		{
			this.pickupTime = pickupTime;
		}

		protected RelinquishJobDGTimeoutTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
