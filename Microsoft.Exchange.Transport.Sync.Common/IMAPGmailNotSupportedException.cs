using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IMAPGmailNotSupportedException : LocalizedException
	{
		public IMAPGmailNotSupportedException() : base(Strings.IMAPGmailNotSupportedException)
		{
		}

		public IMAPGmailNotSupportedException(Exception innerException) : base(Strings.IMAPGmailNotSupportedException, innerException)
		{
		}

		protected IMAPGmailNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
