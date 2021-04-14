using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SmtpServerInfoMissingException : MailboxReplicationPermanentException
	{
		public SmtpServerInfoMissingException() : base(MrsStrings.SmtpServerInfoMissing)
		{
		}

		public SmtpServerInfoMissingException(Exception innerException) : base(MrsStrings.SmtpServerInfoMissing, innerException)
		{
		}

		protected SmtpServerInfoMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
