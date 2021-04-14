using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Engine;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class NonIndexableItemStatisticsProvider : NonIndexableItemProvider
	{
		public NonIndexableItemStatisticsProvider(IRecipientSession recipientSession, ExTimeZone timeZone, CallerInfo callerInfo, OrganizationId orgId, string[] mailboxes, bool searchArchiveOnly) : base(recipientSession, timeZone, callerInfo, orgId, mailboxes, searchArchiveOnly)
		{
			this.Results = new List<NonIndexableItemStatisticsInfo>(mailboxes.Length);
		}

		public List<NonIndexableItemStatisticsInfo> Results { get; private set; }

		protected override void InternalExecuteSearch()
		{
			List<string> list = new List<string>(1);
			List<ADRawEntry> list2 = NonIndexableItemProvider.FindMailboxesInAD(this.recipientSession, this.mailboxes, list);
			foreach (ADRawEntry adrawEntry in list2)
			{
				string text = (string)adrawEntry[ADRecipientSchema.LegacyExchangeDN];
				try
				{
					if (!this.searchArchiveOnly)
					{
						this.RetrieveFailedItemsCount(adrawEntry, MailboxType.Primary);
					}
					if (!this.alreadyProxy && (Guid)adrawEntry[ADUserSchema.ArchiveGuid] != Guid.Empty)
					{
						this.RetrieveFailedItemsCount(adrawEntry, MailboxType.Archive);
					}
				}
				catch (Exception ex)
				{
					Factory.Current.GeneralTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Failed to retrieve failed items count for {1}", this.callerInfo.QueryCorrelationId, text);
					base.AddFailedMailbox(text, ex.Message);
					this.UpdateResults(text, 0);
				}
			}
			foreach (string text2 in list)
			{
				base.AddFailedMailbox(text2, Strings.SourceMailboxUserNotFoundInAD(text2));
				this.UpdateResults(text2, 0);
			}
		}

		protected override void InternalExecuteSearchWebService()
		{
			GetNonIndexableItemStatisticsParameters parameters = new GetNonIndexableItemStatisticsParameters
			{
				Mailboxes = new string[]
				{
					this.mailboxInfo.LegacyExchangeDN
				},
				SearchArchiveOnly = !this.mailboxInfo.IsPrimary
			};
			IAsyncResult result = this.ewsClient.BeginGetNonIndexableItemStatistics(null, null, parameters);
			GetNonIndexableItemStatisticsResponse getNonIndexableItemStatisticsResponse = this.ewsClient.EndGetNonIndexableItemStatistics(result);
			if (getNonIndexableItemStatisticsResponse.NonIndexableStatistics != null && getNonIndexableItemStatisticsResponse.NonIndexableStatistics.Count > 0)
			{
				NonIndexableItemStatistic nonIndexableItemStatistic = getNonIndexableItemStatisticsResponse.NonIndexableStatistics[0];
				if (!string.IsNullOrEmpty(nonIndexableItemStatistic.ErrorMessage))
				{
					base.AddFailedMailbox(nonIndexableItemStatistic.Mailbox, nonIndexableItemStatistic.ErrorMessage);
				}
				this.UpdateResults(nonIndexableItemStatistic.Mailbox, (int)getNonIndexableItemStatisticsResponse.NonIndexableStatistics[0].ItemCount);
			}
		}

		protected override void HandleExecuteSearchWebServiceFailed(string legacyDn)
		{
			string errorMessage = this.failedMailboxes.ContainsKey(legacyDn) ? this.failedMailboxes[legacyDn] : null;
			bool flag = false;
			foreach (NonIndexableItemStatisticsInfo nonIndexableItemStatisticsInfo in this.Results)
			{
				if (string.Equals(nonIndexableItemStatisticsInfo.Mailbox, legacyDn, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					nonIndexableItemStatisticsInfo.ErrorMessage = errorMessage;
					break;
				}
			}
			if (!flag)
			{
				this.Results.Add(new NonIndexableItemStatisticsInfo(legacyDn, 0, errorMessage));
			}
		}

		private void RetrieveFailedItemsCount(ADRawEntry adRawEntry, MailboxType mailboxType)
		{
			base.PerformMailboxDiscovery(adRawEntry, mailboxType, out this.groupId, out this.mailboxInfo);
			switch (this.groupId.GroupType)
			{
			case GroupType.CrossServer:
			case GroupType.CrossPremise:
				base.ExecuteSearchWebService();
				return;
			case GroupType.SkippedError:
			{
				string error = (this.groupId.Error == null) ? string.Empty : this.groupId.Error.Message;
				base.AddFailedMailbox(this.mailboxInfo.LegacyExchangeDN, error);
				return;
			}
			default:
				if (this.mailboxInfo.MailboxGuid != Guid.Empty)
				{
					Guid guid = (mailboxType == MailboxType.Archive) ? this.mailboxInfo.ArchiveDatabase : this.mailboxInfo.MdbGuid;
					string indexSystemName = FastIndexVersion.GetIndexSystemName(guid);
					using (IFailedItemStorage failedItemStorage = Factory.Current.CreateFailedItemStorage(Factory.Current.CreateSearchServiceConfig(), indexSystemName))
					{
						FailedItemParameters parameters = new FailedItemParameters(FailureMode.All, FieldSet.None)
						{
							MailboxGuid = new Guid?(this.mailboxInfo.MailboxGuid)
						};
						int totalNonIndexableItems = (int)failedItemStorage.GetFailedItemsCount(parameters);
						this.UpdateResults(this.mailboxInfo.LegacyExchangeDN, totalNonIndexableItems);
					}
				}
				return;
			}
		}

		private void UpdateResults(string legacyDn, int totalNonIndexableItems)
		{
			string errorMessage = this.failedMailboxes.ContainsKey(legacyDn) ? this.failedMailboxes[legacyDn] : null;
			bool flag = false;
			foreach (NonIndexableItemStatisticsInfo nonIndexableItemStatisticsInfo in this.Results)
			{
				if (string.Equals(nonIndexableItemStatisticsInfo.Mailbox, legacyDn, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					nonIndexableItemStatisticsInfo.ItemCount += totalNonIndexableItems;
					nonIndexableItemStatisticsInfo.ErrorMessage = errorMessage;
					break;
				}
			}
			if (!flag)
			{
				this.Results.Add(new NonIndexableItemStatisticsInfo(legacyDn, totalNonIndexableItems, errorMessage));
			}
		}
	}
}
