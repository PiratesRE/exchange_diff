using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AccountPartition", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetAccountPartition : SetSystemConfigurationObjectTask<AccountPartitionIdParameter, AccountPartition>
	{
		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				return configurationSession.GetOrgContainerId().GetChildId(AccountPartition.AccountForestContainerName);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAccountPartition(this.Identity.ToString());
			}
		}

		[Parameter]
		public Fqdn Trust
		{
			get
			{
				return (Fqdn)base.Fields["Trust"];
			}
			set
			{
				base.Fields["Trust"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter EnabledForProvisioning
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnabledForProvisioning"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EnabledForProvisioning"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override void InternalValidate()
		{
			this.adTrust = null;
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.IsModified("Trust") && this.Trust != null)
			{
				this.adTrust = NewAccountPartition.ResolveAndValidateForestTrustForADDomain(this.Trust, new Task.ErrorLoggerDelegate(base.WriteError), (IConfigurationSession)base.DataSession);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (base.Fields.IsModified("Trust") && !this.Force)
			{
				if (this.adTrust != null)
				{
					if (this.DataObject.IsLocalForest)
					{
						if (!base.ShouldContinue(Strings.ConfirmationResettingLocalForestAccountPartition(this.DataObject.Name)))
						{
							return;
						}
					}
					else if (this.DataObject.TrustedDomain != null && !this.adTrust.DistinguishedName.Equals(this.DataObject.TrustedDomain.DistinguishedName, StringComparison.OrdinalIgnoreCase) && !base.ShouldContinue(Strings.ConfirmationChangePartitionTrust(this.DataObject.Name, this.DataObject.TrustedDomain.Name, this.adTrust.Name)))
					{
						return;
					}
				}
				else if (this.DataObject.TrustedDomain != null && !base.ShouldContinue(Strings.ConfirmationResettingPartitionTrust(this.DataObject.Name, this.DataObject.TrustedDomain.Name)))
				{
					return;
				}
			}
			if (base.Fields.IsModified("EnabledForProvisioning") && this.DataObject.EnabledForProvisioning && !this.EnabledForProvisioning && !this.Force && !base.ShouldContinue(Strings.ConfirmationDisablingPartitionFromProvisioning(this.DataObject.Name)))
			{
				return;
			}
			if (base.Fields.IsModified("Trust"))
			{
				this.DataObject.TrustedDomain = ((this.adTrust == null) ? null : this.adTrust.Id);
			}
			if (this.DataObject.IsLocalForest && this.adTrust != null)
			{
				this.DataObject.IsLocalForest = false;
			}
			if (base.Fields.IsModified("EnabledForProvisioning"))
			{
				this.DataObject.EnabledForProvisioning = this.EnabledForProvisioning;
			}
			base.InternalProcessRecord();
		}

		private const string EnabledForProvisioningStr = "EnabledForProvisioning";

		private ADDomainTrustInfo adTrust;
	}
}
