using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ExchangeAssistanceConfig")]
	public sealed class NewExchangeAssistanceConfig : NewMultitenancyFixedNameSystemConfigurationObjectTask<ExchangeAssistance>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override IConfigurable PrepareDataObject()
		{
			this.DataObject = (ExchangeAssistance)base.PrepareDataObject();
			if (!base.HasErrors)
			{
				this.DataObject.SetId(base.CurrentOrgContainerId.GetChildId("ExchangeAssistance"));
				this.DataObject.ControlPanelHelpURL = new Uri("http://help.outlook.com/140");
				this.DataObject.ControlPanelFeedbackURL = new Uri("http://go.microsoft.com/fwlink/?LinkId=89770");
				this.DataObject.ManagementConsoleHelpURL = new Uri("http://technet.microsoft.com/library(EXCHG.141)");
				this.DataObject.ManagementConsoleFeedbackURL = new Uri("http://go.microsoft.com/fwlink/?LinkId=103028");
				this.DataObject.OWAHelpURL = new Uri("http://help.outlook.com/140");
				this.DataObject.OWALightHelpURL = new Uri("http://help.outlook.com/140");
				this.DataObject.OWAFeedbackURL = new Uri("http://go.microsoft.com/fwlink/?LinkId=103030");
				this.DataObject.OWALightFeedbackURL = new Uri("http://go.microsoft.com/fwlink/?LinkId=103029");
				this.DataObject.WindowsLiveAccountPageURL = new Uri("http://go.microsoft.com/fwlink/?LinkId=91489");
				this.DataObject.PrivacyStatementURL = new Uri("http://go.microsoft.com/fwlink/?LinkId=91487");
				this.DataObject.CommunityURL = new Uri("http://go.microsoft.com/fwlink/?LinkId=178185");
			}
			return this.DataObject;
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataSession.Read<ExchangeAssistance>(this.DataObject.Id) == null)
			{
				base.InternalProcessRecord();
			}
			this.InstallCurrentVersionExchangeAssistanceAsChild();
		}

		private void InstallCurrentVersionExchangeAssistanceAsChild()
		{
			ADObjectId childId = this.DataObject.Id.GetChildId(NewExchangeAssistanceConfig.CurrentVersionContainerName);
			if (this.IsExchangeAssistanceInstalled(childId, this.DataObject.Id))
			{
				return;
			}
			ExchangeAssistance exchangeAssistance = new ExchangeAssistance();
			exchangeAssistance.ProvisionalClone(this.DataObject);
			exchangeAssistance.SetId(childId);
			exchangeAssistance.ControlPanelHelpURL = new Uri("http://technet.microsoft.com/library(EXCHG.150)");
			exchangeAssistance.ControlPanelFeedbackURL = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253080");
			exchangeAssistance.ManagementConsoleHelpURL = new Uri("http://technet.microsoft.com/library(EXCHG.150)");
			exchangeAssistance.ManagementConsoleFeedbackURL = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253081");
			exchangeAssistance.OWAHelpURL = new Uri("http://o15.officeredir.microsoft.com/r/rlidOfficeWebHelp");
			exchangeAssistance.OWALightHelpURL = new Uri("http://o15.officeredir.microsoft.com/r/rlidOfficeWebHelp");
			exchangeAssistance.OWAFeedbackURL = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253083");
			exchangeAssistance.OWALightFeedbackURL = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253087");
			exchangeAssistance.WindowsLiveAccountPageURL = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253084");
			exchangeAssistance.PrivacyStatementURL = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253085");
			exchangeAssistance.CommunityURL = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253086");
			((IConfigurationSession)base.DataSession).Save(exchangeAssistance);
		}

		private bool IsExchangeAssistanceInstalled(ADObjectId id, ADObjectId rootId)
		{
			IEnumerable<ExchangeAssistance> enumerable = base.DataSession.FindPaged<ExchangeAssistance>(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, id), rootId, true, null, 0);
			IEnumerator<ExchangeAssistance> enumerator = enumerable.GetEnumerator();
			return enumerator != null && enumerator.MoveNext();
		}

		private const string ContainerName = "ExchangeAssistance";

		public static readonly string CurrentVersionContainerName = "ExchangeAssistance" + 15;
	}
}
