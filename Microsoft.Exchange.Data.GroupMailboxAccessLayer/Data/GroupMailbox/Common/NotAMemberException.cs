using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.GroupMailbox.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotAMemberException : DataSourceOperationException
	{
		public NotAMemberException(LocalizedString message) : base(message)
		{
		}

		public NotAMemberException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected NotAMemberException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
