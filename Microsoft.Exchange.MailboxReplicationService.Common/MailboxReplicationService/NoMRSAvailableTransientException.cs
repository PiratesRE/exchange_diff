using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoMRSAvailableTransientException : MailboxReplicationTransientException
	{
		public NoMRSAvailableTransientException() : base(MrsStrings.NoMRSAvailable)
		{
		}

		public NoMRSAvailableTransientException(Exception innerException) : base(MrsStrings.NoMRSAvailable, innerException)
		{
		}

		protected NoMRSAvailableTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
