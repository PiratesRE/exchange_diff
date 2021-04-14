using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.DataClassification
{
	[Cmdlet("Install", "DataClassificationConfig")]
	public sealed class InstallDataClassificationConfig : NewMultitenancyFixedNameSystemConfigurationObjectTask<DataClassificationConfig>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			DataClassificationConfig dataClassificationConfig = (DataClassificationConfig)base.PrepareDataObject();
			dataClassificationConfig.Name = "Default Data Config";
			dataClassificationConfig.SetId(base.DataSession as IConfigurationSession, dataClassificationConfig.Name);
			TaskLogger.LogEnter();
			return dataClassificationConfig;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (OrganizationId.ForestWideOrgId.Equals(base.ExecutingUserOrganizationId) && base.Organization == null)
			{
				base.WriteError(new InvalidOperationException(Strings.DataClassificationConfigFirstOrgNotSupported), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!InstallDataClassificationConfig.DataClassificationConfigExists(base.DataSession as IConfigurationSession))
			{
				base.CreateParentContainerIfNeeded(this.DataObject);
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private static bool DataClassificationConfigExists(IConfigurationSession session)
		{
			DataClassificationConfig[] array = session.Find<DataClassificationConfig>(null, QueryScope.SubTree, null, null, 1);
			return array.Length != 0;
		}
	}
}
