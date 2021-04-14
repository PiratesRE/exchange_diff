using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DeltaSyncServiceEndpointsLoadException : LocalizedException
	{
		public DeltaSyncServiceEndpointsLoadException() : base(Strings.DeltaSyncServiceEndpointsLoadException)
		{
		}

		public DeltaSyncServiceEndpointsLoadException(Exception innerException) : base(Strings.DeltaSyncServiceEndpointsLoadException, innerException)
		{
		}

		protected DeltaSyncServiceEndpointsLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
