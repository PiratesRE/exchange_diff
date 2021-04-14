using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxNotFoundException : DataSourceOperationException
	{
		public MailboxNotFoundException(LocalizedString message) : base(message)
		{
		}

		public MailboxNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
