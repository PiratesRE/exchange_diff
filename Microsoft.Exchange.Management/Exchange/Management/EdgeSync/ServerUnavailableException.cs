using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerUnavailableException : LocalizedException
	{
		public ServerUnavailableException() : base(Strings.ServerUnavailableException)
		{
		}

		public ServerUnavailableException(Exception innerException) : base(Strings.ServerUnavailableException, innerException)
		{
		}

		protected ServerUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
