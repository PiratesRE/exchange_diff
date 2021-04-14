using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.TextMessaging.MobileDriver.Resources
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MobileDriverCantBeCodedException : MobilePermanentException
	{
		public MobileDriverCantBeCodedException(LocalizedString message) : base(message)
		{
		}

		public MobileDriverCantBeCodedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MobileDriverCantBeCodedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
