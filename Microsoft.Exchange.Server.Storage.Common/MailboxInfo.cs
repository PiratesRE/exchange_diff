using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class MailboxInfo
	{
		public Guid MdbGuid { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public Guid? PartitionGuid { get; private set; }

		public int Lcid { get; private set; }

		public Guid OwnerGuid { get; private set; }

		public string OwnerLegacyDN { get; private set; }

		public string OwnerDisplayName { get; private set; }

		public string SimpleDisplayName { get; private set; }

		public string OwnerDistinguishedName { get; private set; }

		public bool IsMicrosoftExchangeRecipient { get; private set; }

		public bool IsSystemAttendantRecipient { get; private set; }

		public SecurityIdentifier MasterAccountSid { get; private set; }

		public SecurityIdentifier UserSid { get; private set; }

		public SecurityIdentifier[] UserSidHistory { get; private set; }

		public SecurityDescriptor ExchangeSecurityDescriptor { get; private set; }

		public bool IsDisconnected { get; set; }

		public bool IsArchiveMailbox { get; private set; }

		public bool IsDiscoveryMailbox { get; private set; }

		public bool IsSystemMailbox { get; private set; }

		public bool IsHealthMailbox { get; private set; }

		public MailboxInfo.MailboxType Type { get; private set; }

		public MailboxInfo.MailboxTypeDetail TypeDetail { get; private set; }

		public int RulesQuota { get; private set; }

		public UnlimitedBytes MaxSendSize { get; private set; }

		public UnlimitedBytes MaxReceiveSize { get; private set; }

		public QuotaStyle QuotaStyle { get; private set; }

		public UnlimitedBytes MaxMessageSize { get; private set; }

		public UnlimitedBytes MaxStreamSize { get; private set; }

		public QuotaInfo QuotaInfo { get; private set; }

		public UnlimitedBytes OrgWidePublicFolderWarningQuota { get; private set; }

		public UnlimitedBytes OrgWidePublicFolderProhibitPostQuota { get; private set; }

		public UnlimitedBytes OrgWidePublicFolderMaxItemSize { get; private set; }

		public bool IsTenantMailbox { get; private set; }

		public Guid ExternalDirectoryOrganizationId { get; private set; }

		public MailboxInfo(Guid mdbGuid, Guid mailboxGuid, Guid? partitionGuid, bool isTenantMailbox, bool isArchiveMailbox, bool isSystemMailbox, bool isHealthMailbox, bool isDiscoveryMailbox, MailboxInfo.MailboxType mailboxType, MailboxInfo.MailboxTypeDetail mailboxTypeDetail, Guid ownerGuid, string ownerLegacyDN, string ownerDisplayName, string simpleDisplayName, string ownerDistinguishedName, bool isMSExchangeRecipient, bool isSystemAttendantRecipient, SecurityIdentifier masterAccountSid, SecurityIdentifier userSid, SecurityIdentifier[] userSidHistory, SecurityDescriptor exchangeSecurityDescriptor, int lcid, int rulesQuota, UnlimitedBytes maxSendSize, UnlimitedBytes maxReceiveSize, QuotaStyle quotaStyle, QuotaInfo quotaInfo, UnlimitedBytes orgWidePublicFolderWarningQuota, UnlimitedBytes orgWidePublicFolderProhibitPostQuota, UnlimitedBytes orgWidePublicFolderMaxItemSize, Guid externalDirectoryOrganizationId)
		{
			this.MdbGuid = mdbGuid;
			this.MailboxGuid = mailboxGuid;
			this.PartitionGuid = partitionGuid;
			this.IsTenantMailbox = isTenantMailbox;
			this.IsArchiveMailbox = isArchiveMailbox;
			this.IsSystemMailbox = isSystemMailbox;
			this.IsHealthMailbox = isHealthMailbox;
			this.IsDiscoveryMailbox = isDiscoveryMailbox;
			this.Type = mailboxType;
			this.TypeDetail = mailboxTypeDetail;
			this.OwnerGuid = ownerGuid;
			this.OwnerLegacyDN = ownerLegacyDN;
			this.OwnerDisplayName = ownerDisplayName;
			this.SimpleDisplayName = simpleDisplayName;
			this.OwnerDistinguishedName = ownerDistinguishedName;
			this.IsMicrosoftExchangeRecipient = isMSExchangeRecipient;
			this.IsSystemAttendantRecipient = isSystemAttendantRecipient;
			this.MasterAccountSid = masterAccountSid;
			this.UserSid = userSid;
			this.UserSidHistory = userSidHistory;
			this.ExchangeSecurityDescriptor = exchangeSecurityDescriptor;
			this.IsDisconnected = false;
			this.Lcid = lcid;
			this.RulesQuota = ((rulesQuota > 0) ? rulesQuota : 65536);
			this.MaxSendSize = maxSendSize;
			this.MaxReceiveSize = maxReceiveSize;
			this.MaxMessageSize = maxSendSize;
			this.OrgWidePublicFolderWarningQuota = orgWidePublicFolderWarningQuota;
			this.OrgWidePublicFolderProhibitPostQuota = orgWidePublicFolderProhibitPostQuota;
			this.OrgWidePublicFolderMaxItemSize = orgWidePublicFolderMaxItemSize;
			this.ExternalDirectoryOrganizationId = externalDirectoryOrganizationId;
			this.QuotaStyle = quotaStyle;
			this.QuotaInfo = quotaInfo;
			if (maxReceiveSize > maxSendSize)
			{
				this.MaxMessageSize = maxReceiveSize;
			}
			this.MaxStreamSize = this.MaxMessageSize * 5L;
		}

		public enum MailboxType
		{
			Private,
			PublicFolderPrimary,
			PublicFolderSecondary
		}

		public enum MailboxTypeDetail
		{
			None,
			UserMailbox,
			SharedMailbox,
			TeamMailbox,
			GroupMailbox,
			AuxArchiveMailbox
		}
	}
}
