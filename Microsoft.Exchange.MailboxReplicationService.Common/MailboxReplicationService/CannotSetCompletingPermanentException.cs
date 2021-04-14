using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetCompletingPermanentException : MailboxReplicationPermanentException
	{
		public CannotSetCompletingPermanentException(LocalizedString message) : base(message)
		{
		}

		public CannotSetCompletingPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CannotSetCompletingPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
