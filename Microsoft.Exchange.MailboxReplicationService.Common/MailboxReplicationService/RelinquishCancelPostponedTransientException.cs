using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishCancelPostponedTransientException : RelinquishJobTransientException
	{
		public RelinquishCancelPostponedTransientException(DateTime removeAfter) : base(MrsStrings.JobHasBeenRelinquishedDueToCancelPostponed(removeAfter))
		{
			this.removeAfter = removeAfter;
		}

		public RelinquishCancelPostponedTransientException(DateTime removeAfter, Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToCancelPostponed(removeAfter), innerException)
		{
			this.removeAfter = removeAfter;
		}

		protected RelinquishCancelPostponedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.removeAfter = (DateTime)info.GetValue("removeAfter", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("removeAfter", this.removeAfter);
		}

		public DateTime RemoveAfter
		{
			get
			{
				return this.removeAfter;
			}
		}

		private readonly DateTime removeAfter;
	}
}
