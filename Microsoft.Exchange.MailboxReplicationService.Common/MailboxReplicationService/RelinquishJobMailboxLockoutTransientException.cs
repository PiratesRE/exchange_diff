using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobMailboxLockoutTransientException : RelinquishJobTransientException
	{
		public RelinquishJobMailboxLockoutTransientException(DateTime pickupTime) : base(MrsStrings.JobHasBeenRelinquishedDueToMailboxLockout(pickupTime))
		{
			this.pickupTime = pickupTime;
		}

		public RelinquishJobMailboxLockoutTransientException(DateTime pickupTime, Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToMailboxLockout(pickupTime), innerException)
		{
			this.pickupTime = pickupTime;
		}

		protected RelinquishJobMailboxLockoutTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
