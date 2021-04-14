using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AdminAuditLogIdParameter : ADIdParameter
	{
		public AdminAuditLogIdParameter()
		{
		}

		public AdminAuditLogIdParameter(string identity) : base(identity)
		{
		}

		public AdminAuditLogIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AdminAuditLogIdParameter(AdminAuditLogConfig adminAuditLogConfig) : base(adminAuditLogConfig.Id)
		{
		}

		public AdminAuditLogIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		public static AdminAuditLogIdParameter Parse(string identity)
		{
			return new AdminAuditLogIdParameter(identity);
		}

		internal const string FixedValue = "Admin Audit Log Settings";
	}
}
