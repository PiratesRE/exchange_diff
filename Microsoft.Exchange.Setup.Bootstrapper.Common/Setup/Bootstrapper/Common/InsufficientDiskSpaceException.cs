using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InsufficientDiskSpaceException : LocalizedException
	{
		public InsufficientDiskSpaceException() : base(Strings.InsufficientDiskSpace)
		{
		}

		public InsufficientDiskSpaceException(Exception innerException) : base(Strings.InsufficientDiskSpace, innerException)
		{
		}

		protected InsufficientDiskSpaceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
