using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToReadADTransientException : MailboxReplicationTransientException
	{
		public UnableToReadADTransientException() : base(MrsStrings.UnableToReadAD)
		{
		}

		public UnableToReadADTransientException(Exception innerException) : base(MrsStrings.UnableToReadAD, innerException)
		{
		}

		protected UnableToReadADTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
