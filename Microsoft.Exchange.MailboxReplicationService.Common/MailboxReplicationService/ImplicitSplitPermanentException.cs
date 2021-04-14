using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ImplicitSplitPermanentException : MailboxReplicationPermanentException
	{
		public ImplicitSplitPermanentException() : base(MrsStrings.ErrorImplicitSplit)
		{
		}

		public ImplicitSplitPermanentException(Exception innerException) : base(MrsStrings.ErrorImplicitSplit, innerException)
		{
		}

		protected ImplicitSplitPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
