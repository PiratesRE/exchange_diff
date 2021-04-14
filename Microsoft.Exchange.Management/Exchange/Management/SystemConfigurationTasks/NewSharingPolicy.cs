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
	[Cmdlet("New", "SharingPolicy", SupportsShouldProcess = true)]
	public sealed class NewSharingPolicy : NewMultitenancySystemConfigurationObjectTask<SharingPolicy>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = true)]
		public MultiValuedProperty<SharingPolicyDomain> Domains
		{
			get
			{
				return this.DataObject.Domains;
			}
			set
			{
				this.DataObject.Domains = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Default { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewSharingPolicy(base.Name, base.FormatMultiValuedProperty(this.Domains));
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			SharingPolicy sharingPolicy = (SharingPolicy)base.PrepareDataObject();
			sharingPolicy.SetId((IConfigurationSession)base.DataSession, base.Name);
			return sharingPolicy;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			if (this.Default)
			{
				SetSharingPolicy.SetDefaultSharingPolicy(this.ConfigurationSession, this.DataObject.OrganizationId, this.DataObject.Id);
			}
			if (this.DataObject.IsAllowedForAnyAnonymousFeature() && !SetSharingPolicy.IsDatacenter)
			{
				this.WriteWarning(Strings.AnonymousSharingEnabledWarning);
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable result)
		{
			SharingPolicy sharingPolicy = (SharingPolicy)result;
			sharingPolicy.Default = this.Default;
			base.WriteObject(result);
		}
	}
}
