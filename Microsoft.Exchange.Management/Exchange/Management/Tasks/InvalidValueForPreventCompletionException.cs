using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidValueForPreventCompletionException : MailboxReplicationPermanentException
	{
		public InvalidValueForPreventCompletionException() : base(Strings.ErrorInvalidValueForPreventCompletion)
		{
		}

		public InvalidValueForPreventCompletionException(Exception innerException) : base(Strings.ErrorInvalidValueForPreventCompletion, innerException)
		{
		}

		protected InvalidValueForPreventCompletionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
