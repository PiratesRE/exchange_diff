using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Test", "IRMConfiguration", SupportsShouldProcess = true)]
	public sealed class TestIRMConfiguration : GetMultitenancySingletonSystemConfigurationObjectTask<IRMConfiguration>
	{
		[ValidateCount(1, 100)]
		[ValidateNotNullOrEmpty]
		[Parameter]
		public SmtpAddress[] Recipient
		{
			get
			{
				return (SmtpAddress[])base.Fields["Recipient"];
			}
			set
			{
				base.Fields["Recipient"] = value;
			}
		}

		[Parameter]
		public SmtpAddress? Sender
		{
			get
			{
				return (SmtpAddress?)base.Fields["Sender"];
			}
			set
			{
				base.Fields["Sender"] = value;
			}
		}

		[Parameter]
		public SwitchParameter RMSOnline
		{
			get
			{
				return (SwitchParameter)(base.Fields["RMSOnline"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RMSOnline"] = value;
			}
		}

		[Parameter]
		public Guid RMSOnlineOrgOverride
		{
			get
			{
				return (Guid)(base.Fields["RMSOnlineOrgOverride"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["RMSOnlineOrgOverride"] = value;
			}
		}

		[Parameter]
		public string RMSOnlineAuthCertSubjectNameOverride
		{
			get
			{
				return (string)base.Fields["RMSOnlineAuthCertSubjectNameOverride"];
			}
			set
			{
				base.Fields["RMSOnlineAuthCertSubjectNameOverride"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestIRMConfiguration;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			this.ThrowIfBothSenderAndRmsOnlineParametersSpecified();
			this.ThrowIfNeitherSenderAndRmsOnlineParametersSpecified();
			this.ThrowIfCurrentOrganizationIdIsNull();
			if (this.RMSOnline)
			{
				RMSOnlineValidator rmsonlineValidator = new RMSOnlineValidator(this.ConfigurationSession, (IConfigurationSession)base.DataSession, base.CurrentOrganizationId, this.RMSOnlineOrgOverride, this.RMSOnlineAuthCertSubjectNameOverride);
				this.WriteResult(rmsonlineValidator.Validate());
				return;
			}
			RmsClientManager.ADSession = this.ConfigurationSession;
			IRMConfigurationValidator irmconfigurationValidator = new IRMConfigurationValidator(new RmsClientManagerContext(base.CurrentOrganizationId, null), this.Sender.Value, this.Recipient);
			IRMConfigurationValidationResult dataObject = irmconfigurationValidator.Validate();
			RmsClientManager.ADSession = null;
			this.WriteResult(dataObject);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			IRMConfigurationValidationResult irmconfigurationValidationResult = dataObject as IRMConfigurationValidationResult;
			if (irmconfigurationValidationResult != null)
			{
				base.WriteResult(irmconfigurationValidationResult);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception);
		}

		private void ThrowIfBothSenderAndRmsOnlineParametersSpecified()
		{
			if (this.Sender != null && this.RMSOnline)
			{
				base.WriteError(new SenderAndRmsOnlineParametersCannotBeCombinedException(), ErrorCategory.InvalidOperation, null);
			}
		}

		private void ThrowIfNeitherSenderAndRmsOnlineParametersSpecified()
		{
			if (this.Sender == null && !this.RMSOnline)
			{
				base.WriteError(new EitherSenderOrRmsOnlineParametersMustBeSpecifiedException(), ErrorCategory.InvalidOperation, null);
			}
		}

		private void ThrowIfCurrentOrganizationIdIsNull()
		{
			if (base.CurrentOrganizationId == null)
			{
				base.WriteError(new NullOrganizationIdException(), ErrorCategory.InvalidOperation, null);
			}
		}
	}
}
