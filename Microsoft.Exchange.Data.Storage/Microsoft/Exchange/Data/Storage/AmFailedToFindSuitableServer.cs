using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmFailedToFindSuitableServer : AmServerException
	{
		public AmFailedToFindSuitableServer() : base(ServerStrings.AmFailedToFindSuitableServer)
		{
		}

		public AmFailedToFindSuitableServer(Exception innerException) : base(ServerStrings.AmFailedToFindSuitableServer, innerException)
		{
		}

		protected AmFailedToFindSuitableServer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
