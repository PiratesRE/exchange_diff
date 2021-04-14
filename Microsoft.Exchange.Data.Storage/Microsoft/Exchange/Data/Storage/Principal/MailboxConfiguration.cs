using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxConfiguration : IMailboxConfiguration
	{
		public MailboxConfiguration(IGenericADUser adUser)
		{
			ArgumentValidator.ThrowIfNull("adUser", adUser);
			this.SharePointUrl = adUser.SharePointUrl;
			this.IsMapiEnabled = adUser.IsMapiEnabled;
			this.IsOwaEnabled = adUser.IsOwaEnabled;
			this.IsMowaEnabled = adUser.IsMowaEnabled;
			this.ThrottlingPolicy = adUser.ThrottlingPolicy;
			this.OwaMailboxPolicy = adUser.OwaMailboxPolicy;
			this.MobileDeviceMailboxPolicy = adUser.MobileDeviceMailboxPolicy;
			this.AddressBookPolicy = adUser.AddressBookPolicy;
			this.IsPersonToPersonMessagingEnabled = adUser.IsPersonToPersonMessagingEnabled;
			this.IsMachineToPersonMessagingEnabled = adUser.IsMachineToPersonMessagingEnabled;
			this.SkuCapability = adUser.SkuCapability;
			this.SkuAssigned = adUser.SkuAssigned;
			this.IsMailboxAuditEnabled = adUser.IsMailboxAuditEnabled;
			this.BypassAudit = adUser.BypassAudit;
			this.MailboxAuditLogAgeLimit = adUser.MailboxAuditLogAgeLimit;
			this.AuditAdminOperations = adUser.AuditAdminOperations;
			this.AuditDelegateOperations = adUser.AuditDelegateOperations;
			this.AuditDelegateAdminOperations = adUser.AuditDelegateAdminOperations;
			this.AuditOwnerOperations = adUser.AuditOwnerOperations;
			this.AuditLastAdminAccess = adUser.AuditLastAdminAccess;
			this.AuditLastDelegateAccess = adUser.AuditLastDelegateAccess;
			this.AuditLastExternalAccess = adUser.AuditLastExternalAccess;
		}

		public MailboxConfiguration()
		{
			this.SharePointUrl = null;
			this.IsMapiEnabled = false;
			this.IsOwaEnabled = false;
			this.IsMowaEnabled = false;
			this.ThrottlingPolicy = null;
			this.OwaMailboxPolicy = null;
			this.MobileDeviceMailboxPolicy = null;
			this.AddressBookPolicy = null;
			this.IsPersonToPersonMessagingEnabled = false;
			this.IsMachineToPersonMessagingEnabled = false;
			this.SkuCapability = null;
			this.SkuAssigned = null;
			this.IsMailboxAuditEnabled = false;
			this.BypassAudit = false;
			this.MailboxAuditLogAgeLimit = default(EnhancedTimeSpan);
			this.AuditAdminOperations = MailboxAuditOperations.None;
			this.AuditDelegateOperations = MailboxAuditOperations.None;
			this.AuditDelegateAdminOperations = MailboxAuditOperations.None;
			this.AuditOwnerOperations = MailboxAuditOperations.None;
			this.AuditLastAdminAccess = null;
			this.AuditLastDelegateAccess = null;
			this.AuditLastExternalAccess = null;
		}

		public Uri SharePointUrl { get; private set; }

		public bool IsMapiEnabled { get; private set; }

		public bool IsOwaEnabled { get; private set; }

		public bool IsMowaEnabled { get; private set; }

		public ADObjectId ThrottlingPolicy { get; private set; }

		public ADObjectId OwaMailboxPolicy { get; private set; }

		public ADObjectId MobileDeviceMailboxPolicy { get; private set; }

		public ADObjectId AddressBookPolicy { get; private set; }

		public bool IsPersonToPersonMessagingEnabled { get; private set; }

		public bool IsMachineToPersonMessagingEnabled { get; private set; }

		public Capability? SkuCapability { get; private set; }

		public bool? SkuAssigned { get; private set; }

		public bool IsMailboxAuditEnabled { get; private set; }

		public bool BypassAudit { get; private set; }

		public EnhancedTimeSpan MailboxAuditLogAgeLimit { get; private set; }

		public MailboxAuditOperations AuditAdminOperations { get; private set; }

		public MailboxAuditOperations AuditDelegateOperations { get; private set; }

		public MailboxAuditOperations AuditDelegateAdminOperations { get; private set; }

		public MailboxAuditOperations AuditOwnerOperations { get; private set; }

		public DateTime? AuditLastAdminAccess { get; private set; }

		public DateTime? AuditLastDelegateAccess { get; private set; }

		public DateTime? AuditLastExternalAccess { get; private set; }
	}
}
