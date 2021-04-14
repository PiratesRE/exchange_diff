using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MailboxOverQuotaException : TransientException
	{
		public MailboxOverQuotaException() : base(Strings.MailboxOverQuotaException)
		{
		}

		public MailboxOverQuotaException(Exception innerException) : base(Strings.MailboxOverQuotaException, innerException)
		{
		}

		protected MailboxOverQuotaException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
