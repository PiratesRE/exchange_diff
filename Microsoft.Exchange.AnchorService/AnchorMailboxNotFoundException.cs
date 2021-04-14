using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.AnchorService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AnchorMailboxNotFoundException : AnchorPermanentException
	{
		public AnchorMailboxNotFoundException() : base(Strings.AnchorMailboxNotFound)
		{
		}

		public AnchorMailboxNotFoundException(Exception innerException) : base(Strings.AnchorMailboxNotFound, innerException)
		{
		}

		protected AnchorMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
