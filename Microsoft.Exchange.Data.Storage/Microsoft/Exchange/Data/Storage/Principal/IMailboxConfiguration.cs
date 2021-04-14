using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxConfiguration
	{
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
	}
}
