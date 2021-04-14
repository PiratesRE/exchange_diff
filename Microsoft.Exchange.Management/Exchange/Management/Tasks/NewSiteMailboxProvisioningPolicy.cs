using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "SiteMailboxProvisioningPolicy", SupportsShouldProcess = true)]
	public sealed class NewSiteMailboxProvisioningPolicy : NewMailboxPolicyBase<TeamMailboxProvisioningPolicy>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Static;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewTeamMailboxProvisioningPolicy(base.Name.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefault
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefault"] ?? false);
			}
			set
			{
				base.Fields["IsDefault"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MaxReceiveSize
		{
			get
			{
				return this.DataObject.MaxReceiveSize;
			}
			set
			{
				this.DataObject.MaxReceiveSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize IssueWarningQuota
		{
			get
			{
				return this.DataObject.IssueWarningQuota;
			}
			set
			{
				this.DataObject.IssueWarningQuota = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ProhibitSendReceiveQuota
		{
			get
			{
				return this.DataObject.ProhibitSendReceiveQuota;
			}
			set
			{
				this.DataObject.ProhibitSendReceiveQuota = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DefaultAliasPrefixEnabled
		{
			get
			{
				return this.DataObject.DefaultAliasPrefixEnabled;
			}
			set
			{
				this.DataObject.DefaultAliasPrefixEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AliasPrefix
		{
			get
			{
				return this.DataObject.AliasPrefix;
			}
			set
			{
				this.DataObject.AliasPrefix = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.IsDefault)
			{
				this.DataObject.IsDefault = true;
				this.existingDefaultPolicies = DefaultTeamMailboxProvisioningPolicyUtility.GetDefaultPolicies((IConfigurationSession)base.DataSession);
				if (this.existingDefaultPolicies != null && this.existingDefaultPolicies.Count > 0)
				{
					this.updateExistingDefaultPolicies = true;
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.DataObject != null && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.updateExistingDefaultPolicies)
			{
				try
				{
					DefaultMailboxPolicyUtility<TeamMailboxProvisioningPolicy>.ClearDefaultPolicies(base.DataSession as IConfigurationSession, this.existingDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
