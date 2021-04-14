using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public interface IADRecipient : IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag
	{
		ADObjectId AddressBookPolicy { get; set; }

		string Alias { get; set; }

		string AssistantName { get; set; }

		MailboxAuditOperations AuditAdminOperations { get; set; }

		MailboxAuditOperations AuditDelegateAdminOperations { get; set; }

		MailboxAuditOperations AuditDelegateOperations { get; set; }

		DateTime? AuditLastAdminAccess { get; }

		DateTime? AuditLastDelegateAccess { get; }

		DateTime? AuditLastExternalAccess { get; }

		MailboxAuditOperations AuditOwnerOperations { get; set; }

		bool BypassAudit { get; set; }

		string DisplayName { get; set; }

		ADObjectId DefaultPublicFolderMailbox { get; }

		ProxyAddressCollection EmailAddresses { get; set; }

		string ExternalDirectoryObjectId { get; }

		ProxyAddress ExternalEmailAddress { get; set; }

		ADObjectId GlobalAddressListFromAddressBookPolicy { get; }

		MultiValuedProperty<ADObjectId> GrantSendOnBehalfTo { get; set; }

		string ImmutableId { get; set; }

		string ImmutableIdPartial { get; }

		bool IsMachineToPersonTextMessagingEnabled { get; }

		bool IsPersonToPersonTextMessagingEnabled { get; }

		bool IsResource { get; }

		string LegacyExchangeDN { get; set; }

		bool MailboxAuditEnabled { get; set; }

		EnhancedTimeSpan MailboxAuditLogAgeLimit { get; set; }

		bool MAPIEnabled { get; set; }

		SecurityIdentifier MasterAccountSid { get; }

		bool MOWAEnabled { get; set; }

		string Notes { get; set; }

		OrganizationId OrganizationId { get; }

		bool OWAEnabled { get; set; }

		string PhoneticCompany { get; set; }

		string PhoneticDepartment { get; set; }

		string PhoneticDisplayName { get; set; }

		string PhoneticFirstName { get; set; }

		string PhoneticLastName { get; set; }

		SmtpAddress PrimarySmtpAddress { get; set; }

		RecipientType RecipientType { get; }

		RecipientTypeDetails RecipientTypeDetails { get; }

		ADObjectId ThrottlingPolicy { get; set; }

		string UMExtension { get; }

		ADObjectId UMRecipientDialPlanId { get; set; }

		byte[] UMSpokenName { get; set; }
	}
}
