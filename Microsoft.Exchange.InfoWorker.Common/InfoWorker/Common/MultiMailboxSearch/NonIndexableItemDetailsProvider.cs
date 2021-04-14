using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Engine;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class NonIndexableItemDetailsProvider : NonIndexableItemProvider
	{
		public NonIndexableItemDetailsProvider(IRecipientSession recipientSession, ExTimeZone timeZone, CallerInfo callerInfo, OrganizationId orgId, string[] mailboxes, bool searchArchiveOnly, NonIndexableItemPagingInfo pagingInfo) : base(recipientSession, timeZone, callerInfo, orgId, mailboxes, searchArchiveOnly)
		{
			this.pagingInfo = pagingInfo;
			if (this.pagingInfo == null)
			{
				this.pagingInfo = new NonIndexableItemPagingInfo(int.MaxValue, null);
			}
			if (this.pagingInfo.PageSize == 0)
			{
				throw new ArgumentException("Page size cannot be 0");
			}
			this.Results = new List<NonIndexableItem>();
		}

		public List<NonIndexableItem> Results { get; private set; }

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
						this.RetrieveFailedItems(adrawEntry, MailboxType.Primary);
					}
					if (!this.alreadyProxy && (Guid)adrawEntry[ADUserSchema.ArchiveGuid] != Guid.Empty)
					{
						this.RetrieveFailedItems(adrawEntry, MailboxType.Archive);
					}
				}
				catch (Exception ex)
				{
					Factory.Current.GeneralTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Failed to retrieve failed items for {1}", this.callerInfo.QueryCorrelationId, text);
					base.AddFailedMailbox(text, ex.Message);
				}
			}
			foreach (string text2 in list)
			{
				base.AddFailedMailbox(text2, Strings.SourceMailboxUserNotFoundInAD(text2));
			}
		}

		protected override void InternalExecuteSearchWebService()
		{
			GetNonIndexableItemDetailsParameters parameters = new GetNonIndexableItemDetailsParameters
			{
				Mailboxes = new string[]
				{
					this.mailboxInfo.LegacyExchangeDN
				},
				SearchArchiveOnly = !this.mailboxInfo.IsPrimary,
				PageSize = new int?(this.pagingInfo.PageSize),
				PageItemReference = this.pagingInfo.PageItemReference
			};
			IAsyncResult result = this.ewsClient.BeginGetNonIndexableItemDetails(null, null, parameters);
			GetNonIndexableItemDetailsResponse getNonIndexableItemDetailsResponse = this.ewsClient.EndGetNonIndexableItemDetails(result);
			if (getNonIndexableItemDetailsResponse.NonIndexableItemsResult != null)
			{
				if (getNonIndexableItemDetailsResponse.NonIndexableItemsResult.Items != null && getNonIndexableItemDetailsResponse.NonIndexableItemsResult.Items.Length > 0)
				{
					List<NonIndexableItem> nonIndexableItems = NonIndexableItemDetailsProvider.ConvertFromWebServiceFailedItemsCollection(getNonIndexableItemDetailsResponse.NonIndexableItemsResult.Items);
					this.UpdateResults(nonIndexableItems);
				}
				if (getNonIndexableItemDetailsResponse.NonIndexableItemsResult.FailedMailboxes != null && getNonIndexableItemDetailsResponse.NonIndexableItemsResult.FailedMailboxes.Length > 0)
				{
					foreach (FailedSearchMailbox failedSearchMailbox in getNonIndexableItemDetailsResponse.NonIndexableItemsResult.FailedMailboxes)
					{
						base.AddFailedMailbox(failedSearchMailbox.Mailbox, failedSearchMailbox.ErrorMessage);
					}
				}
			}
		}

		protected override void HandleExecuteSearchWebServiceFailed(string legacyDn)
		{
		}

		private static void AddFailedItemsToCollection(IFailedItemStorage store, Guid mailboxGuid, long referenceDocId, int pageSize, List<NonIndexableItem> nonIndexableItems)
		{
			FailedItemParameters parameters = new FailedItemParameters(FailureMode.All, FieldSet.Default)
			{
				MailboxGuid = new Guid?(mailboxGuid),
				StartingIndexId = referenceDocId,
				ResultLimit = pageSize
			};
			foreach (IFailureEntry failureEntry in store.GetFailedItems(parameters))
			{
				MdbItemIdentity mdbItemIdentity = (MdbItemIdentity)failureEntry.ItemId;
				nonIndexableItems.Add(new NonIndexableItem
				{
					CompositeId = mdbItemIdentity,
					ErrorCode = NonIndexableItem.ConvertSearchErrorCode(failureEntry.ErrorCode),
					ErrorDescription = failureEntry.ErrorDescription.ToString(),
					IsPartiallyIndexed = failureEntry.IsPartiallyIndexed,
					IsPermanentFailure = failureEntry.IsPermanentFailure,
					AttemptCount = failureEntry.AttemptCount,
					LastAttemptTime = failureEntry.LastAttemptTime,
					AdditionalInfo = failureEntry.AdditionalInfo,
					SortValue = IndexId.CreateIndexId(mdbItemIdentity.MailboxNumber, mdbItemIdentity.DocumentId).ToString()
				});
			}
		}

		private static List<NonIndexableItem> ConvertFromWebServiceFailedItemsCollection(NonIndexableItem[] sourceNonIndexableItems)
		{
			List<NonIndexableItem> list = new List<NonIndexableItem>(sourceNonIndexableItems.Length);
			foreach (NonIndexableItem nonIndexableItem in sourceNonIndexableItems)
			{
				list.Add(new NonIndexableItem
				{
					ItemId = nonIndexableItem.ItemId,
					ErrorCode = NonIndexableItem.ConvertSearchErrorCode(nonIndexableItem.ErrorCode.ToString()),
					ErrorDescription = nonIndexableItem.ErrorDescription,
					IsPartiallyIndexed = nonIndexableItem.IsPartiallyIndexed,
					IsPermanentFailure = nonIndexableItem.IsPermanentFailure,
					AttemptCount = nonIndexableItem.AttemptCount,
					LastAttemptTime = nonIndexableItem.LastAttemptTime,
					AdditionalInfo = nonIndexableItem.AdditionalInfo,
					SortValue = nonIndexableItem.SortValue
				});
			}
			return list;
		}

		private static List<NonIndexableItem> MergeFailedItemsCollection(List<NonIndexableItem> firstCollection, List<NonIndexableItem> secondCollection, int pageSize)
		{
			if (firstCollection == null || firstCollection.Count == 0)
			{
				return NonIndexableItemDetailsProvider.CreateCollectionOfFailedItems(secondCollection, pageSize);
			}
			if (secondCollection == null || secondCollection.Count == 0)
			{
				return NonIndexableItemDetailsProvider.CreateCollectionOfFailedItems(firstCollection, pageSize);
			}
			List<NonIndexableItem> list;
			if (pageSize == 2147483647)
			{
				list = new List<NonIndexableItem>();
			}
			else
			{
				list = new List<NonIndexableItem>(pageSize);
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			do
			{
				NonIndexableItem nonIndexableItem;
				if (num < firstCollection.Count)
				{
					nonIndexableItem = firstCollection[num];
				}
				else
				{
					nonIndexableItem = null;
				}
				NonIndexableItem nonIndexableItem2;
				if (num2 < secondCollection.Count)
				{
					nonIndexableItem2 = secondCollection[num2];
				}
				else
				{
					nonIndexableItem2 = null;
				}
				if (nonIndexableItem == null && nonIndexableItem2 == null)
				{
					break;
				}
				if (nonIndexableItem == null)
				{
					list.Add(nonIndexableItem2);
					num2++;
				}
				else if (nonIndexableItem2 == null)
				{
					list.Add(nonIndexableItem);
					num++;
				}
				else
				{
					long num4;
					long.TryParse(nonIndexableItem.SortValue, out num4);
					long num5;
					long.TryParse(nonIndexableItem2.SortValue, out num5);
					if (num4 <= num5)
					{
						list.Add(nonIndexableItem);
						num++;
					}
					else
					{
						list.Add(nonIndexableItem2);
						num2++;
					}
				}
			}
			while (++num3 < pageSize);
			return list;
		}

		private static List<NonIndexableItem> CreateCollectionOfFailedItems(List<NonIndexableItem> sourceCollection, int pageSize)
		{
			if (sourceCollection != null && sourceCollection.Count > 0)
			{
				List<NonIndexableItem> list = new List<NonIndexableItem>(Math.Min(sourceCollection.Count, pageSize));
				if (sourceCollection.Count <= pageSize)
				{
					list.AddRange(sourceCollection);
				}
				else
				{
					for (int i = 0; i < pageSize; i++)
					{
						list.Add(sourceCollection[i]);
					}
				}
				return list;
			}
			return new List<NonIndexableItem>(1);
		}

		private void RetrieveFailedItems(ADRawEntry adRawEntry, MailboxType mailboxType)
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
						List<NonIndexableItem> nonIndexableItems = new List<NonIndexableItem>();
						NonIndexableItemDetailsProvider.AddFailedItemsToCollection(failedItemStorage, this.mailboxInfo.MailboxGuid, this.GetReferenceDocId(), this.pagingInfo.PageSize, nonIndexableItems);
						this.UpdateResults(nonIndexableItems);
					}
				}
				return;
			}
		}

		private void UpdateResults(List<NonIndexableItem> nonIndexableItems)
		{
			this.Results = NonIndexableItemDetailsProvider.MergeFailedItemsCollection(this.Results, nonIndexableItems, this.pagingInfo.PageSize);
		}

		private long GetReferenceDocId()
		{
			long num = 0L;
			if (!string.IsNullOrEmpty(this.pagingInfo.PageItemReference))
			{
				long.TryParse(this.pagingInfo.PageItemReference, out num);
			}
			if (num != 0L)
			{
				return num + 1L;
			}
			return num;
		}

		private readonly NonIndexableItemPagingInfo pagingInfo;
	}
}
