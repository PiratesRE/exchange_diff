using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetNonIndexableItemDetails : SingleStepServiceCommand<GetNonIndexableItemDetailsRequest, NonIndexableItemDetailResult>, IDisposeTrackable, IDisposable
	{
		public GetNonIndexableItemDetails(CallContext callContext, GetNonIndexableItemDetailsRequest request) : base(callContext, request)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.mailboxes = request.Mailboxes;
			this.pageSize = request.PageSize;
			this.pageItemReference = request.PageItemReference;
			this.pageDirection = request.PageDirection;
			this.searchArchiveOnly = request.SearchArchiveOnly;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<GetNonIndexableItemDetails>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetNonIndexableItemDetailsResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<NonIndexableItemDetailResult> Execute()
		{
			MailboxSearchHelper.PerformCommonAuthorization(base.CallContext.IsExternalUser, out this.runspaceConfig, out this.recipientSession);
			return this.ProcessRequest();
		}

		private void Dispose(bool fromDispose)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (!this.disposed)
			{
				this.disposed = true;
			}
		}

		private ServiceResult<NonIndexableItemDetailResult> ProcessRequest()
		{
			if (this.mailboxes.Length != 1)
			{
				throw new ServiceArgumentException((CoreResources.IDs)4136809189U);
			}
			int num = 100;
			if (this.pageSize != null && this.pageSize != null && this.pageSize.Value > 0)
			{
				num = this.pageSize.Value;
			}
			if (this.pageDirection != null && this.pageDirection != null && this.pageDirection.Value == SearchPageDirectionType.Previous)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3699315399U);
			}
			Dictionary<string, ADRawEntry> dictionary = MailboxSearchHelper.FindADEntriesByLegacyExchangeDNs(this.recipientSession, this.mailboxes, MailboxSearchHelper.AdditionalProperties);
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, ADRawEntry> keyValuePair in dictionary)
				{
					if (!MailboxSearchHelper.HasPermissionToSearchMailbox(this.runspaceConfig, keyValuePair.Value))
					{
						throw new ServiceInvalidOperationException((CoreResources.IDs)2354781453U);
					}
				}
			}
			List<NonIndexableItemDetail> list = new List<NonIndexableItemDetail>();
			CallerInfo callerInfo = new CallerInfo(MailboxSearchHelper.IsOpenAsAdmin(base.CallContext), MailboxSearchConverter.GetCommonAccessToken(base.CallContext), base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallContext.EffectiveCaller.PrimarySmtpAddress, this.recipientSession.SessionSettings.CurrentOrganizationId, string.Empty, MailboxSearchHelper.GetQueryCorrelationId(), MailboxSearchHelper.GetUserRolesFromAuthZClientInfo(base.CallContext.EffectiveCaller), MailboxSearchHelper.GetApplicationRolesFromAuthZClientInfo(base.CallContext.EffectiveCaller));
			NonIndexableItemPagingInfo pagingInfo = new NonIndexableItemPagingInfo(num, this.pageItemReference);
			NonIndexableItemDetailsProvider nonIndexableItemDetailsProvider = new NonIndexableItemDetailsProvider(this.recipientSession, MailboxSearchHelper.GetTimeZone(), callerInfo, this.recipientSession.SessionSettings.CurrentOrganizationId, this.mailboxes, this.searchArchiveOnly, pagingInfo);
			nonIndexableItemDetailsProvider.ExecuteSearch();
			NonIndexableItemDetailResult nonIndexableItemDetailResult = new NonIndexableItemDetailResult();
			if (nonIndexableItemDetailsProvider.Results != null)
			{
				List<int> list2 = new List<int>(nonIndexableItemDetailsProvider.Results.Count);
				Dictionary<int, int> dictionary2 = new Dictionary<int, int>(nonIndexableItemDetailsProvider.Results.Count);
				ItemId itemId = null;
				MailboxId mailboxId = null;
				int num2 = 0;
				foreach (NonIndexableItem nonIndexableItem in nonIndexableItemDetailsProvider.Results)
				{
					ItemId itemId2;
					if (nonIndexableItem.CompositeId != null)
					{
						MdbItemIdentity compositeId = nonIndexableItem.CompositeId;
						int documentId = compositeId.DocumentId;
						dictionary2.Add(num2, documentId);
						if (mailboxId == null)
						{
							Guid mailboxGuid = compositeId.MailboxGuid;
							mailboxId = new MailboxId(compositeId.MailboxGuid);
						}
						itemId2 = IdConverter.GetItemIdFromStoreId(compositeId.ItemId, mailboxId);
						if (itemId == null && itemId2 != null)
						{
							itemId = itemId2;
						}
						if (documentId > 0)
						{
							list2.Add(documentId);
						}
					}
					else
					{
						itemId2 = new ItemId(nonIndexableItem.ItemId.UniqueId, nonIndexableItem.ItemId.ChangeKey);
						dictionary2.Add(num2, -1);
					}
					list.Add(new NonIndexableItemDetail
					{
						ItemId = itemId2,
						ErrorCode = ItemIndexError.None,
						ErrorDescription = nonIndexableItem.ErrorDescription.ToString(),
						IsPartiallyIndexed = nonIndexableItem.IsPartiallyIndexed,
						IsPermanentFailure = nonIndexableItem.IsPermanentFailure,
						AttemptCount = nonIndexableItem.AttemptCount,
						LastAttemptTime = nonIndexableItem.LastAttemptTime,
						AdditionalInfo = nonIndexableItem.AdditionalInfo,
						SortValue = nonIndexableItem.SortValue
					});
					num2++;
				}
				Dictionary<int, StoreId> storeIdsFromDocumentIds = this.GetStoreIdsFromDocumentIds(itemId, list2);
				if (storeIdsFromDocumentIds != null)
				{
					num2 = 0;
					foreach (NonIndexableItemDetail nonIndexableItemDetail in list)
					{
						int num3 = dictionary2[num2++];
						if (num3 > 0 && storeIdsFromDocumentIds.ContainsKey(num3))
						{
							StoreId storeId = storeIdsFromDocumentIds[num3];
							if (storeId != null)
							{
								ItemId itemIdFromStoreId = IdConverter.GetItemIdFromStoreId(storeId, mailboxId);
								if (itemIdFromStoreId != null)
								{
									if (nonIndexableItemDetail.ItemId != null && itemIdFromStoreId.Id != nonIndexableItemDetail.ItemId.Id)
									{
										string arg = (this.mailboxes != null && this.mailboxes.Length > 0) ? this.mailboxes[0] : string.Empty;
										ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<int, string>(0L, "GetNonIndexableItemDetails::ProcessRequest -- Got a newId from the DocId query that was different from the Id we got from FAST. DocumentId: {0} mailbox: {1}", num3, arg);
									}
									nonIndexableItemDetail.ItemId = itemIdFromStoreId;
									nonIndexableItemDetail.AdditionalInfo = string.Format("{0}{1}{2}", nonIndexableItemDetail.AdditionalInfo, "4887312c-8b40-4fec-a252-f2749065c0e5", num3);
								}
							}
						}
					}
				}
				nonIndexableItemDetailResult.Items = list.ToArray();
			}
			if (nonIndexableItemDetailsProvider.FailedMailboxes != null && nonIndexableItemDetailsProvider.FailedMailboxes.Count > 0)
			{
				List<FailedSearchMailbox> list3 = new List<FailedSearchMailbox>(nonIndexableItemDetailsProvider.FailedMailboxes.Count);
				foreach (KeyValuePair<string, string> keyValuePair2 in nonIndexableItemDetailsProvider.FailedMailboxes)
				{
					list3.Add(new FailedSearchMailbox
					{
						Mailbox = keyValuePair2.Key,
						ErrorMessage = keyValuePair2.Value
					});
				}
				nonIndexableItemDetailResult.FailedMailboxes = list3.ToArray();
			}
			return new ServiceResult<NonIndexableItemDetailResult>(nonIndexableItemDetailResult);
		}

		public Dictionary<int, StoreId> GetStoreIdsFromDocumentIds(ItemId itemId, List<int> docIdList)
		{
			if (itemId == null)
			{
				return null;
			}
			IdAndSession idAndSession = null;
			Item item = null;
			try
			{
				idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(itemId, BasicTypes.Item, false, true, ref item);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "GetNonIndexableItemDetails::GetStoreIdsFromDocumentIds -- threw an Exception trying to get a sessionOnly: {0}, we don't want to return this to the client.", ex.ToString());
				return null;
			}
			List<ComparisonFilter> list = new List<ComparisonFilter>(docIdList.Count);
			foreach (int num in docIdList)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.DocumentId, num));
			}
			Dictionary<int, StoreId> dictionary = new Dictionary<int, StoreId>(list.Count);
			if (idAndSession != null)
			{
				StoreSession session = idAndSession.Session;
				Folder folder = null;
				ItemQueryType queryFlags = ItemQueryType.None;
				if (session != null)
				{
					if (session.IsPublicFolderSession)
					{
						StoreId parentFolderId = idAndSession.ParentFolderId;
						if (parentFolderId != null)
						{
							folder = Folder.Bind(session, parentFolderId);
						}
					}
					else
					{
						folder = Folder.Bind(session, DefaultFolderType.Configuration, new PropertyDefinition[]
						{
							FolderSchema.Id
						});
						queryFlags = ItemQueryType.DocumentIdView;
					}
				}
				if (folder == null)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "GetNonIndexableItemDetails::GetStoreIdsFromDocumentIds -- When querying for ItemId from DocumentId: folder was null");
					return null;
				}
				List<PropertyDefinition> list2 = new List<PropertyDefinition>(2);
				list2.Add(ItemSchema.DocumentId);
				list2.Add(ItemSchema.Id);
				QueryFilter queryFilter = new OrFilter(list.ToArray());
				int rowCount = Math.Min(list.Count, 100);
				using (folder)
				{
					using (QueryResult queryResult = folder.ItemQuery(queryFlags, queryFilter, null, list2))
					{
						object[][] rows;
						while ((rows = queryResult.GetRows(rowCount)) != null && rows.Length > 0)
						{
							foreach (object[] array2 in rows)
							{
								int? num2 = array2[0] as int?;
								VersionedId versionedId = array2[1] as VersionedId;
								if (versionedId == null)
								{
									ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "GetNonIndexableItemDetails::GetStoreIdsFromDocumentIds -- When querying for ItemId from DocumentId: ItemId was null");
								}
								else
								{
									StoreId objectId = versionedId.ObjectId;
									int key = (num2 != null) ? num2.Value : -1;
									if (num2 != null && objectId != null && !dictionary.ContainsKey(key))
									{
										dictionary.Add(key, objectId);
									}
								}
							}
						}
					}
					return dictionary;
				}
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "GetNonIndexableItemDetails::GetStoreIdsFromDocumentIds -- When querying for ItemId from DocumentId: idAndSessionOnly was null");
			return null;
		}

		private const int DefaultPageSize = 100;

		private const int QUERYROWS_BATCH_SIZE = 100;

		public const string DocumentIdMarker = "4887312c-8b40-4fec-a252-f2749065c0e5";

		private readonly DisposeTracker disposeTracker;

		private readonly string[] mailboxes;

		private readonly int? pageSize;

		private readonly string pageItemReference;

		private readonly SearchPageDirectionType? pageDirection;

		private readonly bool searchArchiveOnly;

		private bool disposed;

		private ExchangeRunspaceConfiguration runspaceConfig;

		private IRecipientSession recipientSession;
	}
}
