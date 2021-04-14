using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidOSVersionException : LocalizedException
	{
		public InvalidOSVersionException() : base(Strings.InvalidOSVersion)
		{
		}

		public InvalidOSVersionException(Exception innerException) : base(Strings.InvalidOSVersion, innerException)
		{
		}

		protected InvalidOSVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
