using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PSTIOExceptionTransientException : MailboxReplicationPermanentException
	{
		public PSTIOExceptionTransientException() : base(MrsStrings.PSTIOException)
		{
		}

		public PSTIOExceptionTransientException(Exception innerException) : base(MrsStrings.PSTIOException, innerException)
		{
		}

		protected PSTIOExceptionTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
