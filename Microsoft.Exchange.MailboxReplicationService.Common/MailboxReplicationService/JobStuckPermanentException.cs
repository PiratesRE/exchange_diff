using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JobStuckPermanentException : MailboxReplicationPermanentException
	{
		public JobStuckPermanentException(DateTime lastProgressTimestamp, DateTime jobPickupTimestamp) : base(MrsStrings.JobIsStuck(lastProgressTimestamp, jobPickupTimestamp))
		{
			this.lastProgressTimestamp = lastProgressTimestamp;
			this.jobPickupTimestamp = jobPickupTimestamp;
		}

		public JobStuckPermanentException(DateTime lastProgressTimestamp, DateTime jobPickupTimestamp, Exception innerException) : base(MrsStrings.JobIsStuck(lastProgressTimestamp, jobPickupTimestamp), innerException)
		{
			this.lastProgressTimestamp = lastProgressTimestamp;
			this.jobPickupTimestamp = jobPickupTimestamp;
		}

		protected JobStuckPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.lastProgressTimestamp = (DateTime)info.GetValue("lastProgressTimestamp", typeof(DateTime));
			this.jobPickupTimestamp = (DateTime)info.GetValue("jobPickupTimestamp", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lastProgressTimestamp", this.lastProgressTimestamp);
			info.AddValue("jobPickupTimestamp", this.jobPickupTimestamp);
		}

		public DateTime LastProgressTimestamp
		{
			get
			{
				return this.lastProgressTimestamp;
			}
		}

		public DateTime JobPickupTimestamp
		{
			get
			{
				return this.jobPickupTimestamp;
			}
		}

		private readonly DateTime lastProgressTimestamp;

		private readonly DateTime jobPickupTimestamp;
	}
}
