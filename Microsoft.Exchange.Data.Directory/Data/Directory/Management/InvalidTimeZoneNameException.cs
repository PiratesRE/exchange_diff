using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidTimeZoneNameException : LocalizedException
	{
		public InvalidTimeZoneNameException(LocalizedString message) : base(message)
		{
		}

		public InvalidTimeZoneNameException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidTimeZoneNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
