using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "EmailAddressPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetEmailAddressPolicy : GetMultitenancySystemConfigurationObjectTask<EmailAddressPolicyIdParameter, EmailAddressPolicy>
	{
		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.IncludeMailboxSettingOnlyPolicy.IsPresent)
				{
					return null;
				}
				return new ComparisonFilter(ComparisonOperator.Equal, EmailAddressPolicySchema.PolicyOptionListValue, EmailAddressPolicy.PolicyGuid.ToByteArray());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeMailboxSettingOnlyPolicy
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeMailboxSettingOnlyPolicy"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeMailboxSettingOnlyPolicy"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity == null)
				{
					return base.CurrentOrgContainerId.GetDescendantId(EmailAddressPolicy.RdnEapContainerToOrganization);
				}
				return null;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			EmailAddressPolicy emailAddressPolicy = (EmailAddressPolicy)dataObject;
			if (!this.IncludeMailboxSettingOnlyPolicy.IsPresent && !emailAddressPolicy.HasEmailAddressSetting)
			{
				this.mailboxSettingOnlyPolicyIgnored = true;
				TaskLogger.LogExit();
				return;
			}
			OrganizationId organizationId = emailAddressPolicy.OrganizationId;
			if (this.domainValidator == null || !this.domainValidator.OrganizationId.Equals(organizationId))
			{
				this.domainValidator = new UpdateEmailAddressPolicy.ReadOnlyDomainValidator(organizationId, this.ConfigurationSession, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			this.domainValidator.Validate(emailAddressPolicy);
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.mailboxSettingOnlyPolicyIgnored = false;
			base.InternalProcessRecord();
			if (this.Identity == null && !this.IncludeMailboxSettingOnlyPolicy.IsPresent)
			{
				try
				{
					this.mailboxSettingOnlyPolicyIgnored = true;
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.NotEqual, EmailAddressPolicySchema.PolicyOptionListValue, EmailAddressPolicy.PolicyGuid.ToByteArray());
					base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(EmailAddressPolicy), filter, this.RootId, this.DeepSearch));
					EmailAddressPolicy[] array = (base.DataSession as IConfigurationSession).Find<EmailAddressPolicy>((ADObjectId)this.RootId, this.DeepSearch ? QueryScope.SubTree : QueryScope.OneLevel, filter, null, 1);
					this.mailboxSettingOnlyPolicyIgnored = (0 != array.Length);
				}
				catch (DataSourceTransientException ex)
				{
					base.WriteVerbose(ex.LocalizedString);
				}
				catch (DataSourceOperationException ex2)
				{
					base.WriteVerbose(ex2.LocalizedString);
				}
			}
			if (this.mailboxSettingOnlyPolicyIgnored)
			{
				this.WriteWarning(Strings.WarningIgnoreMailboxSettingOnlyPolicy);
			}
			TaskLogger.LogExit();
		}

		private bool mailboxSettingOnlyPolicyIgnored;

		private UpdateEmailAddressPolicy.ReadOnlyDomainValidator domainValidator;
	}
}
