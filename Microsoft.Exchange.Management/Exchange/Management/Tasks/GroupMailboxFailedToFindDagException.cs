using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GroupMailboxFailedToFindDagException : RecipientTaskException
	{
		public GroupMailboxFailedToFindDagException(LocalizedString message) : base(message)
		{
		}

		public GroupMailboxFailedToFindDagException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected GroupMailboxFailedToFindDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
