using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidMRSServerPermanentException : MailboxReplicationPermanentException
	{
		public InvalidMRSServerPermanentException(LocalizedString message) : base(message)
		{
		}

		public InvalidMRSServerPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidMRSServerPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
