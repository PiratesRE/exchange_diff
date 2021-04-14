using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "OutlookProvider", SupportsShouldProcess = true)]
	public sealed class NewOutlookProviderTask : NewMultitenancySystemConfigurationObjectTask<OutlookProvider>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOutlookProvider(base.Name.ToString());
			}
		}

		public NewOutlookProviderTask()
		{
			this.DataObject.InitializeDefaults();
		}

		protected override ObjectId RootId
		{
			get
			{
				return OutlookProvider.GetParentContainer(base.DataSession as ITopologyConfigurationSession);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			OutlookProvider outlookProvider = (OutlookProvider)base.PrepareDataObject();
			ADObjectId parentContainer = OutlookProvider.GetParentContainer(base.DataSession as ITopologyConfigurationSession);
			outlookProvider.SetId(parentContainer.GetChildId(base.Name));
			return outlookProvider;
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataSession.Read<OutlookProvider>(this.DataObject.Id) == null)
			{
				base.InternalProcessRecord();
				return;
			}
			this.WriteWarning(Strings.OutlookProviderAlreadyExists(base.Name.ToString()));
		}
	}
}
