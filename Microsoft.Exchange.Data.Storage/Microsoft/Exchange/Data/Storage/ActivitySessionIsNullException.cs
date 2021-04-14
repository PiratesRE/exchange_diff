using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ActivitySessionIsNullException : StoragePermanentException
	{
		public ActivitySessionIsNullException() : base(ServerStrings.ActivitySessionIsNull)
		{
		}

		public ActivitySessionIsNullException(Exception innerException) : base(ServerStrings.ActivitySessionIsNull, innerException)
		{
		}

		protected ActivitySessionIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
