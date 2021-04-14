using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "RuleCollection")]
	public sealed class InstallRuleCollection : NewMultitenancySystemConfigurationObjectTask<TransportRuleCollection>
	{
		protected override IConfigurable PrepareDataObject()
		{
			TransportRuleCollection transportRuleCollection = (TransportRuleCollection)base.PrepareDataObject();
			ADObjectId adobjectId = base.CurrentOrgContainerId;
			adobjectId = adobjectId.GetChildId("Transport Settings");
			adobjectId = adobjectId.GetChildId("Rules");
			adobjectId = adobjectId.GetChildId(base.Name);
			transportRuleCollection.SetId(adobjectId);
			return transportRuleCollection;
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataSession.Read<TransportRuleCollection>(this.DataObject.Id) == null)
			{
				base.InternalProcessRecord();
				return;
			}
			base.WriteVerbose(Strings.RuleCollectionAlreadyExists(base.Name));
		}
	}
}
