using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Security.RightsManagement.Protectors
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AttachmentProtectionException : LocalizedException
	{
		public AttachmentProtectionException(LocalizedString message) : base(message)
		{
		}

		public AttachmentProtectionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AttachmentProtectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
