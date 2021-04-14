using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MailboxComplianceConfiguration", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxComplianceConfiguration : GetRecipientBase<MailboxIdParameter, ADUser>
	{
		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return RecipientConstants.GetMailboxOrSyncMailbox_AllowedRecipientTypeDetails;
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetMailboxComplianceConfiguration.SortPropertiesArray;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return new MailboxSchema();
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			if (((ADUser)dataObject).ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				base.WriteError(new InvalidOperationException(Strings.NotSupportedForPre14Mailbox(ExchangeObjectVersion.Exchange2010.ToString(), this.Identity.ToString(), ((ADUser)dataObject).ExchangeVersion.ToString())), ErrorCategory.InvalidOperation, this.Identity);
			}
			IConfigurable result;
			using (MailboxSession mailboxSession = ELCTaskHelper.OpenMailboxSession((ADUser)dataObject, "Client=Management;Action=Get-MailboxComplianceConfiguration", new Task.TaskErrorLoggingDelegate(base.WriteError)))
			{
				if (mailboxSession == null)
				{
					base.WriteError(new TaskException(Strings.ErrorNonExchangeUserError(this.Identity.ToString())), ErrorCategory.NotSpecified, null);
				}
				result = new MailboxComplianceConfiguration(mailboxSession)
				{
					Identity = dataObject.Identity,
					OrganizationId = ((ADUser)dataObject).OrganizationId
				};
			}
			return result;
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			MailEnabledRecipientSchema.DisplayName,
			MailEnabledRecipientSchema.Alias,
			MailboxSchema.Database,
			MailboxSchema.ServerLegacyDN
		};
	}
}
