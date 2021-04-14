using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BackSyncUrlInvalidProtcolFormatException : LocalizedException
	{
		public BackSyncUrlInvalidProtcolFormatException() : base(Strings.BackSyncUrlInvalidProtcolFormat)
		{
		}

		public BackSyncUrlInvalidProtcolFormatException(Exception innerException) : base(Strings.BackSyncUrlInvalidProtcolFormat, innerException)
		{
		}

		protected BackSyncUrlInvalidProtcolFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
