using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PreventCompletionCannotSetException : MailboxReplicationPermanentException
	{
		public PreventCompletionCannotSetException() : base(Strings.ErrorPreventCompletionCannotSet)
		{
		}

		public PreventCompletionCannotSetException(Exception innerException) : base(Strings.ErrorPreventCompletionCannotSet, innerException)
		{
		}

		protected PreventCompletionCannotSetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
