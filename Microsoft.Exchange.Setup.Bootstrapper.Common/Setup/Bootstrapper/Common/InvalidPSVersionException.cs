using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidPSVersionException : LocalizedException
	{
		public InvalidPSVersionException() : base(Strings.InvalidPSVersion)
		{
		}

		public InvalidPSVersionException(Exception innerException) : base(Strings.InvalidPSVersion, innerException)
		{
		}

		protected InvalidPSVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
