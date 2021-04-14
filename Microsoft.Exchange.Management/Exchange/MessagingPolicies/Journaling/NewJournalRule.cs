using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Cmdlet("new", "journalrule", SupportsShouldProcess = true)]
	public class NewJournalRule : NewMultitenancySystemConfigurationObjectTask<TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewJournalrule(base.Name.ToString(), this.JournalEmailAddress.ToString());
			}
		}

		public NewJournalRule()
		{
			this.Scope = JournalRuleScope.Global;
			this.Recipient = null;
			this.Enabled = false;
			this.ExpiryDate = null;
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = false)]
		public JournalRuleScope Scope
		{
			get
			{
				return (JournalRuleScope)base.Fields["Scope"];
			}
			set
			{
				base.Fields["Scope"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress? Recipient
		{
			get
			{
				return (SmtpAddress?)base.Fields["RecipientProperty"];
			}
			set
			{
				base.Fields["RecipientProperty"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public RecipientIdParameter JournalEmailAddress
		{
			get
			{
				return (RecipientIdParameter)base.Fields[JournalRuleObjectSchema.JournalEmailAddress];
			}
			set
			{
				base.Fields[JournalRuleObjectSchema.JournalEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields[JournalRuleObjectSchema.Enabled];
			}
			set
			{
				base.Fields[JournalRuleObjectSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter LawfulInterception
		{
			get
			{
				return (SwitchParameter)(base.Fields["LawfulInterception"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["LawfulInterception"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FullReport
		{
			get
			{
				return (bool)base.Fields["FullReport"];
			}
			set
			{
				base.Fields["FullReport"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? ExpiryDate
		{
			get
			{
				return (DateTime?)base.Fields["ExpiryDate"];
			}
			set
			{
				base.Fields["ExpiryDate"] = value;
			}
		}

		protected override void InternalValidate()
		{
			this.DataObject = (TransportRule)this.PrepareDataObject();
			if (!Utils.ValidateGccJournalRuleParameters(this, true))
			{
				return;
			}
			if (this.ExpiryDate != null && this.ExpiryDate.Value.ToUniversalTime().Date < DateTime.UtcNow.Date)
			{
				base.WriteError(new InvalidOperationException(Strings.JournalingExpiryDateAlreadyExpired), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			ADJournalRuleStorageManager adjournalRuleStorageManager = null;
			try
			{
				adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", base.DataSession);
			}
			catch (RuleCollectionNotInAdException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				return;
			}
			int num = 0;
			TransportRule transportRule = null;
			SmtpAddress journalingReportNdrToSmtpAddress = JournalRuleObject.GetJournalingReportNdrToSmtpAddress(this.ResolveCurrentOrganization(), this.ConfigurationSession);
			JournalRuleObject journalRuleObject;
			try
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled && journalingReportNdrToSmtpAddress == SmtpAddress.NullReversePath)
				{
					base.WriteError(new InvalidOperationException(Strings.JournalNdrMailboxCannotBeNull), ErrorCategory.InvalidOperation, null);
				}
				bool flag = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled && !this.LawfulInterception;
				SmtpAddress smtpAddress;
				if (!JournalRuleObject.LookupAndCheckAllowedTypes(this.JournalEmailAddress, base.TenantGlobalCatalogSession, base.SessionSettings.CurrentOrganizationId, flag, out smtpAddress))
				{
					base.WriteError(new InvalidOperationException(Strings.JournalingToExternalOnly), ErrorCategory.InvalidOperation, null);
					return;
				}
				if (smtpAddress == journalingReportNdrToSmtpAddress)
				{
					this.WriteWarning(Strings.JournalingReportNdrToSameAsJournalEmailAddress);
				}
				if (flag)
				{
					adjournalRuleStorageManager.LoadRuleCollection();
					if (adjournalRuleStorageManager.Count >= 10)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorTooManyJournalRules(10)), ErrorCategory.InvalidOperation, null);
						return;
					}
				}
				if (this.Recipient != null && this.Recipient != null && this.Recipient.Value == journalingReportNdrToSmtpAddress)
				{
					base.WriteError(new InvalidOperationException(Strings.JournalingReportNdrToSameAsRecipient), ErrorCategory.InvalidOperation, null);
					return;
				}
				GccType gccRuleType = GccType.None;
				if (this.LawfulInterception)
				{
					this.ValidateLawfulInterceptionTenantConfiguration();
					if ((base.Fields.IsChanged("FullReport") || base.Fields.IsModified("FullReport")) && this.FullReport)
					{
						gccRuleType = GccType.Full;
					}
					else
					{
						gccRuleType = GccType.Prtt;
					}
				}
				DateTime? expiryDate = null;
				if (this.ExpiryDate != null)
				{
					expiryDate = new DateTime?(this.ExpiryDate.Value.ToUniversalTime());
				}
				journalRuleObject = new JournalRuleObject(base.Name, this.Enabled, this.Recipient, smtpAddress, this.Scope, expiryDate, gccRuleType);
			}
			catch (DataValidationException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, base.Name);
				return;
			}
			catch (RecipientInvalidException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, this.JournalEmailAddress);
				return;
			}
			try
			{
				JournalingRule rule = journalRuleObject.Serialize();
				adjournalRuleStorageManager.NewRule(rule, this.ResolveCurrentOrganization(), ref num, out transportRule);
			}
			catch (RulesValidationException)
			{
				base.WriteError(new ArgumentException(Strings.RuleNameAlreadyExists, "Name"), ErrorCategory.InvalidArgument, base.Name);
				return;
			}
			catch (ParserException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidData, null);
				return;
			}
			if (Utils.Exchange12HubServersExist(this))
			{
				this.WriteWarning(Strings.NewRuleSyncAcrossDifferentVersionsNeeded);
			}
			journalRuleObject.SetTransportRule(transportRule);
			if (journalingReportNdrToSmtpAddress == SmtpAddress.NullReversePath)
			{
				this.WriteWarning(Strings.JournalingReportNdrToNotSet);
			}
			base.WriteObject(journalRuleObject);
		}

		private void ValidateLawfulInterceptionTenantConfiguration()
		{
			string config = JournalConfigSchema.Configuration.GetConfig<string>("LegalInterceptTenantName");
			if (string.IsNullOrEmpty(config))
			{
				base.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorGccTenantSettingNotExist), ErrorCategory.ObjectNotFound, null);
			}
			Guid lawfulInterceptTenantGuid = ADJournalRuleStorageManager.GetLawfulInterceptTenantGuid(config);
			if (lawfulInterceptTenantGuid == Guid.Empty)
			{
				base.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorGccTenantNotFound(config)), ErrorCategory.ObjectNotFound, null);
			}
		}

		internal const int DatacenterMaxJournalRules = 10;
	}
}
