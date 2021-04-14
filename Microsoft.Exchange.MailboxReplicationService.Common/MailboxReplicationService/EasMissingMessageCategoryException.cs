using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasMissingMessageCategoryException : MailboxReplicationPermanentException
	{
		public EasMissingMessageCategoryException() : base(MrsStrings.EasMissingMessageCategory)
		{
		}

		public EasMissingMessageCategoryException(Exception innerException) : base(MrsStrings.EasMissingMessageCategory, innerException)
		{
		}

		protected EasMissingMessageCategoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
