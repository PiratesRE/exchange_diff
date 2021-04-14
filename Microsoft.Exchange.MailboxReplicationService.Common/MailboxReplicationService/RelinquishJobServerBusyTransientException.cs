using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobServerBusyTransientException : RelinquishJobTransientException
	{
		public RelinquishJobServerBusyTransientException(LocalizedString error, TimeSpan backoffTimeSpan) : base(MrsStrings.JobHasBeenRelinquishedDueToServerBusy(error, backoffTimeSpan))
		{
			this.error = error;
			this.backoffTimeSpan = backoffTimeSpan;
		}

		public RelinquishJobServerBusyTransientException(LocalizedString error, TimeSpan backoffTimeSpan, Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToServerBusy(error, backoffTimeSpan), innerException)
		{
			this.error = error;
			this.backoffTimeSpan = backoffTimeSpan;
		}

		protected RelinquishJobServerBusyTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (LocalizedString)info.GetValue("error", typeof(LocalizedString));
			this.backoffTimeSpan = (TimeSpan)info.GetValue("backoffTimeSpan", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
			info.AddValue("backoffTimeSpan", this.backoffTimeSpan);
		}

		public LocalizedString Error
		{
			get
			{
				return this.error;
			}
		}

		public TimeSpan BackoffTimeSpan
		{
			get
			{
				return this.backoffTimeSpan;
			}
		}

		private readonly LocalizedString error;

		private readonly TimeSpan backoffTimeSpan;
	}
}
