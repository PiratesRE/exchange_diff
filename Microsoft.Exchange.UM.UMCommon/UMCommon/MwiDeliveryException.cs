using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MwiDeliveryException : LocalizedException
	{
		public MwiDeliveryException(LocalizedString message) : base(message)
		{
		}

		public MwiDeliveryException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MwiDeliveryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
