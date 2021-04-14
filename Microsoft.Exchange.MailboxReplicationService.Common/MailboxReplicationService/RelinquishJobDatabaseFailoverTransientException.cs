using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobDatabaseFailoverTransientException : RelinquishJobTransientException
	{
		public RelinquishJobDatabaseFailoverTransientException() : base(MrsStrings.JobHasBeenRelinquishedDueToDatabaseFailover)
		{
		}

		public RelinquishJobDatabaseFailoverTransientException(Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToDatabaseFailover, innerException)
		{
		}

		protected RelinquishJobDatabaseFailoverTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
