using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdminAuditLogConfigurationNotFoundException : AdminAuditLogException
	{
		public AdminAuditLogConfigurationNotFoundException(LocalizedString message) : base(message)
		{
		}

		public AdminAuditLogConfigurationNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AdminAuditLogConfigurationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
