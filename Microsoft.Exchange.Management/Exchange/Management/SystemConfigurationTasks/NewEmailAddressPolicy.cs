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
	[Cmdlet("New", "EmailAddressPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "SMTPTemplateWithPrecannedFilter")]
	public sealed class NewEmailAddressPolicy : NewMultitenancySystemConfigurationObjectTask<EmailAddressPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("AllTemplatesWithPrecannedFilter" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewEmailAddressPolicyAllTemplatesWithPrecannedFilter(base.Name.ToString(), this.IncludedRecipients.ToString(), base.FormatMultiValuedProperty(this.EnabledEmailAddressTemplates));
				}
				if ("AllTemplatesWithCustomFilter" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewEmailAddressPolicyAllTemplatesWithCustomFilter(base.Name.ToString(), this.RecipientFilter.ToString(), base.FormatMultiValuedProperty(this.EnabledEmailAddressTemplates));
				}
				if ("SMTPTemplateWithCustomFilter" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewEmailAddressPolicySMTPTemplateWithCustomFilter(base.Name.ToString(), this.RecipientFilter.ToString(), this.EnabledPrimarySMTPAddressTemplate.ToString());
				}
				return Strings.ConfirmationMessageNewEmailAddressPolicySMTPTemplateWithPrecannedFilter(base.Name.ToString(), this.IncludedRecipients.ToString(), this.EnabledPrimarySMTPAddressTemplate.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SMTPTemplateWithCustomFilter")]
		[Parameter(Mandatory = true, ParameterSetName = "AllTemplatesWithCustomFilter")]
		public string RecipientFilter
		{
			get
			{
				return (string)base.Fields["RecipientFilter"];
			}
			set
			{
				base.Fields["RecipientFilter"] = (value ?? string.Empty);
				MonadFilter monadFilter = new MonadFilter(value ?? string.Empty, this, ObjectSchema.GetInstance<ADRecipientProperties>());
				this.DataObject.SetRecipientFilter(monadFilter.InnerFilter);
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = true, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public WellKnownRecipientType? IncludedRecipients
		{
			get
			{
				return this.DataObject.IncludedRecipients;
			}
			set
			{
				this.DataObject.IncludedRecipients = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalDepartment
		{
			get
			{
				return this.DataObject.ConditionalDepartment;
			}
			set
			{
				this.DataObject.ConditionalDepartment = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCompany
		{
			get
			{
				return this.DataObject.ConditionalCompany;
			}
			set
			{
				this.DataObject.ConditionalCompany = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalStateOrProvince
		{
			get
			{
				return this.DataObject.ConditionalStateOrProvince;
			}
			set
			{
				this.DataObject.ConditionalStateOrProvince = value;
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

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute1
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute1;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute1 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute2
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute2;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute2 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute3
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute3;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute3 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute4
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute4;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute4 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute5
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute5;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute5 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute6
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute6;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute6 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute7
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute7;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute7 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute8
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute8;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute8 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute9
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute9;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute9 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute10
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute10;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute10 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute11
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute11;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute11 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute12
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute12;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute12 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute13
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute13;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute13 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute14
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute14;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute14 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute15
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute15;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute15 = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SMTPTemplateWithCustomFilter")]
		[Parameter(Mandatory = true, ParameterSetName = "SMTPTemplateWithPrecannedFilter")]
		[ValidateNotNullOrEmpty]
		public string EnabledPrimarySMTPAddressTemplate
		{
			get
			{
				return this.DataObject.EnabledPrimarySMTPAddressTemplate;
			}
			set
			{
				this.DataObject.EnabledPrimarySMTPAddressTemplate = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		[Parameter(Mandatory = true, ParameterSetName = "AllTemplatesWithCustomFilter")]
		public ProxyAddressTemplateCollection EnabledEmailAddressTemplates
		{
			get
			{
				return this.DataObject.EnabledEmailAddressTemplates;
			}
			set
			{
				this.DataObject.EnabledEmailAddressTemplates = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithCustomFilter")]
		[Parameter(Mandatory = false, ParameterSetName = "AllTemplatesWithPrecannedFilter")]
		public ProxyAddressTemplateCollection DisabledEmailAddressTemplates
		{
			get
			{
				return this.DataObject.DisabledEmailAddressTemplates;
			}
			set
			{
				this.DataObject.DisabledEmailAddressTemplates = value;
			}
		}

		[Parameter]
		public EmailAddressPolicyPriority Priority
		{
			get
			{
				return this.DataObject.Priority;
			}
			set
			{
				this.DataObject.Priority = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.domainValidator = new UpdateEmailAddressPolicy.WritableDomainValidator(base.OrganizationId ?? OrganizationId.ForestWideOrgId, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			this.containerId = base.CurrentOrgContainerId.GetDescendantId(EmailAddressPolicy.RdnEapContainerToOrganization);
			this.DataObject = (EmailAddressPolicy)base.PrepareDataObject();
			if (!base.HasErrors)
			{
				this.DataObject.SetId(this.containerId.GetChildId(base.Name));
				if (!this.DataObject.IsModified(EmailAddressPolicySchema.Priority))
				{
					this.DataObject.Priority = EmailAddressPolicyPriority.Lowest;
				}
				if (!base.HasErrors && (this.DataObject.Priority != 0 || !this.DataObject.IsModified(EmailAddressPolicySchema.Priority)))
				{
					UpdateEmailAddressPolicy.PreparePriorityOfEapObjects(base.OrganizationId ?? OrganizationId.ForestWideOrgId, this.DataObject, base.DataSession, new TaskExtendedErrorLoggingDelegate(this.WriteError), out this.affectedPolicies, out this.affectedPoliciesOriginalPriority);
				}
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
				else if (this.DataObject.RecipientContainer != null)
				{
					organizationalUnitIdParameter = new OrganizationalUnitIdParameter(this.DataObject.RecipientContainer);
				}
				if (organizationalUnitIdParameter != null)
				{
					if (base.GlobalConfigSession.IsInPreE14InteropMode())
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorCannotSetRecipientContainer), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					}
					this.DataObject.RecipientContainer = NewAddressBookBase.GetRecipientContainer(organizationalUnitIdParameter, (IConfigurationSession)base.DataSession, base.OrganizationId ?? OrganizationId.ForestWideOrgId, new NewAddressBookBase.GetUniqueObject(base.GetDataObject<ExchangeOrganizationalUnit>), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			RecipientFilterHelper.StampE2003FilterMetadata(this.DataObject, this.DataObject.LdapRecipientFilter, EmailAddressPolicySchema.PurportedSearchUI);
			this.domainValidator.Validate(this.DataObject);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				bool flag = this.affectedPolicies != null && this.affectedPolicies.Length > 0;
				List<EmailAddressPolicy> list = new List<EmailAddressPolicy>();
				try
				{
					for (int i = 0; i < this.affectedPolicies.Length; i++)
					{
						if (flag)
						{
							base.WriteProgress(Strings.ProgressEmailAddressPolicyPreparingPriority, Strings.ProgressEmailAddressPolicyAdjustingPriority(this.affectedPolicies[i].Identity.ToString()), i * 99 / this.affectedPolicies.Length + 1);
						}
						base.DataSession.Save(this.affectedPolicies[i]);
						this.affectedPolicies[i].Priority = this.affectedPoliciesOriginalPriority[i];
						list.Add(this.affectedPolicies[i]);
					}
					list.Clear();
				}
				finally
				{
					if (list.Count != 0)
					{
						try
						{
							base.DataSession.Delete(this.DataObject);
						}
						catch (DataSourceTransientException)
						{
							this.WriteWarning(Strings.VerboseFailedToDeleteEapWhileRollingBack(this.DataObject.Id.ToString()));
						}
					}
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
			}
			TaskLogger.LogExit();
		}

		private ADObjectId containerId;

		private EmailAddressPolicy[] affectedPolicies;

		private EmailAddressPolicyPriority[] affectedPoliciesOriginalPriority;

		private UpdateEmailAddressPolicy.WritableDomainValidator domainValidator;
	}
}
