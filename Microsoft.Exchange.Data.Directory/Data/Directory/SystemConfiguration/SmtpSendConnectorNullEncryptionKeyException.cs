using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SmtpSendConnectorNullEncryptionKeyException : LocalizedException
	{
		public SmtpSendConnectorNullEncryptionKeyException() : base(DirectoryStrings.NullPasswordEncryptionKey)
		{
		}

		public SmtpSendConnectorNullEncryptionKeyException(Exception innerException) : base(DirectoryStrings.NullPasswordEncryptionKey, innerException)
		{
		}

		protected SmtpSendConnectorNullEncryptionKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
