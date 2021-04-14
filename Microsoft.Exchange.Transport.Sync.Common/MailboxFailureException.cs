using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MailboxFailureException : LocalizedException
	{
		public MailboxFailureException() : base(Strings.MailboxFailure)
		{
		}

		public MailboxFailureException(Exception innerException) : base(Strings.MailboxFailure, innerException)
		{
		}

		protected MailboxFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
