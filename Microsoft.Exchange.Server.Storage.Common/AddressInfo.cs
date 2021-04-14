using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class AddressInfo
	{
		public Guid ObjectId { get; private set; }

		public Guid ADCachingUseOnlyMailboxGuid { get; private set; }

		public string DisplayName { get; private set; }

		public string SimpleDisplayName { get; private set; }

		public SecurityIdentifier UserSid { get; private set; }

		public SecurityIdentifier[] UserSidHistory { get; private set; }

		public SecurityIdentifier MasterAccountSid { get; private set; }

		public string LegacyExchangeDN { get; private set; }

		public string DistinguishedName { get; private set; }

		public string OriginalEmailAddress { get; private set; }

		public string OriginalEmailAddressType { get; private set; }

		public string PrimaryEmailAddress { get; private set; }

		public string PrimaryEmailAddressType { get; private set; }

		public string PrimarySmtpAddress { get; private set; }

		public bool IsDistributionList { get; private set; }

		public bool IsMailPublicFolder { get; private set; }

		public SecurityDescriptor OSSecurityDescriptor { get; private set; }

		public IList<AddressInfo.PublicDelegate> PublicDelegates { get; private set; }

		public bool HasMailbox { get; private set; }

		public AddressInfo(Guid objectId, Guid cachingUseOnlyMailboxGuid, string displayName, string simpleDisplayName, SecurityIdentifier masterAccountSid, string legacyExchangeDN, string distinguishedName, SecurityIdentifier userSid, SecurityIdentifier[] userSidHistory, string userOrgEmailAddr, string userOrgAddrType, string userEmailAddress, string userAddressType, string primarySmtpAddress, bool isDistributionList, bool isMailPublicFolder, SecurityDescriptor osSecurityDescriptor, IList<AddressInfo.PublicDelegate> publicDelegates, bool hasMailbox)
		{
			this.ObjectId = objectId;
			this.ADCachingUseOnlyMailboxGuid = cachingUseOnlyMailboxGuid;
			this.DisplayName = displayName;
			this.MasterAccountSid = masterAccountSid;
			this.LegacyExchangeDN = legacyExchangeDN;
			this.DistinguishedName = distinguishedName;
			this.UserSid = userSid;
			this.UserSidHistory = userSidHistory;
			if (!string.IsNullOrEmpty(simpleDisplayName))
			{
				this.SimpleDisplayName = simpleDisplayName;
			}
			else if (!string.IsNullOrEmpty(displayName))
			{
				this.SimpleDisplayName = displayName;
			}
			else
			{
				this.SimpleDisplayName = userEmailAddress;
			}
			if (!string.IsNullOrEmpty(userOrgEmailAddr))
			{
				this.OriginalEmailAddress = userOrgEmailAddr;
				this.OriginalEmailAddressType = userOrgAddrType;
			}
			if (!string.IsNullOrEmpty(userEmailAddress))
			{
				this.PrimaryEmailAddress = userEmailAddress;
				this.PrimaryEmailAddressType = userAddressType;
			}
			if (!string.IsNullOrEmpty(primarySmtpAddress))
			{
				this.PrimarySmtpAddress = primarySmtpAddress;
			}
			this.IsDistributionList = isDistributionList;
			this.IsMailPublicFolder = isMailPublicFolder;
			this.OSSecurityDescriptor = osSecurityDescriptor;
			this.PublicDelegates = publicDelegates;
			this.HasMailbox = hasMailbox;
		}

		public struct PublicDelegate
		{
			internal PublicDelegate(string distinguishedName, Guid objectId, bool isDistributionList)
			{
				this.distinguishedName = distinguishedName;
				this.objectId = objectId;
				this.isDistributionList = isDistributionList;
			}

			public string DistinguishedName
			{
				get
				{
					return this.distinguishedName;
				}
			}

			public Guid ObjectId
			{
				get
				{
					return this.objectId;
				}
			}

			public bool IsDistributionList
			{
				get
				{
					return this.isDistributionList;
				}
			}

			private string distinguishedName;

			private Guid objectId;

			private bool isDistributionList;
		}
	}
}
