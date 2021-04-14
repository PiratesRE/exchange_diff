using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.SendAsDefaults
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SendAsDefaultsManager
	{
		protected virtual SyncLogSession SyncLogSession
		{
			get
			{
				return CommonLoggingHelper.SyncLogSession;
			}
		}

		public List<SendAddress> GetAllSendAddresses(string mailboxIdParameterString, string outlookLiveAddress, List<PimAggregationSubscription> sendAsSubscriptions)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxIdParameterString", mailboxIdParameterString);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("outlookLiveAddress", outlookLiveAddress);
			SyncUtilities.ThrowIfArgumentNull("sendAsSubscriptions", sendAsSubscriptions);
			List<SendAddress> list = new List<SendAddress>(sendAsSubscriptions.Count + 2);
			this.AddAutomaticTo(list, mailboxIdParameterString);
			this.AddOutlookLiveSendAddressTo(list, mailboxIdParameterString, outlookLiveAddress);
			this.AddSendAsEnabledSendAddressesTo(list, mailboxIdParameterString, sendAsSubscriptions);
			list.Sort();
			return list;
		}

		public SendAddress LookUpSendAddress(string id, string mailboxIdParameterString, string outlookLiveAddress, List<PimAggregationSubscription> sendAsSubscriptions)
		{
			SyncUtilities.ThrowIfArgumentNull("id", id);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxIdParameterString", mailboxIdParameterString);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("outlookLiveAddress", outlookLiveAddress);
			SyncUtilities.ThrowIfArgumentNull("sendAsSubscriptions", sendAsSubscriptions);
			if (id.Equals(string.Empty))
			{
				this.SyncLogSession.LogVerbose((TSLID)26UL, SendAsDefaultsManager.Tracer, "Exact match to string.Empty found", new object[0]);
				return SendAddress.CreateAutomaticSendAddressFor(mailboxIdParameterString);
			}
			if (id.Equals(outlookLiveAddress))
			{
				return new SendAddress(outlookLiveAddress, outlookLiveAddress, mailboxIdParameterString);
			}
			if (!this.IsSubscriptionSendAddressId(id))
			{
				this.SyncLogSession.LogVerbose((TSLID)27UL, SendAsDefaultsManager.Tracer, "Id: {0} is not a subscription send address id, so defaulting to Automatic.", new object[]
				{
					id
				});
				return SendAddress.CreateAutomaticSendAddressFor(mailboxIdParameterString);
			}
			this.SyncLogSession.LogVerbose((TSLID)28UL, SendAsDefaultsManager.Tracer, "Searching for id in the passed in {0} subscriptions", new object[]
			{
				sendAsSubscriptions.Count
			});
			foreach (PimAggregationSubscription pimAggregationSubscription in sendAsSubscriptions)
			{
				string text = this.ExtractIdStringFrom(pimAggregationSubscription);
				if (id.Equals(text))
				{
					return new SendAddress(text, this.ExtractDisplayNameFrom(pimAggregationSubscription), mailboxIdParameterString);
				}
			}
			this.SyncLogSession.LogVerbose((TSLID)29UL, SendAsDefaultsManager.Tracer, "id: {0} did not match any send as subscriptions.  Defaulting to Automatic.", new object[]
			{
				id
			});
			return SendAddress.CreateAutomaticSendAddressFor(mailboxIdParameterString);
		}

		public bool TryParseSubscriptionSendAddressId(string sendAsDefaultValue, out Guid parsedSubscriptionSendAddressId)
		{
			SyncUtilities.ThrowIfArgumentNull("sendAsDefaultValue", sendAsDefaultValue);
			return GuidHelper.TryParseGuid(sendAsDefaultValue, out parsedSubscriptionSendAddressId);
		}

		public void SaveSettingForOutlook(string sendAsDefaultValue, MailboxSession mailboxSession)
		{
			SyncUtilities.ThrowIfArgumentNull("sendAsDefaultValue", sendAsDefaultValue);
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			int num = 0;
			bool flag = false;
			do
			{
				using (UserConfiguration userConfiguration = this.RetrieveUserConfiguration(mailboxSession))
				{
					IDictionary dictionary = userConfiguration.GetDictionary();
					string name = MailboxMessageConfigurationSchema.SendAddressDefault.Name;
					if (!dictionary.Contains(name))
					{
						dictionary.Add(name, sendAsDefaultValue);
					}
					else
					{
						dictionary[name] = sendAsDefaultValue;
					}
					try
					{
						userConfiguration.Save();
						break;
					}
					catch (ObjectExistedException)
					{
						if (flag)
						{
							throw;
						}
						flag = true;
					}
					catch (SaveConflictException)
					{
						if (num >= 5)
						{
							throw;
						}
						num++;
						flag = true;
					}
				}
			}
			while (flag);
		}

		private UserConfiguration RetrieveUserConfiguration(MailboxSession mailboxSession)
		{
			UserConfigurationManager userConfigurationManager = mailboxSession.UserConfigurationManager;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			UserConfiguration result;
			try
			{
				result = userConfigurationManager.GetFolderConfiguration("SendAsDefaults.OutlookConfiguration", UserConfigurationTypes.Dictionary, defaultFolderId);
			}
			catch (ObjectNotFoundException)
			{
				result = userConfigurationManager.CreateFolderConfiguration("SendAsDefaults.OutlookConfiguration", UserConfigurationTypes.Dictionary, defaultFolderId);
			}
			return result;
		}

		private void AddAutomaticTo(List<SendAddress> allSendAddresses, string mailboxIdParameterString)
		{
			allSendAddresses.Insert(0, SendAddress.CreateAutomaticSendAddressFor(mailboxIdParameterString));
		}

		private void AddOutlookLiveSendAddressTo(List<SendAddress> allSendAddresses, string mailboxIdParameterString, string outlookLiveAddress)
		{
			allSendAddresses.Add(new SendAddress(outlookLiveAddress, outlookLiveAddress, mailboxIdParameterString));
		}

		private void AddSendAsEnabledSendAddressesTo(List<SendAddress> allSendAddresses, string mailboxIdParameterString, List<PimAggregationSubscription> sendAsSubscriptions)
		{
			this.SyncLogSession.LogVerbose((TSLID)30UL, SendAsDefaultsManager.Tracer, "Converting {0} subscriptions into SendAddresses", new object[]
			{
				sendAsSubscriptions.Count
			});
			foreach (PimAggregationSubscription pimAggregationSubscription in sendAsSubscriptions)
			{
				string addressId = this.ExtractIdStringFrom(pimAggregationSubscription);
				SendAddress item = new SendAddress(addressId, this.ExtractDisplayNameFrom(pimAggregationSubscription), mailboxIdParameterString);
				allSendAddresses.Add(item);
			}
		}

		private string ExtractDisplayNameFrom(PimAggregationSubscription sendAsSubscription)
		{
			return sendAsSubscription.Email.ToString();
		}

		private bool IsSubscriptionSendAddressId(string sendAddressId)
		{
			Guid guid;
			return GuidHelper.TryParseGuid(sendAddressId, out guid);
		}

		private string ExtractIdStringFrom(PimAggregationSubscription subscription)
		{
			return subscription.SubscriptionGuid.ToString("D");
		}

		private const string SendAsDefaultsOutlookConfigurationName = "SendAsDefaults.OutlookConfiguration";

		private static readonly Trace Tracer = ExTraceGlobals.SendAsTracer;
	}
}
