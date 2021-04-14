using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Net.AAD
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AADTransportException : AADException
	{
		public AADTransportException() : base(NetException.aadTransportFailureException)
		{
		}

		public AADTransportException(Exception innerException) : base(NetException.aadTransportFailureException, innerException)
		{
		}

		protected AADTransportException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
