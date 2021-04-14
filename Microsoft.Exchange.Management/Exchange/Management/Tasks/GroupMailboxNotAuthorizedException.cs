using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GroupMailboxNotAuthorizedException : RecipientTaskException
	{
		public GroupMailboxNotAuthorizedException(LocalizedString message) : base(message)
		{
		}

		public GroupMailboxNotAuthorizedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected GroupMailboxNotAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
