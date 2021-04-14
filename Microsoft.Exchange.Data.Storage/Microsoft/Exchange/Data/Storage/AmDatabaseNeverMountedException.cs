using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDatabaseNeverMountedException : AmServerException
	{
		public AmDatabaseNeverMountedException() : base(ServerStrings.AmDatabaseNeverMountedException)
		{
		}

		public AmDatabaseNeverMountedException(Exception innerException) : base(ServerStrings.AmDatabaseNeverMountedException, innerException)
		{
		}

		protected AmDatabaseNeverMountedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
