using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3CapabilitiesNotSupportedException : LocalizedException
	{
		public Pop3CapabilitiesNotSupportedException() : base(Strings.Pop3CapabilitiesNotSupportedException)
		{
		}

		public Pop3CapabilitiesNotSupportedException(Exception innerException) : base(Strings.Pop3CapabilitiesNotSupportedException, innerException)
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
