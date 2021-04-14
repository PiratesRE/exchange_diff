using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3CapabilitiesNotSupportedException : LocalizedException
	{
		public Pop3CapabilitiesNotSupportedException() : base(CXStrings.Pop3CapabilitiesNotSupportedMsg)
		{
		}

		public Pop3CapabilitiesNotSupportedException(Exception innerException) : base(CXStrings.Pop3CapabilitiesNotSupportedMsg, innerException)
		{
		}

		protected Pop3CapabilitiesNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
