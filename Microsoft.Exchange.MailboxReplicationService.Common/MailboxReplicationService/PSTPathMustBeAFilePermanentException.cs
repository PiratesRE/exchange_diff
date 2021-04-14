using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PSTPathMustBeAFilePermanentException : MailboxReplicationPermanentException
	{
		public PSTPathMustBeAFilePermanentException() : base(MrsStrings.PSTPathMustBeAFile)
		{
		}

		public PSTPathMustBeAFilePermanentException(Exception innerException) : base(MrsStrings.PSTPathMustBeAFile, innerException)
		{
		}

		protected PSTPathMustBeAFilePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
