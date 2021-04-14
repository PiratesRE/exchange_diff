using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Cmdlet("Set", "JournalRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetJournalRule : SetSystemConfigurationObjectTask<RuleIdParameter, JournalRuleObject, TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetJournalrule(this.Identity.ToString());
			}
		}

		public SetJournalRule()
		{
			this.Name = string.Empty;
			this.Recipient = null;
			this.Scope = JournalRuleScope.Global;
			this.JournalEmailAddress = null;
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
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

		[Parameter(Mandatory = false)]
		public RecipientIdParameter JournalEmailAddress
		{
			get
			{
				return (RecipientIdParameter)base.Fields["JournalEmailAddress"];
			}
			set
			{
				base.Fields["JournalEmailAddress"] = value;
			}
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

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateConfigurationSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateConfigurationSession(sessionSettings);
		}

		internal override IRecipientSession CreateTenantGlobalCatalogSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateTenantGlobalCatalogSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateTenantGlobalCatalogSession(sessionSettings);
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn("JournalingVersioned");
			}
			if (!Utils.ValidateGccJournalRuleParameters(this, false))
			{
				return;
			}
			if (this.ExpiryDate != null && this.ExpiryDate.Value.ToUniversalTime().Date < DateTime.UtcNow.Date)
			{
				base.WriteError(new InvalidOperationException(Strings.JournalingExpiryDateAlreadyExpired), ErrorCategory.InvalidOperation, null);
				return;
			}
			this.DataObject = (TransportRule)base.GetDataObject<TransportRule>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, null, null);
			if (!this.DataObject.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && !((IDirectorySession)base.DataSession).SessionSettings.CurrentOrganizationId.Equals(this.DataObject.OrganizationId))
			{
				base.UnderscopeDataSession(this.DataObject.OrganizationId);
			}
			if (base.HasErrors)
			{
				return;
			}
			if (!Utils.IsChildOfRuleContainer(this.Identity, "JournalingVersioned"))
			{
				throw new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(RuleIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null));
			}
			JournalingRule journalingRule = null;
			try
			{
				journalingRule = (JournalingRule)JournalingRuleParser.Instance.GetRule(this.DataObject.Xml);
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
				return;
			}
			if (journalingRule.GccRuleType != GccType.None && !this.LawfulInterception)
			{
				throw new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(RuleIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null));
			}
			if (journalingRule.IsTooAdvancedToParse)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotModifyRuleDueToVersion(journalingRule.Name)), ErrorCategory.InvalidOperation, this.Name);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (base.Fields.IsModified("JournalEmailAddress") && this.JournalEmailAddress == null)
			{
				base.WriteError(new ArgumentException(Strings.NoRecipients, "JournalEmailAddress"), ErrorCategory.InvalidArgument, this.JournalEmailAddress);
				return;
			}
			if (!base.Fields.IsModified("Name") && !base.Fields.IsModified("Scope") && !base.Fields.IsModified("RecipientProperty") && !base.Fields.IsModified("JournalEmailAddress") && !base.Fields.IsModified("ExpiryDate") && !base.Fields.IsModified("FullReport"))
			{
				base.InternalProcessRecord();
				return;
			}
			ADJournalRuleStorageManager adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", base.DataSession);
			JournalingRule journalingRule;
			try
			{
				journalingRule = (JournalingRule)JournalingRuleParser.Instance.GetRule(this.DataObject.Xml);
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
				return;
			}
			JournalRuleObject journalRuleObject = new JournalRuleObject();
			try
			{
				journalRuleObject.Deserialize(journalingRule);
				journalRuleObject.Name = this.DataObject.Name;
			}
			catch (RecipientInvalidException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, this.JournalEmailAddress);
				return;
			}
			catch (JournalRuleCorruptException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.IsChanged("Name") || base.Fields.IsModified("Name"))
			{
				journalRuleObject.Name = this.Name;
			}
			SmtpAddress journalingReportNdrToSmtpAddress = JournalRuleObject.GetJournalingReportNdrToSmtpAddress(this.DataObject.OrganizationId, this.ConfigurationSession);
			if (base.Fields.IsChanged("RecipientProperty") || base.Fields.IsModified("RecipientProperty"))
			{
				if (this.Recipient != null && this.Recipient != null && this.Recipient.Value == journalingReportNdrToSmtpAddress)
				{
					base.WriteError(new InvalidOperationException(Strings.JournalingReportNdrToSameAsRecipient), ErrorCategory.InvalidOperation, null);
					return;
				}
				journalRuleObject.Recipient = this.Recipient;
			}
			if (!base.Fields.IsChanged("JournalEmailAddress"))
			{
				if (!base.Fields.IsModified("JournalEmailAddress"))
				{
					goto IL_2AE;
				}
			}
			try
			{
				bool isNonGccInDc = Datacenter.IsMultiTenancyEnabled() && !this.LawfulInterception;
				SmtpAddress smtpAddress;
				if (!JournalRuleObject.LookupAndCheckAllowedTypes(this.JournalEmailAddress, base.TenantGlobalCatalogSession, this.DataObject.OrganizationId, isNonGccInDc, out smtpAddress))
				{
					base.WriteError(new InvalidOperationException(Strings.JournalingToExternalOnly), ErrorCategory.InvalidOperation, null);
					return;
				}
				if (smtpAddress == journalingReportNdrToSmtpAddress)
				{
					this.WriteWarning(Strings.JournalingReportNdrToSameAsJournalEmailAddress);
				}
				journalRuleObject.JournalEmailAddress = smtpAddress;
			}
			catch (RecipientInvalidException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidArgument, "JournalEmailAddress");
				return;
			}
			IL_2AE:
			if (base.Fields.IsChanged("Scope") || base.Fields.IsModified("Scope"))
			{
				journalRuleObject.Scope = this.Scope;
			}
			if (base.Fields.IsChanged("ExpiryDate") || base.Fields.IsModified("ExpiryDate"))
			{
				if (this.ExpiryDate != null)
				{
					journalRuleObject.ExpiryDate = new DateTime?(this.ExpiryDate.Value.ToUniversalTime());
				}
				else
				{
					journalRuleObject.ExpiryDate = null;
				}
			}
			if (base.Fields.IsChanged("FullReport") || base.Fields.IsModified("FullReport"))
			{
				GccType gccRuleType = GccType.None;
				if (this.LawfulInterception)
				{
					if (this.FullReport)
					{
						gccRuleType = GccType.Full;
					}
					else
					{
						gccRuleType = GccType.Prtt;
					}
				}
				journalRuleObject.RuleType = JournalRuleObject.ConvertGccTypeToJournalRuleType(gccRuleType);
			}
			journalingRule = journalRuleObject.Serialize();
			this.DataObject.Xml = JournalingRuleSerializer.Instance.SaveRuleToString(journalingRule);
			if (!adjournalRuleStorageManager.CanRename((ADObjectId)this.DataObject.Identity, this.DataObject.Name, journalingRule.Name))
			{
				base.WriteError(new ArgumentException(Strings.RuleNameAlreadyExist, "Name"), ErrorCategory.InvalidArgument, this.Name);
				return;
			}
			if (Utils.Exchange12HubServersExist(this))
			{
				this.WriteWarning(Strings.SetRuleSyncAcrossDifferentVersionsNeeded);
			}
			this.DataObject.Name = journalingRule.Name;
			base.InternalProcessRecord();
			if (journalingReportNdrToSmtpAddress == SmtpAddress.NullReversePath)
			{
				this.WriteWarning(Strings.JournalingReportNdrToNotSet);
			}
			return;
		}

		internal const string NameProperty = "Name";

		internal const string ScopeProperty = "Scope";

		internal const string RecipientProperty = "RecipientProperty";

		internal const string JournalEmailAddressProperty = "JournalEmailAddress";

		internal const string LawfulInterceptionProperty = "LawfulInterception";

		internal const string FullReportProperty = "FullReport";

		internal const string ExpiryDateProperty = "ExpiryDate";

		internal const string OrganizationProperty = "Organization";
	}
}
