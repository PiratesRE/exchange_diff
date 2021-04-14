using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.SendAsDefaults;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SendAddressDataProvider : IConfigDataProvider
	{
		public SendAddressDataProvider(ExchangePrincipal userPrincipal, string mailboxIdParameterString)
		{
			SyncUtilities.ThrowIfArgumentNull("userPrincipal", userPrincipal);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxIdParameterString", mailboxIdParameterString);
			this.userPrincipal = userPrincipal;
			this.mailboxIdParameterString = mailboxIdParameterString;
		}

		public string Source
		{
			get
			{
				return "SendAddressDataProvider";
			}
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			List<SendAddress> allSendAddresses = this.sendAsDefaultsManager.GetAllSendAddresses(this.mailboxIdParameterString, this.userPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.GetAllSendAsSubscriptions());
			return allSendAddresses.ToArray();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			List<SendAddress> allSendAddresses = this.sendAsDefaultsManager.GetAllSendAddresses(this.mailboxIdParameterString, this.userPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.GetAllSendAsSubscriptions());
			List<T> list = new List<T>(allSendAddresses.Count);
			foreach (IConfigurable configurable in allSendAddresses)
			{
				list.Add((T)((object)configurable));
			}
			return list;
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			SendAddressIdentity sendAddressIdentity = (SendAddressIdentity)identity;
			return this.sendAsDefaultsManager.LookUpSendAddress(sendAddressIdentity.AddressId, this.mailboxIdParameterString, this.userPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.GetAllSendAsSubscriptions());
		}

		public void Delete(IConfigurable instance)
		{
			throw new NotSupportedException("Delete: SendAddress");
		}

		public void Save(IConfigurable instance)
		{
			throw new NotSupportedException("Save: SendAddress");
		}

		private List<PimAggregationSubscription> GetAllSendAsSubscriptions()
		{
			List<PimAggregationSubscription> allSendAsSubscriptions;
			using (MailboxSession mailboxSession = SubscriptionManager.OpenMailbox(this.userPrincipal, ExchangeMailboxOpenType.AsAdministrator, SendAddressDataProvider.ClientInfoString))
			{
				allSendAsSubscriptions = SubscriptionManager.GetAllSendAsSubscriptions(mailboxSession, false);
			}
			return allSendAsSubscriptions;
		}

		private static readonly string ClientInfoString = "Client=TransportSync;Action=SendAddress";

		private readonly ExchangePrincipal userPrincipal;

		private readonly string mailboxIdParameterString;

		private SendAsDefaultsManager sendAsDefaultsManager = new SendAsDefaultsManager();
	}
}
