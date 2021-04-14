using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.TextMessaging.MobileDriver.Resources
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MobileDriverTooManyRetriesException : MobileDriverTransientException
	{
		public MobileDriverTooManyRetriesException(LocalizedString message) : base(message)
		{
		}

		public MobileDriverTooManyRetriesException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MobileDriverTooManyRetriesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
