using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IGenericADUser : IFederatedIdentityParameters
	{
		string DisplayName { get; }

		string UserPrincipalName { get; }

		string LegacyDn { get; }

		string Alias { get; }

		ADObjectId DefaultPublicFolderMailbox { get; }

		SecurityIdentifier Sid { get; }

		SecurityIdentifier MasterAccountSid { get; }

		IEnumerable<SecurityIdentifier> SidHistory { get; }

		IEnumerable<ADObjectId> GrantSendOnBehalfTo { get; }

		IEnumerable<CultureInfo> Languages { get; }

		RecipientType RecipientType { get; }

		RecipientTypeDetails RecipientTypeDetails { get; }

		bool? IsResource { get; }

		SmtpAddress PrimarySmtpAddress { get; }

		ProxyAddress ExternalEmailAddress { get; }

		ProxyAddressCollection EmailAddresses { get; }

		Guid MailboxGuid { get; }

		ADObjectId MailboxDatabase { get; }

		DateTime? WhenMailboxCreated { get; }

		NetID NetId { get; }

		ModernGroupObjectType ModernGroupType { get; }

		IEnumerable<SecurityIdentifier> PublicToGroupSids { get; }

		string ExternalDirectoryObjectId { get; }

		ADObjectId ArchiveDatabase { get; }

		Guid ArchiveGuid { get; }

		IEnumerable<string> ArchiveName { get; }

		ArchiveState ArchiveState { get; }

		ArchiveStatusFlags ArchiveStatus { get; }

		SmtpDomain ArchiveDomain { get; }

		IEnumerable<Guid> AggregatedMailboxGuids { get; }

		IEnumerable<IMailboxLocationInfo> MailboxLocations { get; }

		Uri SharePointUrl { get; }

		bool IsMapiEnabled { get; }

		bool IsOwaEnabled { get; }

		bool IsMowaEnabled { get; }

		ADObjectId ThrottlingPolicy { get; }

		ADObjectId OwaMailboxPolicy { get; }

		ADObjectId MobileDeviceMailboxPolicy { get; }

		ADObjectId AddressBookPolicy { get; }

		bool IsPersonToPersonMessagingEnabled { get; }

		bool IsMachineToPersonMessagingEnabled { get; }

		Capability? SkuCapability { get; }

		bool? SkuAssigned { get; }

		bool IsMailboxAuditEnabled { get; }

		bool BypassAudit { get; }

		EnhancedTimeSpan MailboxAuditLogAgeLimit { get; }

		MailboxAuditOperations AuditAdminOperations { get; }

		MailboxAuditOperations AuditDelegateOperations { get; }

		MailboxAuditOperations AuditDelegateAdminOperations { get; }

		MailboxAuditOperations AuditOwnerOperations { get; }

		DateTime? AuditLastAdminAccess { get; }

		DateTime? AuditLastDelegateAccess { get; }

		DateTime? AuditLastExternalAccess { get; }

		ADObjectId QueryBaseDN { get; }

		SmtpAddress GetFederatedSmtpAddress();

		FederatedIdentity GetFederatedIdentity();
	}
}
