using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MultipleAdminAuditLogConfigException : AdminAuditLogException
	{
		public MultipleAdminAuditLogConfigException(string organization) : base(Strings.MultipleAdminAuditLogConfig(organization))
		{
			this.organization = organization;
		}

		public MultipleAdminAuditLogConfigException(string organization, Exception innerException) : base(Strings.MultipleAdminAuditLogConfig(organization), innerException)
		{
			this.organization = organization;
		}

		protected MultipleAdminAuditLogConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.organization = (string)info.GetValue("organization", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("organization", this.organization);
		}

		public string Organization
		{
			get
			{
				return this.organization;
			}
		}

		private readonly string organization;
	}
}
