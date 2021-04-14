using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services
{
	internal class ModernGroupNotificationLocator
	{
		internal ModernGroupNotificationLocator(IRecipientSession adSession)
		{
			this.adSession = adSession;
		}

		internal IMemberSubscriptionItem GetMemberSubscription(MailboxSession mailboxSession, UserMailboxLocator userMailboxLocator)
		{
			return this.GetMemberSubscriptions(mailboxSession, new UserMailboxLocator[]
			{
				userMailboxLocator
			})[0];
		}

		internal IMemberSubscriptionItem[] GetMemberSubscriptions(MailboxSession mailboxSession, IEnumerable<UserMailboxLocator> userMailboxLocators)
		{
			IEnumerable<UserMailbox> userGroupRelationship = this.GetUserGroupRelationship(mailboxSession, userMailboxLocators);
			MemberSubscriptionItem[] array = new MemberSubscriptionItem[userGroupRelationship.Count<UserMailbox>()];
			int num = 0;
			foreach (UserMailbox userMailbox in userGroupRelationship)
			{
				string subscriptionId = ModernGroupNotificationLocator.GetSubscriptionId(userMailbox.Locator);
				ExDateTime lastVisitedDate = ModernGroupNotificationLocator.GetLastVisitedDate(userMailbox);
				array[num++] = new MemberSubscriptionItem(subscriptionId, lastVisitedDate);
			}
			return array;
		}

		internal void UpdateMemberSubscription(MailboxSession mailboxSession, UserMailboxLocator userMailboxLocator)
		{
			ProxyAddress proxyAddress = new SmtpProxyAddress(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), true);
			GroupMailboxLocator groupMailboxLocator = GroupMailboxLocator.Instantiate(this.adSession, proxyAddress);
			GroupMailboxAccessLayer.Execute("ModernGroupNotificationLocator.UpdateMemberSubscription", this.adSession, mailboxSession, delegate(GroupMailboxAccessLayer accessLayer)
			{
				accessLayer.SetLastVisitedDate(userMailboxLocator, groupMailboxLocator, ExDateTime.UtcNow);
			});
		}

		public static string GetSubscriptionId(IMailboxLocator mailboxLocator)
		{
			if (string.IsNullOrEmpty(mailboxLocator.ExternalId))
			{
				return mailboxLocator.LegacyDn;
			}
			return mailboxLocator.ExternalId;
		}

		private IEnumerable<UserMailbox> GetUserGroupRelationship(MailboxSession mailboxSession, IEnumerable<UserMailboxLocator> userMailboxLocators)
		{
			IEnumerable<UserMailbox> members = null;
			ProxyAddress proxyAddress = new SmtpProxyAddress(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), true);
			GroupMailboxLocator groupMailboxLocator = GroupMailboxLocator.Instantiate(this.adSession, proxyAddress);
			GroupMailboxAccessLayer.Execute("ModernGroupNotificationLocator.GetMailboxAssociation", this.adSession, mailboxSession, delegate(GroupMailboxAccessLayer accessLayer)
			{
				if (userMailboxLocators.Count<UserMailboxLocator>() > 0 && !string.IsNullOrEmpty(userMailboxLocators.First<UserMailboxLocator>().ExternalId))
				{
					members = accessLayer.GetUnseenMembers(groupMailboxLocator, userMailboxLocators);
					return;
				}
				ExWatson.SendReport(new InvalidOperationException("ModernGroupNotificationLocator - Getting unseen notification members without external id."), ReportOptions.None, null);
				members = accessLayer.GetMembers(groupMailboxLocator, userMailboxLocators, false);
			});
			return members;
		}

		private static ExDateTime GetLastVisitedDate(UserMailbox member)
		{
			ExDateTime d = default(ExDateTime);
			if (member != null)
			{
				if (member.LastVisitedDate > member.JoinDate)
				{
					d = member.LastVisitedDate;
				}
				else
				{
					d = member.JoinDate;
				}
			}
			if (d == default(ExDateTime))
			{
				d = ExDateTime.UtcNow;
			}
			return d.ToUtc();
		}

		private IRecipientSession adSession;
	}
}
