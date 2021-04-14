using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "EmailAddressPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetEmailAddressPolicy : SetSystemConfigurationObjectTask<EmailAddressPolicyIdParameter, EmailAddressPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetEmailAddressPolicy(this.Identity.ToString());
			}
		}

		[Parameter]
		public string RecipientFilter
		{
			get
			{
				return (string)base.Fields[EmailAddressPolicySchema.RecipientFilter];
			}
			set
			{
				base.Fields[EmailAddressPolicySchema.RecipientFilter] = (value ?? string.Empty);
				MonadFilter monadFilter = new MonadFilter(value ?? string.Empty, this, ObjectSchema.GetInstance<ADRecipientProperties>());
				this.innerFilter = monadFilter.InnerFilter;
			}
		}

		[Parameter]
		public OrganizationalUnitIdParameter RecipientContainer
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["RecipientContainer"];
			}
			set
			{
				base.Fields["RecipientContainer"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ForceUpgrade
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceUpgrade"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceUpgrade"] = value;
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return base.Fields.IsModified(EmailAddressPolicySchema.RecipientFilter) || RecipientFilterHelper.IsRecipientFilterPropertiesModified(adObject, false);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			EmailAddressPolicy emailAddressPolicy = (EmailAddressPolicy)this.GetDynamicParameters();
			if (base.Fields.IsModified(EmailAddressPolicySchema.RecipientFilter) && RecipientFilterHelper.IsRecipientFilterPropertiesModified(emailAddressPolicy, false))
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorBothCustomAndPrecannedFilterSpecified, null), ErrorCategory.InvalidArgument, null);
			}
			if (emailAddressPolicy.IsModified(EmailAddressPolicySchema.EnabledPrimarySMTPAddressTemplate) && emailAddressPolicy.IsModified(EmailAddressPolicySchema.EnabledEmailAddressTemplates))
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorEnabledPrimarySmtpAndEmailAddressTemplatesSpecified, null), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			EmailAddressPolicy emailAddressPolicy = (EmailAddressPolicy)dataObject;
			bool flag = EmailAddressPolicyPriority.Lowest == emailAddressPolicy.Priority;
			if (flag)
			{
				if (this.Instance.IsChanged(ADObjectSchema.RawName) || ((EmailAddressPolicy)this.GetDynamicParameters()).IsChanged(ADObjectSchema.Name) || this.Instance.IsChanged(EmailAddressPolicySchema.Priority) || this.Instance.IsChanged(EmailAddressPolicySchema.RecipientContainer) || ((EmailAddressPolicy)this.GetDynamicParameters()).IsChanged(EmailAddressPolicySchema.RecipientContainer) || base.Fields.IsModified("RecipientContainer"))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnLowestEap(dataObject.Identity.ToString())), ErrorCategory.InvalidOperation, dataObject.Identity);
				}
				if (!emailAddressPolicy.ExchangeVersion.IsOlderThan(EmailAddressPolicySchema.RecipientFilter.VersionAdded) && (base.Fields.IsModified(EmailAddressPolicySchema.RecipientFilter) || RecipientFilterHelper.IsRecipientFilterPropertiesModified((ADObject)this.GetDynamicParameters(), false) || RecipientFilterHelper.IsRecipientFilterPropertiesModified(this.Instance, true)))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnLowestEap(dataObject.Identity.ToString())), ErrorCategory.InvalidOperation, dataObject.Identity);
				}
			}
			base.StampChangesOn(dataObject);
			if (base.Fields.IsModified(EmailAddressPolicySchema.RecipientFilter))
			{
				emailAddressPolicy.SetRecipientFilter(this.innerFilter);
			}
			if (flag && emailAddressPolicy.IsChanged(EmailAddressPolicySchema.LdapRecipientFilter) && !string.Equals(emailAddressPolicy.LdapRecipientFilter, LdapFilterBuilder.LdapFilterFromQueryFilter(EmailAddressPolicy.RecipientFilterForDefaultPolicy), StringComparison.OrdinalIgnoreCase))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidFilterForLowestEap(this.Identity.ToString(), emailAddressPolicy.RecipientFilter)), ErrorCategory.InvalidOperation, dataObject.Identity);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			OrganizationId organizationId = this.DataObject.OrganizationId;
			OrganizationalUnitIdParameter organizationalUnitIdParameter = null;
			if (base.Fields.IsModified("RecipientContainer"))
			{
				if (this.RecipientContainer == null)
				{
					this.DataObject.RecipientContainer = null;
				}
				else
				{
					organizationalUnitIdParameter = this.RecipientContainer;
				}
			}
			else if (this.DataObject.IsModified(AddressBookBaseSchema.RecipientContainer) && this.DataObject.RecipientContainer != null)
			{
				organizationalUnitIdParameter = new OrganizationalUnitIdParameter(this.DataObject.RecipientContainer);
			}
			if (organizationalUnitIdParameter != null)
			{
				if (base.GlobalConfigSession.IsInPreE14InteropMode())
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCannotSetRecipientContainer), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				}
				this.DataObject.RecipientContainer = NewAddressBookBase.GetRecipientContainer(organizationalUnitIdParameter, (IConfigurationSession)base.DataSession, organizationId, new NewAddressBookBase.GetUniqueObject(base.GetDataObject<ExchangeOrganizationalUnit>), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (this.IsObjectStateChanged() && this.DataObject.HasMailboxManagerSetting)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCanNotUpgradePolicyWithMailboxSetting(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Id);
			}
			if (this.IsObjectStateChanged() && this.DataObject.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorObjectNotManagableFromCurrentConsole(this.Identity.ToString(), this.DataObject.ExchangeVersion.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (!base.HasErrors)
			{
				if (this.DataObject.IsChanged(EmailAddressPolicySchema.Priority) && this.DataObject.Priority != 0)
				{
					UpdateEmailAddressPolicy.PreparePriorityOfEapObjects(organizationId, this.DataObject, base.DataSession, new TaskExtendedErrorLoggingDelegate(this.WriteError), out this.affectedPolicies, out this.affectedPoliciesOriginalPriority);
				}
				if (!base.HasErrors && (this.DataObject.IsChanged(EmailAddressPolicySchema.RecipientFilter) || this.DataObject.IsChanged(EmailAddressPolicySchema.Priority) || this.DataObject.IsChanged(EmailAddressPolicySchema.RawEnabledEmailAddressTemplates) || this.DataObject.IsChanged(EmailAddressPolicySchema.DisabledEmailAddressTemplates) || this.DataObject.IsChanged(EmailAddressPolicySchema.NonAuthoritativeDomains) || this.DataObject.IsChanged(EmailAddressPolicySchema.RecipientContainer)))
				{
					this.DataObject[EmailAddressPolicySchema.RecipientFilterApplied] = false;
				}
			}
			if (this.domainValidator == null || !this.domainValidator.OrganizationId.Equals(organizationId))
			{
				this.domainValidator = new UpdateEmailAddressPolicy.WritableDomainValidator(organizationId, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			this.domainValidator.Validate(this.DataObject);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (RecipientFilterHelper.FixExchange12RecipientFilterMetadata(this.DataObject, ADObjectSchema.ExchangeVersion, EmailAddressPolicySchema.PurportedSearchUI, EmailAddressPolicySchema.RecipientFilterMetadata, this.DataObject.LdapRecipientFilter))
			{
				base.WriteVerbose(Strings.WarningFixTheInvalidRecipientFilterMetadata(this.Identity.ToString()));
			}
			bool flag = this.affectedPolicies != null && this.affectedPolicies.Length > 0;
			List<EmailAddressPolicy> list = new List<EmailAddressPolicy>();
			try
			{
				if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ContinueUpgradeObjectVersion(this.DataObject.Name)))
				{
					if (this.DataObject.IsChanged(EmailAddressPolicySchema.Priority))
					{
						for (int i = 0; i < this.affectedPolicies.Length; i++)
						{
							if (flag)
							{
								base.WriteProgress(Strings.ProgressEmailAddressPolicyPreparingPriority, Strings.ProgressEmailAddressPolicyAdjustingPriority(this.affectedPolicies[i].Identity.ToString()), i * 99 / this.affectedPolicies.Length + 1);
							}
							bool recipientFilterApplied = this.affectedPolicies[i].RecipientFilterApplied;
							if (!this.affectedPolicies[i].ExchangeVersion.IsOlderThan(EmailAddressPolicySchema.RecipientFilterApplied.VersionAdded))
							{
								this.affectedPolicies[i][EmailAddressPolicySchema.RecipientFilterApplied] = false;
							}
							base.DataSession.Save(this.affectedPolicies[i]);
							if (!this.affectedPolicies[i].ExchangeVersion.IsOlderThan(EmailAddressPolicySchema.RecipientFilterApplied.VersionAdded))
							{
								this.affectedPolicies[i][EmailAddressPolicySchema.RecipientFilterApplied] = recipientFilterApplied;
							}
							this.affectedPolicies[i].Priority = this.affectedPoliciesOriginalPriority[i];
							list.Add(this.affectedPolicies[i]);
						}
					}
					base.InternalProcessRecord();
					if (!base.HasErrors)
					{
						list.Clear();
					}
				}
			}
			finally
			{
				for (int j = 0; j < list.Count; j++)
				{
					EmailAddressPolicy emailAddressPolicy = list[j];
					try
					{
						if (flag)
						{
							base.WriteProgress(Strings.ProgressEmailAddressPolicyPreparingPriority, Strings.ProgressEmailAddressPolicyRollingBackPriority(emailAddressPolicy.Identity.ToString()), j * 99 / list.Count + 1);
						}
						base.DataSession.Save(emailAddressPolicy);
					}
					catch (DataSourceTransientException)
					{
						this.WriteWarning(Strings.VerboseFailedToRollbackPriority(emailAddressPolicy.Id.ToString()));
					}
				}
			}
			TaskLogger.LogExit();
		}

		private EmailAddressPolicy[] affectedPolicies;

		private EmailAddressPolicyPriority[] affectedPoliciesOriginalPriority;

		private QueryFilter innerFilter;

		private UpdateEmailAddressPolicy.WritableDomainValidator domainValidator;
	}
}
