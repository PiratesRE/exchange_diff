using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TenantNotificationTestFirstOrgNotSupportedException : LocalizedException
	{
		public TenantNotificationTestFirstOrgNotSupportedException() : base(Strings.TenantNotificationTestFirstOrgNotSupported)
		{
		}

		public TenantNotificationTestFirstOrgNotSupportedException(Exception innerException) : base(Strings.TenantNotificationTestFirstOrgNotSupported, innerException)
		{
		}

		protected TenantNotificationTestFirstOrgNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
