using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToCreateAuditLogSearchException : LocalizedException
	{
		public FailedToCreateAuditLogSearchException() : base(Strings.FailedToCreateAuditLogSearch)
		{
		}

		public FailedToCreateAuditLogSearchException(Exception innerException) : base(Strings.FailedToCreateAuditLogSearch, innerException)
		{
		}

		protected FailedToCreateAuditLogSearchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
