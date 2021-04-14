using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SystemProbeException : LocalizedException
	{
		public SystemProbeException(LocalizedString message) : base(message)
		{
		}

		public SystemProbeException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SystemProbeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
