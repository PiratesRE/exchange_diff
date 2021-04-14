using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxOrDatabaseNotSpecifiedException : LocalizedException
	{
		public MailboxOrDatabaseNotSpecifiedException() : base(Strings.descMailboxOrDatabaseNotSpecified)
		{
		}

		public MailboxOrDatabaseNotSpecifiedException(Exception innerException) : base(Strings.descMailboxOrDatabaseNotSpecified, innerException)
		{
		}

		protected MailboxOrDatabaseNotSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
