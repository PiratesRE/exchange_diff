using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TextMessageInsufficientPermissionException : LocalizedException
	{
		public TextMessageInsufficientPermissionException() : base(Strings.ErrorTextMessageInsufficientPermission)
		{
		}

		public TextMessageInsufficientPermissionException(Exception innerException) : base(Strings.ErrorTextMessageInsufficientPermission, innerException)
		{
		}

		protected TextMessageInsufficientPermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
