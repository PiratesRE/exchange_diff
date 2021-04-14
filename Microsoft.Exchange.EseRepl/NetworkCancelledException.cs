using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.EseRepl
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkCancelledException : NetworkTransportException
	{
		public NetworkCancelledException() : base(Strings.NetworkCancelled)
		{
		}

		public NetworkCancelledException(Exception innerException) : base(Strings.NetworkCancelled, innerException)
		{
		}

		protected NetworkCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
