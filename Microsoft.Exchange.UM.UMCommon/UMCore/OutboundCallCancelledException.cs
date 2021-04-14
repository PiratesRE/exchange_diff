using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OutboundCallCancelledException : LocalizedException
	{
		public OutboundCallCancelledException() : base(Strings.OutboundCallCancelled)
		{
		}

		public OutboundCallCancelledException(Exception innerException) : base(Strings.OutboundCallCancelled, innerException)
		{
		}

		protected OutboundCallCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
