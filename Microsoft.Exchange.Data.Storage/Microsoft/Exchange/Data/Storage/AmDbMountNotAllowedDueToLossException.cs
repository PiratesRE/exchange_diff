using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMountNotAllowedDueToLossException : AmServerException
	{
		public AmDbMountNotAllowedDueToLossException() : base(ServerStrings.AmDbMountNotAllowedDueToLossException)
		{
		}

		public AmDbMountNotAllowedDueToLossException(Exception innerException) : base(ServerStrings.AmDbMountNotAllowedDueToLossException, innerException)
		{
		}

		protected AmDbMountNotAllowedDueToLossException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
