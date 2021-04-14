using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "OwaMailboxPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOwaMailboxPolicy : SetMailboxPolicyBase<OwaMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOWAMailboxPolicy(this.Identity.ToString());
			}
		}

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
		public SwitchParameter DisableFacebook
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisableFacebook"] ?? false);
			}
			set
			{
				base.Fields["DisableFacebook"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.DataObject.InstantMessagingType == InstantMessagingTypeOptions.Msn && !Datacenter.IsMultiTenancyEnabled())
			{
				base.WriteError(new TaskException(Strings.ErrorMsnIsNotSupportedInEnterprise), ErrorCategory.InvalidArgument, null);
			}
			if (this.IsDefault)
			{
				this.DataObject.IsDefault = true;
				this.otherDefaultPolicies = DefaultOwaMailboxPolicyUtility.GetDefaultPolicies((IConfigurationSession)base.DataSession);
				if (this.otherDefaultPolicies.Count > 0)
				{
					this.updateOtherDefaultPolicies = true;
					return;
				}
			}
			else if (!this.IsDefault && base.Fields.IsChanged("IsDefault") && this.DataObject.IsDefault)
			{
				base.WriteError(new InvalidOperationException(Strings.ResettingIsDefaultIsNotSupported("IsDefault", "OwaMailboxPolicy")), ErrorCategory.WriteError, this.DataObject);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.updateOtherDefaultPolicies)
			{
				if (!base.ShouldContinue(Strings.ConfirmationMessageSwitchMailboxPolicy("OWAMailboxPolicy", this.Identity.ToString())))
				{
					return;
				}
				try
				{
					DefaultMailboxPolicyUtility<OwaMailboxPolicy>.ClearDefaultPolicies(base.DataSession as IConfigurationSession, this.otherDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
			if (base.Fields.IsChanged("DisableFacebook") && this.DisableFacebook)
			{
				this.DataObject.FacebookEnabled = false;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private const string DisableFacebookParam = "DisableFacebook";
	}
}
