using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidNetFwVersionException : LocalizedException
	{
		public InvalidNetFwVersionException() : base(Strings.InvalidNetFwVersion)
		{
		}

		public InvalidNetFwVersionException(Exception innerException) : base(Strings.InvalidNetFwVersion, innerException)
		{
		}

		protected InvalidNetFwVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
