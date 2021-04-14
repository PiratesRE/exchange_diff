using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MobileRecoInvalidRequestException : LocalizedException
	{
		public MobileRecoInvalidRequestException(LocalizedString message) : base(message)
		{
		}

		public MobileRecoInvalidRequestException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MobileRecoInvalidRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
