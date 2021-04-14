using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class NewOrganizationServerSettingsModule : RunspaceServerSettingsInitModule
	{
		private SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)this.fields["TenantDomainName"];
			}
			set
			{
				this.fields["TenantDomainName"] = value;
			}
		}

		private string Name
		{
			get
			{
				return (string)this.fields["TenantName"];
			}
			set
			{
				LocalizedString localizedString;
				this.fields["TenantName"] = MailboxTaskHelper.GetNameOfAcceptableLengthForMultiTenantMode(value, out localizedString);
				if (localizedString != LocalizedString.Empty)
				{
					base.CurrentTaskContext.CommandShell.WriteWarning(localizedString);
				}
			}
		}

		public NewOrganizationServerSettingsModule(TaskContext context) : base(context)
		{
		}

		protected override ADServerSettings GetCmdletADServerSettings()
		{
			this.fields = base.CurrentTaskContext.InvocationInfo.Fields;
			SwitchParameter switchParameter = this.fields.Contains("IsDatacenter") ? ((SwitchParameter)this.fields["IsDatacenter"]) : new SwitchParameter(false);
			if (!this.fields.Contains(ManageOrganizationTaskBase.ParameterCreateSharedConfig))
			{
				new SwitchParameter(false);
			}
			else
			{
				SwitchParameter switchParameter2 = (SwitchParameter)this.fields[ManageOrganizationTaskBase.ParameterCreateSharedConfig];
			}
			string text = (string)this.fields["TenantProgramId"];
			string text2 = (string)this.fields["TenantOfferId"];
			AccountPartitionIdParameter accountPartitionIdParameter = (AccountPartitionIdParameter)this.fields["AccountPartition"];
			string value = null;
			if (TopologyProvider.CurrentTopologyMode == TopologyMode.ADTopologyService && switchParameter)
			{
				ADServerSettings serverSettings = ExchangePropertyContainer.GetServerSettings(base.CurrentTaskContext.SessionState);
				if (serverSettings != null && accountPartitionIdParameter != null)
				{
					PartitionId partitionId = RecipientTaskHelper.ResolvePartitionId(accountPartitionIdParameter, null);
					if (partitionId != null)
					{
						value = serverSettings.PreferredGlobalCatalog(partitionId.ForestFQDN);
					}
				}
				if (string.IsNullOrEmpty(value) && this.Name != null)
				{
					if (this.domainBasedADServerSettings == null)
					{
						PartitionId partitionId2 = (accountPartitionIdParameter == null) ? PartitionId.LocalForest : RecipientTaskHelper.ResolvePartitionId(accountPartitionIdParameter, null);
						this.domainBasedADServerSettings = RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(this.Name.ToLowerInvariant(), partitionId2.ForestFQDN, false);
					}
					return this.domainBasedADServerSettings;
				}
			}
			return base.GetCmdletADServerSettings();
		}

		private ADServerSettings domainBasedADServerSettings;

		private PropertyBag fields;
	}
}
