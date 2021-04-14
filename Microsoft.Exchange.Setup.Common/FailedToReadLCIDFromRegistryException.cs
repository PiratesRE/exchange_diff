using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToReadLCIDFromRegistryException : LocalizedException
	{
		public FailedToReadLCIDFromRegistryException() : base(Strings.FailedToReadLCIDFromRegistryError)
		{
		}

		public FailedToReadLCIDFromRegistryException(Exception innerException) : base(Strings.FailedToReadLCIDFromRegistryError, innerException)
		{
		}

		protected FailedToReadLCIDFromRegistryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
