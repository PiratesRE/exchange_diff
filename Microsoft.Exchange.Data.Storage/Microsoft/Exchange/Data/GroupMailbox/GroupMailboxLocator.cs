using System;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupMailboxLocator : MailboxLocator
	{
		public GroupMailboxLocator(IRecipientSession adSession, string externalDirectoryObjectId, string legacyDn) : base(adSession, externalDirectoryObjectId, legacyDn)
		{
		}

		private GroupMailboxLocator(IRecipientSession adSession) : base(adSession)
		{
		}

		public override string LocatorType
		{
			get
			{
				return GroupMailboxLocator.MailboxLocatorType;
			}
		}

		public static GroupMailboxLocator Instantiate(IRecipientSession adSession, ProxyAddress proxyAddress)
		{
			GroupMailboxLocator groupMailboxLocator = new GroupMailboxLocator(adSession);
			groupMailboxLocator.InitializeFromAd(proxyAddress);
			return groupMailboxLocator;
		}

		public static GroupMailboxLocator Instantiate(IRecipientSession adSession, ADUser adUser)
		{
			GroupMailboxLocator groupMailboxLocator = new GroupMailboxLocator(adSession);
			groupMailboxLocator.InitializeFromAd(adUser);
			return groupMailboxLocator;
		}

		public override bool IsValidReplicationTarget()
		{
			ADUser aduser = base.FindAdUser();
			return aduser.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox;
		}

		public ModernGroupObjectType GetGroupType()
		{
			return base.FindAdUser().ModernGroupType;
		}

		public string GetThumbnailPhoto()
		{
			ADUser aduser = base.FindAdUser();
			if (aduser.ThumbnailPhoto == null)
			{
				return string.Empty;
			}
			return Convert.ToBase64String(aduser.ThumbnailPhoto);
		}

		public string GetYammerGroupAddress()
		{
			return base.FindAdUser().YammerGroupAddress;
		}

		protected override bool IsValidAdUser(ADUser adUser)
		{
			return MailboxLocatorValidator.IsValidGroupLocator(adUser);
		}

		public static readonly string MailboxLocatorType = "Group Mailbox";
	}
}
