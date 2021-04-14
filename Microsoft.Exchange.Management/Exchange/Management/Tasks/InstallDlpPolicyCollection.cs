using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "DlpPolicyCollection")]
	public sealed class InstallDlpPolicyCollection : NewMultitenancySystemConfigurationObjectTask<ADComplianceProgramCollection>
	{
		protected override IConfigurable PrepareDataObject()
		{
			ADComplianceProgramCollection adcomplianceProgramCollection = (ADComplianceProgramCollection)base.PrepareDataObject();
			ADObjectId adobjectId = base.CurrentOrgContainerId;
			adobjectId = adobjectId.GetChildId("Transport Settings");
			adobjectId = adobjectId.GetChildId("Rules");
			adobjectId = adobjectId.GetChildId(base.Name);
			adcomplianceProgramCollection.SetId(adobjectId);
			return adcomplianceProgramCollection;
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataSession.Read<ADComplianceProgramCollection>(this.DataObject.Id) == null)
			{
				base.InternalProcessRecord();
				return;
			}
			base.WriteVerbose(Strings.RuleCollectionAlreadyExists(base.Name));
		}
	}
}
