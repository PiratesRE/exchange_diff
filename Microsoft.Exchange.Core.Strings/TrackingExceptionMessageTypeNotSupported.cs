using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrackingExceptionMessageTypeNotSupported : LocalizedException
	{
		public TrackingExceptionMessageTypeNotSupported() : base(CoreStrings.TrackingMessageTypeNotSupported)
		{
		}

		public TrackingExceptionMessageTypeNotSupported(Exception innerException) : base(CoreStrings.TrackingMessageTypeNotSupported, innerException)
		{
		}

		protected TrackingExceptionMessageTypeNotSupported(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
