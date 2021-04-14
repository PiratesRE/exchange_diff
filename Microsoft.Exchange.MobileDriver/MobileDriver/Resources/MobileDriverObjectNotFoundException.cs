using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.TextMessaging.MobileDriver.Resources
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MobileDriverObjectNotFoundException : MobilePermanentException
	{
		public MobileDriverObjectNotFoundException(LocalizedString message) : base(message)
		{
		}

		public MobileDriverObjectNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MobileDriverObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
