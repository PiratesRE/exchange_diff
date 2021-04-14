using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.AnchorService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MultipleAnchorMailboxesFoundException : AnchorPermanentException
	{
		public MultipleAnchorMailboxesFoundException() : base(Strings.MultipleAnchorMailboxesFound)
		{
		}

		public MultipleAnchorMailboxesFoundException(Exception innerException) : base(Strings.MultipleAnchorMailboxesFound, innerException)
		{
		}

		protected MultipleAnchorMailboxesFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
