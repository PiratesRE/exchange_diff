using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "AdminAuditLogConfig")]
	public sealed class InstallAdminAuditLogConfig : NewMultitenancyFixedNameSystemConfigurationObjectTask<AdminAuditLogConfig>
	{
		protected override IConfigurable PrepareDataObject()
		{
			AdminAuditLogConfig adminAuditLogConfig = (AdminAuditLogConfig)base.PrepareDataObject();
			IConfigurationSession session = base.DataSession as IConfigurationSession;
			adminAuditLogConfig.SetId(session, "Admin Audit Log Settings");
			adminAuditLogConfig.AdminAuditLogCmdlets = new MultiValuedProperty<string>(new string[]
			{
				"*"
			});
			adminAuditLogConfig.AdminAuditLogParameters = new MultiValuedProperty<string>(new string[]
			{
				"*"
			});
			return adminAuditLogConfig;
		}

		protected override void InternalProcessRecord()
		{
			AdminAuditLogConfig[] array = ((IConfigurationSession)base.DataSession).Find<AdminAuditLogConfig>(null, QueryScope.SubTree, null, null, 0);
			if (array.Length == 0)
			{
				base.InternalProcessRecord();
				return;
			}
			TaskLogger.Trace("{0} config objects were found. We won't create a new one.", new object[]
			{
				array.Length
			});
		}
	}
}
