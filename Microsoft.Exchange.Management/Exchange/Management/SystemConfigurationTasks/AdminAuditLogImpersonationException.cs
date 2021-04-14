using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdminAuditLogImpersonationException : AdminAuditLogException
	{
		public AdminAuditLogImpersonationException() : base(Strings.FailedToCreateEwsConnection)
		{
		}

		public AdminAuditLogImpersonationException(Exception innerException) : base(Strings.FailedToCreateEwsConnection, innerException)
		{
		}

		protected AdminAuditLogImpersonationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
