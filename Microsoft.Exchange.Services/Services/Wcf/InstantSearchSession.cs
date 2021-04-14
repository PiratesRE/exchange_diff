using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Query;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class InstantSearchSession
	{
		public InstantSearchSession(string searchSessionId, InstantSearchMailboxDataSnapshot mbxData, IReadOnlyList<StoreId> folderIds, InstantSearchItemType itemType, SuggestionSourceType suggestionSourceType)
		{
			this.searchSessionId = searchSessionId;
			Guid correlationId;
			if (!Guid.TryParse(this.searchSessionId, out correlationId))
			{
				correlationId = Guid.NewGuid();
			}
			if (folderIds == null || folderIds.Count == 0)
			{
				folderIds = new List<StoreId>
				{
					mbxData.RootFolderId
				};
			}
			this.mailboxData = mbxData;
			ICollection<PropertyDefinition> propertyListForItemType = InstantSearchSession.InstantSearchPropertyDefinitionListFactory.GetPropertyListForItemType(itemType);
			if (propertyListForItemType != null)
			{
				this.instantSearch = new InstantSearch(this.mailboxData.mailboxSession, folderIds, propertyListForItemType, correlationId);
			}
			else
			{
				this.instantSearch = new InstantSearch(this.mailboxData.mailboxSession, folderIds, correlationId);
			}
			this.instantSearch.SearchForConversations = (itemType == InstantSearchItemType.MailConversation);
			this.instantSearch.ResultsUpdatedCallback = new Action<IReadOnlyCollection<IReadOnlyPropertyBag>, ICancelableAsyncResult>(this.OnResultsUpdated);
			this.instantSearch.SuggestionsAvailableCallback = new Action<IReadOnlyCollection<QuerySuggestion>, ICancelableAsyncResult>(this.OnSuggestionsAvailable);
			this.instantSearch.RefinerDataAvailableCallback = new Action<IReadOnlyCollection<RefinerData>, ICancelableAsyncResult>(this.OnRefinerDataAvailable);
			this.instantSearch.HitHighlightingDataAvailableCallback = new Action<IReadOnlyCollection<string>, ICancelableAsyncResult>(this.OnHitHighlightingDataAvailable);
		}

		public string SessionId
		{
			get
			{
				return this.searchSessionId;
			}
		}

		internal void BeginStartSession(long searchRequestId)
		{
			this.instantSearch.BeginStartSession(new AsyncCallback(this.OnStartSessionComplete), searchRequestId);
		}

		internal EndInstantSearchSessionResponse BeginStopSession(long searchRequestId)
		{
			this.instantSearch.BeginStopSession(new AsyncCallback(this.OnStopSessionComplete), searchRequestId);
			Dictionary<long, List<SearchPathSnapshotType>> dataDictionary;
			lock (this.dataLock)
			{
				dataDictionary = this.sessionWidePerfDataCollection;
			}
			return new EndInstantSearchSessionResponse(dataDictionary);
		}

		internal PerformInstantSearchResponse PerformInstantSearch(PerformInstantSearchRequest request, CallContext callContext, IReadOnlyList<StoreId> folderScope, SearchPerfMarkerContainer perfMarkers, IInstantSearchNotificationHandler notificationHandler)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (perfMarkers == null)
			{
				throw new ArgumentNullException("perfMarkers");
			}
			if (notificationHandler == null)
			{
				throw new ArgumentNullException("notificationHandler");
			}
			if (folderScope == null || folderScope.Count == 0)
			{
				folderScope = new List<StoreId>
				{
					this.mailboxData.RootFolderId
				};
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.SearchSessionID, request.SearchSessionId);
			InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.SearchRequestID, request.SearchRequestId);
			InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.RequestedResultTypes, request.ItemType);
			InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.WaitOnSearchResults, request.WaitOnSearchResults);
			QueryOptions queryOptions = InstantSearchSession.GetQueryOptions(request.QueryOptions);
			InstantSearchQueryParameters instantSearchQueryParameters;
			if ((queryOptions & QueryOptions.Results) != QueryOptions.None)
			{
				QueryFilter queryFilter = this.BuildAdditionalFiltersQuery(folderScope, request.IsDeepTraversal, request.DateRestriction);
				InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.QueryFilter, queryFilter);
				instantSearchQueryParameters = new InstantSearchQueryParameters(request.KqlQuery, queryFilter, queryOptions);
				instantSearchQueryParameters.DeepTraversal = request.IsDeepTraversal;
				instantSearchQueryParameters.MaximumResultCount = new int?(request.MaximumResultCount);
				InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.RequestedResultsCount, request.MaximumResultCount);
			}
			else
			{
				instantSearchQueryParameters = new InstantSearchQueryParameters(request.KqlQuery, null, queryOptions);
			}
			InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.QueryOptions, queryOptions);
			instantSearchQueryParameters.FolderScope = folderScope;
			if (folderScope != null && folderScope.Count > 0)
			{
				InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.FolderScope, folderScope[0]);
			}
			if ((queryOptions & QueryOptions.Suggestions) != QueryOptions.None)
			{
				this.instantSearch.MaximumSuggestionsCount = request.MaxSuggestionsCount;
				InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.RequestedSuggestionsCount, request.MaxSuggestionsCount);
				QuerySuggestionSources querySuggestionSources;
				if (request.SuggestionSources == SuggestionSourceType.None)
				{
					querySuggestionSources = QuerySuggestionSources.RecentSearches;
				}
				else
				{
					querySuggestionSources = (QuerySuggestionSources)request.SuggestionSources;
				}
				instantSearchQueryParameters.QuerySuggestionSources = querySuggestionSources;
				InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.SuggestionSources, querySuggestionSources);
			}
			if ((queryOptions & QueryOptions.Refiners) != QueryOptions.None)
			{
				instantSearchQueryParameters.Refiners = this.CreateRefiners(folderScope, request.KqlQuery);
				instantSearchQueryParameters.MaximumRefinersCount = 20;
				InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.RequestedRefinersCount, 20);
			}
			if (request.RefinementFilter != null && request.RefinementFilter.RefinerQueries != null && request.RefinementFilter.RefinerQueries.Length > 0)
			{
				InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.AppliedRefinementFiltersCount, request.RefinementFilter.RefinerQueries.Length);
				instantSearchQueryParameters.RefinementFilter = new RefinementFilter(request.RefinementFilter.RefinerQueries);
			}
			InstantSearchRequestContext instantSearchRequestContext = new InstantSearchRequestContext(request, notificationHandler, perfMarkers);
			instantSearchRequestContext.PerfMarkers.SetPerfMarker(InstantSearchPerfKey.InstantSearchAPIMethodInvocationTimeStamp);
			lock (this.dataLock)
			{
				this.sessionWidePerfDataCollection[instantSearchRequestContext.Request.SearchRequestId] = new List<SearchPathSnapshotType>(7);
			}
			if (request.WaitOnSearchResults)
			{
				instantSearchRequestContext.SearchResultsReceivedEvent = new ManualResetEvent(false);
			}
			InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.PreSearchDuration, stopwatch.ElapsedMilliseconds);
			this.instantSearch.BeginInstantSearchRequest(instantSearchQueryParameters, new AsyncCallback(this.OnCompleteInstantSearchRequest), instantSearchRequestContext);
			if (request.WaitOnSearchResults)
			{
				try
				{
					if (!instantSearchRequestContext.SearchResultsReceivedEvent.WaitOne(30000))
					{
						throw new InstantSearchException("Results Timeout");
					}
					if (instantSearchRequestContext.Error != null)
					{
						throw new InstantSearchException(instantSearchRequestContext.Error);
					}
					return instantSearchRequestContext.Response;
				}
				finally
				{
					lock (instantSearchRequestContext)
					{
						instantSearchRequestContext.SearchResultsReceivedEvent.Close();
						instantSearchRequestContext.ResponseSent = true;
					}
					if (request.WaitOnSearchResults)
					{
						InstantSearchSession.SafeLogData(callContext, PerformInstantSearchMetaData.TotalSearchDuration, stopwatch.ElapsedMilliseconds);
					}
				}
			}
			return new PerformInstantSearchResponse();
		}

		private IReadOnlyCollection<PropertyDefinition> CreateRefiners(IReadOnlyList<StoreId> folderScope, string userQuery)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>(5);
			userQuery = ((userQuery == null) ? string.Empty : userQuery.ToUpper(CultureInfo.InvariantCulture));
			list.Add(StoreObjectSchema.ParentEntryId);
			list.Add(ItemSchema.HasAttachment);
			bool flag = false;
			bool flag2;
			if (this.mailboxData.SentItemsFolderId == null)
			{
				flag = false;
				flag2 = true;
			}
			else
			{
				for (int i = 0; i < folderScope.Count; i++)
				{
					if (this.mailboxData.SentItemsFolderId.Equals(folderScope[i]))
					{
						flag = true;
						break;
					}
				}
				flag2 = (!flag || folderScope.Count > 1);
			}
			if (flag && !userQuery.ToUpper().Contains("TO:"))
			{
				list.Add(ItemSchema.SearchRecipients);
			}
			if (flag2 && !userQuery.ToUpper().Contains("FROM:"))
			{
				list.Add(ItemSchema.From);
			}
			return list;
		}

		private static QueryOptions GetQueryOptions(QueryOptionsType instantSearchRequestedResultType)
		{
			QueryOptions queryOptions = QueryOptions.None;
			if ((instantSearchRequestedResultType & QueryOptionsType.Suggestions) != QueryOptionsType.None)
			{
				queryOptions |= QueryOptions.Suggestions;
			}
			if ((instantSearchRequestedResultType & QueryOptionsType.Refiners) != QueryOptionsType.None)
			{
				queryOptions |= QueryOptions.Refiners;
			}
			if ((instantSearchRequestedResultType & QueryOptionsType.SearchTerms) != QueryOptionsType.None)
			{
				queryOptions |= QueryOptions.SearchTerms;
			}
			if ((instantSearchRequestedResultType & QueryOptionsType.Results) != QueryOptionsType.None)
			{
				queryOptions |= QueryOptions.Results;
			}
			if ((instantSearchRequestedResultType & QueryOptionsType.ExplicitSearch) != QueryOptionsType.None)
			{
				queryOptions |= QueryOptions.ExplicitSearch;
			}
			if ((instantSearchRequestedResultType & QueryOptionsType.SuggestionsPrimer) != QueryOptionsType.None)
			{
				queryOptions |= QueryOptions.SuggestionsPrimer;
			}
			if ((instantSearchRequestedResultType & QueryOptionsType.AllowFuzzing) != QueryOptionsType.None)
			{
				queryOptions |= QueryOptions.AllowFuzzing;
			}
			return queryOptions;
		}

		private void OnHitHighlightingDataAvailable(IReadOnlyCollection<string> highlightData, ICancelableAsyncResult aSyncResult)
		{
			InstantSearchRequestContext updateQueryContext = (InstantSearchRequestContext)aSyncResult.AsyncState;
			if (highlightData == null || highlightData.Count == 0)
			{
				return;
			}
			string[] searchTerms = new string[highlightData.Count];
			int num = 0;
			foreach (string text in highlightData)
			{
				searchTerms[num++] = text;
			}
			if (updateQueryContext.Request.WaitOnSearchResults)
			{
				updateQueryContext.SearchTerms = searchTerms;
				return;
			}
			this.AnnotateWithTimeStampsAndDeliverPayload(updateQueryContext, QueryOptionsType.SearchTerms, true, (SearchPerfMarkerContainer perfMarkerContainer) => new InstantSearchPayloadType(updateQueryContext.Request.SearchSessionId, updateQueryContext.Request.SearchRequestId, InstantSearchResultType.SearchTerms, perfMarkerContainer)
			{
				SearchTerms = searchTerms
			});
		}

		private void OnRefinerDataAvailable(IReadOnlyCollection<RefinerData> refinerData, ICancelableAsyncResult aSyncResult)
		{
			InstantSearchRequestContext updateQueryContext = (InstantSearchRequestContext)aSyncResult.AsyncState;
			this.AnnotateWithTimeStampsAndDeliverPayload(updateQueryContext, QueryOptionsType.Refiners, true, delegate(SearchPerfMarkerContainer perfMarkerContainer)
			{
				List<RefinerDataType> list = new List<RefinerDataType>(refinerData.Count);
				if ((updateQueryContext.Request.QueryOptions & QueryOptionsType.Refiners) != QueryOptionsType.None)
				{
					lock (this.mailboxData.mailboxSession)
					{
						foreach (RefinerData refinerData2 in refinerData)
						{
							RefinerDataType refinerDataType = RefinerDataTypeFactory.TryCreate(refinerData2, this.mailboxData.mailboxSession, new MailboxId(this.mailboxData.MailboxGuid, false));
							if (refinerDataType != null)
							{
								list.Add(refinerDataType);
							}
						}
					}
				}
				return new InstantSearchPayloadType(updateQueryContext.Request.SearchSessionId, updateQueryContext.Request.SearchRequestId, InstantSearchResultType.Refiners, perfMarkerContainer)
				{
					RefinerData = list.ToArray()
				};
			});
		}

		private void OnResultsUpdated(IReadOnlyCollection<IReadOnlyPropertyBag> searchResults, ICancelableAsyncResult aSyncResult)
		{
			InstantSearchRequestContext updateQueryContext = (InstantSearchRequestContext)aSyncResult.AsyncState;
			this.AnnotateWithTimeStampsAndDeliverPayload(updateQueryContext, QueryOptionsType.Results, !updateQueryContext.Request.WaitOnSearchResults, delegate(SearchPerfMarkerContainer perfMarkerContainer)
			{
				InstantSearchItemType itemType = updateQueryContext.Request.ItemType;
				InstantSearchPayloadType result = null;
				switch (itemType)
				{
				case InstantSearchItemType.MailItem:
				{
					ItemType[] items = this.SafeOnItemsReceived(searchResults, updateQueryContext);
					result = new InstantSearchPayloadType(updateQueryContext.Request.SearchSessionId, updateQueryContext.Request.SearchRequestId, InstantSearchResultType.ItemResults, perfMarkerContainer)
					{
						Items = items
					};
					break;
				}
				case InstantSearchItemType.MailConversation:
				{
					ConversationType[] conversations = this.SafeOnConversationsReceived(searchResults, updateQueryContext);
					result = new InstantSearchPayloadType(updateQueryContext.Request.SearchSessionId, updateQueryContext.Request.SearchRequestId, InstantSearchResultType.ConversationResults, perfMarkerContainer)
					{
						Conversations = conversations
					};
					break;
				}
				case InstantSearchItemType.CalendarItem:
				{
					EwsCalendarItemType[] calendarItems = this.SafeOnCalendarItemsReceived(searchResults, updateQueryContext);
					result = new InstantSearchPayloadType(updateQueryContext.Request.SearchSessionId, updateQueryContext.Request.SearchRequestId, InstantSearchResultType.CalendarItemResults, perfMarkerContainer)
					{
						CalendarItems = calendarItems
					};
					break;
				}
				case InstantSearchItemType.Persona:
				{
					Persona[] personaItems = this.SafeOnPeopleItemsReceived(searchResults, updateQueryContext);
					result = new InstantSearchPayloadType(updateQueryContext.Request.SearchSessionId, updateQueryContext.Request.SearchRequestId, InstantSearchResultType.PersonaResults, perfMarkerContainer)
					{
						PersonaItems = personaItems
					};
					break;
				}
				}
				return result;
			});
		}

		private Persona[] SafeOnPeopleItemsReceived(IReadOnlyCollection<IReadOnlyPropertyBag> searchResults, InstantSearchRequestContext updateQueryContext)
		{
			return new Persona[0];
		}

		private EwsCalendarItemType[] SafeOnCalendarItemsReceived(IReadOnlyCollection<IReadOnlyPropertyBag> searchResults, InstantSearchRequestContext updateQueryContext)
		{
			List<EwsCalendarItemType> list = new List<EwsCalendarItemType>(searchResults.Count);
			foreach (IReadOnlyPropertyBag readOnlyPropertyBag in searchResults)
			{
				try
				{
					IStorePropertyBag storePropertyBag = readOnlyPropertyBag as IStorePropertyBag;
					if (storePropertyBag != null)
					{
						EwsCalendarItemType ewsCalendarItemType = new EwsCalendarItemType();
						VersionedId versionedId = storePropertyBag.TryGetValueOrDefault(ItemSchema.Id, null);
						ewsCalendarItemType.ItemId = InstantSearchSession.StoreIdToEwsItemId(versionedId, new MailboxId(this.mailboxData.MailboxGuid, false));
						StoreObjectId storeId = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.ParentItemId, null);
						ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, new MailboxId(this.mailboxData.MailboxGuid), null);
						ewsCalendarItemType.ParentFolderId = new FolderId(concatenatedId.Id, concatenatedId.ChangeKey);
						ewsCalendarItemType.HasAttachments = new bool?(storePropertyBag.TryGetValueOrDefault(ItemSchema.HasAttachment, false));
						ewsCalendarItemType.Subject = storePropertyBag.TryGetValueOrDefault(ItemSchema.Subject, null);
						ewsCalendarItemType.ItemClass = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.ItemClass, null);
						ewsCalendarItemType.IsMeeting = new bool?(storePropertyBag.TryGetValueOrDefault(CalendarItemBaseSchema.IsMeeting, false));
						ewsCalendarItemType.CalendarItemType = (storePropertyBag.TryGetValueOrDefault(CalendarItemBaseSchema.AppointmentRecurring, false) ? CalendarItemTypeType.RecurringMaster : CalendarItemTypeType.Single);
						ewsCalendarItemType.SensitivityString = SensitivityConverter.ToString(storePropertyBag.TryGetValueOrDefault(ItemSchema.Sensitivity, Sensitivity.Normal));
						Participant participant = storePropertyBag.TryGetValueOrDefault(CalendarItemBaseSchema.Organizer, null);
						ewsCalendarItemType.Organizer = new SingleRecipientType();
						ewsCalendarItemType.Organizer.Mailbox = new EmailAddressWrapper();
						ewsCalendarItemType.Organizer.Mailbox.Name = participant.DisplayName;
						ewsCalendarItemType.Organizer.Mailbox.EmailAddress = participant.EmailAddress;
						ewsCalendarItemType.Organizer.Mailbox.OriginalDisplayName = participant.OriginalDisplayName;
						ExDateTime dateTime = storePropertyBag.TryGetValueOrDefault(CalendarItemInstanceSchema.StartTime, ExDateTime.MinValue);
						ewsCalendarItemType.Start = ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, this.mailboxData.TimeZone);
						ExDateTime dateTime2 = storePropertyBag.TryGetValueOrDefault(CalendarItemInstanceSchema.EndTime, ExDateTime.MinValue);
						ewsCalendarItemType.End = ExDateTimeConverter.ToOffsetXsdDateTime(dateTime2, this.mailboxData.TimeZone);
						if (ewsCalendarItemType.CalendarItemType == CalendarItemTypeType.RecurringMaster)
						{
							InternalRecurrence internalRecurrence = InternalRecurrence.FromMasterPropertyBag(storePropertyBag, this.mailboxData.mailboxSession, versionedId);
							OccurrenceInfoType occurrenceInfoType = ewsCalendarItemType.LastOccurrence = new OccurrenceInfoType();
							occurrenceInfoType.Start = ExDateTimeConverter.ToOffsetXsdDateTime(internalRecurrence.EndDate, this.mailboxData.TimeZone);
						}
						list.Add(ewsCalendarItemType);
					}
				}
				catch (Exception ex)
				{
					this.GenerateErrorPayload("SafeOnCalendarItemsReceived", ex, updateQueryContext.Request.SearchRequestId, updateQueryContext.NotificationHandler);
				}
			}
			return list.ToArray();
		}

		private ConversationType[] SafeOnConversationsReceived(IReadOnlyCollection<IReadOnlyPropertyBag> searchResults, InstantSearchRequestContext updateQueryContext)
		{
			List<ConversationType> list = new List<ConversationType>(searchResults.Count);
			foreach (IReadOnlyPropertyBag propertyBag in searchResults)
			{
				try
				{
					ConversationId conversationId = InstantSearchSession.TryGetValueOrDefault<ConversationId>(propertyBag, ConversationItemSchema.ConversationId, null);
					if (conversationId != null)
					{
						ConversationType conversationType = new ConversationType();
						conversationType.ConversationId = new Microsoft.Exchange.Services.Core.Types.ItemId(IdConverter.ConversationIdToEwsId(this.mailboxData.MailboxGuid, conversationId), null);
						StoreId[] array = InstantSearchSession.TryGetValueOrDefault<StoreId[]>(propertyBag, ConversationItemSchema.ConversationItemIds, InstantSearchSession.EmptyStoreId);
						conversationType.ItemIds = Array.ConvertAll<StoreId, Microsoft.Exchange.Services.Core.Types.ItemId>(array, (StoreId s) => InstantSearchSession.StoreIdToEwsItemId(s, new MailboxId(this.mailboxData.MailboxGuid, false)));
						StoreId[] array2 = InstantSearchSession.TryGetValueOrDefault<StoreId[]>(propertyBag, ConversationItemSchema.ConversationItemIds, InstantSearchSession.EmptyStoreId);
						conversationType.GlobalItemIds = Array.ConvertAll<StoreId, Microsoft.Exchange.Services.Core.Types.ItemId>(array2, (StoreId s) => InstantSearchSession.StoreIdToEwsItemId(s, new MailboxId(this.mailboxData.MailboxGuid, false)));
						conversationType.ConversationTopic = InstantSearchSession.TryGetValueOrDefault<string>(propertyBag, ConversationItemSchema.ConversationTopic, null);
						conversationType.MessageCount = new int?(InstantSearchSession.TryGetValueOrDefault<int>(propertyBag, ConversationItemSchema.ConversationMessageCount, 2));
						conversationType.GlobalMessageCount = new int?(InstantSearchSession.TryGetValueOrDefault<int>(propertyBag, ConversationItemSchema.ConversationGlobalMessageCount, 2));
						conversationType.FlagStatus = InstantSearchSession.TryGetValueOrDefault<FlagStatus>(propertyBag, ConversationItemSchema.ConversationFlagStatus, FlagStatus.NotFlagged);
						conversationType.UniqueRecipients = InstantSearchSession.TryGetValueOrDefault<string[]>(propertyBag, ConversationItemSchema.ConversationMVTo, InstantSearchSession.EmptyStringArray);
						conversationType.UniqueSenders = InstantSearchSession.TryGetValueOrDefault<string[]>(propertyBag, ConversationItemSchema.ConversationMVFrom, null);
						conversationType.LastDeliveryTime = InstantSearchSession.GetDateTimeProperty(propertyBag, this.mailboxData.TimeZone, ConversationItemSchema.ConversationLastDeliveryTime);
						conversationType.LastModifiedTime = conversationType.LastDeliveryTime;
						conversationType.ItemClasses = InstantSearchSession.TryGetValueOrDefault<string[]>(propertyBag, ConversationItemSchema.ConversationMessageClasses, InstantSearchSession.EmptyStringArray);
						Importance value = InstantSearchSession.TryGetValueOrDefault<Importance>(propertyBag, ConversationItemSchema.ConversationImportance, Importance.Normal);
						conversationType.Importance = (ImportanceType)Enum.ToObject(typeof(ImportanceType), (int)value);
						conversationType.HasAttachments = new bool?(InstantSearchSession.TryGetValueOrDefault<bool>(propertyBag, ConversationItemSchema.ConversationHasAttach, false));
						conversationType.UnreadCount = new int?(InstantSearchSession.TryGetValueOrDefault<int>(propertyBag, ConversationItemSchema.ConversationUnreadMessageCount, 0));
						conversationType.GlobalUnreadCount = new int?(InstantSearchSession.TryGetValueOrDefault<int>(propertyBag, ConversationItemSchema.ConversationGlobalUnreadMessageCount, 0));
						conversationType.Preview = InstantSearchSession.TryGetValueOrDefault<string>(propertyBag, ConversationItemSchema.ConversationPreview, null);
						list.Add(conversationType);
					}
				}
				catch (Exception ex)
				{
					this.GenerateErrorPayload("SafeOnConversationsReceived", ex, updateQueryContext.Request.SearchRequestId, updateQueryContext.NotificationHandler);
				}
			}
			return list.ToArray();
		}

		private ItemType[] SafeOnItemsReceived(IReadOnlyCollection<IReadOnlyPropertyBag> searchResults, InstantSearchRequestContext updateQueryContext)
		{
			List<ItemType> list = new List<ItemType>(searchResults.Count);
			foreach (IReadOnlyPropertyBag propertyBag in searchResults)
			{
				try
				{
					StoreId storeId = InstantSearchSession.TryGetValueOrDefault<StoreId>(propertyBag, ItemSchema.Id, null);
					StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
					ItemType itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
					itemType.ItemId = InstantSearchSession.StoreIdToEwsItemId(storeId, new MailboxId(this.mailboxData.MailboxGuid, false));
					if (string.Equals(updateQueryContext.Request.ApplicationId, "Outlook"))
					{
						itemType.ItemId.Id = storeObjectId.ToHexEntryId();
					}
					itemType.HasAttachments = new bool?(InstantSearchSession.TryGetValueOrDefault<bool>(propertyBag, ItemSchema.HasAttachment, false));
					ConversationId conversationId = InstantSearchSession.TryGetValueOrDefault<ConversationId>(propertyBag, ItemSchema.ConversationId, null);
					itemType.ConversationId = new Microsoft.Exchange.Services.Core.Types.ItemId(IdConverter.ConversationIdToEwsId(this.mailboxData.MailboxGuid, conversationId), null);
					itemType.Subject = InstantSearchSession.TryGetValueOrDefault<string>(propertyBag, ItemSchema.Subject, null);
					itemType.ItemClass = InstantSearchSession.TryGetValueOrDefault<string>(propertyBag, StoreObjectSchema.ItemClass, null);
					string name = InstantSearchSession.TryGetValueOrDefault<string>(propertyBag, MessageItemSchema.SenderDisplayName, null);
					if (itemType is MessageType)
					{
						((MessageType)itemType).From = new SingleRecipientType();
						((MessageType)itemType).From.Mailbox = new EmailAddressWrapper();
						((MessageType)itemType).From.Mailbox.Name = name;
					}
					itemType.DisplayTo = InstantSearchSession.TryGetValueOrDefault<string>(propertyBag, ItemSchema.DisplayTo, null);
					itemType.DateTimeReceived = InstantSearchSession.GetDateTimeProperty(propertyBag, this.mailboxData.TimeZone, ItemSchema.ReceivedTime);
					Importance value = InstantSearchSession.TryGetValueOrDefault<Importance>(propertyBag, ItemSchema.Importance, Importance.Normal);
					itemType.Importance = (ImportanceType)Enum.ToObject(typeof(ImportanceType), (int)value);
					FlagStatus flagStatus = InstantSearchSession.TryGetValueOrDefault<FlagStatus>(propertyBag, InternalSchema.FlagStatus, FlagStatus.NotFlagged);
					if (flagStatus != FlagStatus.NotFlagged)
					{
						itemType.Flag = new FlagType();
						itemType.Flag.FlagStatus = flagStatus;
					}
					InstantSearchSession.TryGetValueOrDefault<IconIndex>(propertyBag, ItemSchema.IconIndex, IconIndex.Default);
					itemType.Preview = InstantSearchSession.TryGetValueOrDefault<string>(propertyBag, ItemSchema.Preview, null);
					list.Add(itemType);
				}
				catch (Exception ex)
				{
					this.GenerateErrorPayload("SafeOnItemsReceived", ex, updateQueryContext.Request.SearchRequestId, updateQueryContext.NotificationHandler);
				}
			}
			return list.ToArray();
		}

		public void OnSuggestionsAvailable(IReadOnlyCollection<QuerySuggestion> querySuggestions, ICancelableAsyncResult aSyncResult)
		{
			InstantSearchRequestContext updateQueryContext = (InstantSearchRequestContext)aSyncResult.AsyncState;
			this.AnnotateWithTimeStampsAndDeliverPayload(updateQueryContext, QueryOptionsType.Suggestions, !updateQueryContext.Request.WaitOnSearchResults, delegate(SearchPerfMarkerContainer perfMarkerContainer)
			{
				SearchSuggestionType[] array = new SearchSuggestionType[querySuggestions.Count];
				int num = 0;
				foreach (QuerySuggestion querySuggestion in querySuggestions)
				{
					array[num++] = new SearchSuggestionType(querySuggestion.SuggestedQuery, querySuggestion.Weight, (SuggestionSourceType)Enum.ToObject(typeof(SuggestionSourceType), (int)querySuggestion.Source));
				}
				return new InstantSearchPayloadType(updateQueryContext.Request.SearchSessionId, updateQueryContext.Request.SearchRequestId, InstantSearchResultType.Suggestions, perfMarkerContainer)
				{
					SearchSuggestions = array
				};
			});
		}

		private void OnStartSessionComplete(IAsyncResult ar)
		{
			long num = (long)ar.AsyncState;
			try
			{
				this.instantSearch.EndStartSession(ar);
			}
			catch (InstantSearchPermanentException)
			{
			}
			catch (InstantSearchTransientException)
			{
			}
			catch (Exception)
			{
			}
		}

		private void OnStopSessionComplete(IAsyncResult ar)
		{
			long num = (long)ar.AsyncState;
			try
			{
				this.instantSearch.EndStopSession(ar);
			}
			catch (InstantSearchPermanentException)
			{
			}
			catch (InstantSearchTransientException)
			{
			}
			catch (Exception)
			{
			}
			this.instantSearch.Dispose();
		}

		private void OnCompleteInstantSearchRequest(IAsyncResult result)
		{
			InstantSearchRequestContext instantSearchRequestContext = (InstantSearchRequestContext)result.AsyncState;
			bool flag = false;
			QueryStatistics queryStatistics = null;
			string text = null;
			try
			{
				queryStatistics = this.instantSearch.EndInstantSearchRequest((ICancelableAsyncResult)result);
			}
			catch (InstantSearchPermanentException ex)
			{
				flag = true;
				queryStatistics = ex.QueryStatistics;
				text = this.GetErrorMessage(ex);
			}
			catch (InstantSearchTransientException ex2)
			{
				flag = true;
				queryStatistics = ex2.QueryStatistics;
				text = this.GetErrorMessage(ex2);
			}
			catch (Exception ex3)
			{
				flag = true;
				text = this.GetErrorMessage(ex3);
			}
			InstantSearchResultType resultDataType = (flag ? InstantSearchResultType.Errors : InstantSearchResultType.None) | ((queryStatistics == null) ? InstantSearchResultType.None : InstantSearchResultType.QueryStatistics);
			InstantSearchPayloadType instantSearchPayloadType = new InstantSearchPayloadType(this.searchSessionId, instantSearchRequestContext.Request.SearchRequestId, resultDataType, new SearchPerfMarkerContainer());
			instantSearchPayloadType.QueryProcessingComplete = true;
			if (queryStatistics != null)
			{
				instantSearchPayloadType.QueryStatistics = new QueryStatisticsType(queryStatistics);
			}
			if (text != null)
			{
				instantSearchPayloadType.Errors = new string[]
				{
					text
				};
			}
			if (instantSearchRequestContext.Request.WaitOnSearchResults)
			{
				lock (instantSearchRequestContext)
				{
					if (!instantSearchRequestContext.ResponseSent)
					{
						instantSearchRequestContext.ResponseSent = true;
						if (flag)
						{
							instantSearchRequestContext.Error = text;
						}
						else
						{
							instantSearchRequestContext.Response = new PerformInstantSearchResponse();
						}
						instantSearchRequestContext.SearchResultsReceivedEvent.Set();
					}
				}
			}
			instantSearchRequestContext.NotificationHandler.DeliverInstantSearchPayload(instantSearchPayloadType);
		}

		internal static void SafeLogData(CallContext callContext, Enum key, object value)
		{
			if (callContext != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(callContext.ProtocolLog, key, value);
			}
		}

		private static Microsoft.Exchange.Services.Core.Types.ItemId StoreIdToEwsItemId(StoreId storeId, MailboxId mailboxId)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, mailboxId, null);
			return new Microsoft.Exchange.Services.Core.Types.ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
		}

		private QueryFilter BuildAdditionalFiltersQuery(IReadOnlyList<StoreId> folderScope, bool isDeepTraversal, RestrictionType dateRestriction)
		{
			List<QueryFilter> list = new List<QueryFilter>(2);
			if (dateRestriction != null && dateRestriction.Item != null)
			{
				ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
				list.Add(serviceObjectToFilterConverter.Convert(dateRestriction.Item));
			}
			if (folderScope.Count == 1 && folderScope[0].Equals(this.mailboxData.RootFolderId))
			{
				if (this.exclusionFilters == null)
				{
					lock (this.dataLock)
					{
						if (this.exclusionFilters == null)
						{
							this.exclusionFilters = this.mailboxData.ExcludedFoldersQueryFragment;
						}
					}
				}
				list.AddRange(this.exclusionFilters);
			}
			int count = list.Count;
			if (count == 0)
			{
				return null;
			}
			if (count == 1)
			{
				return list[0];
			}
			return new AndFilter(list.ToArray());
		}

		private void GenerateErrorPayload(string context, Exception ex, long requestId, IInstantSearchNotificationHandler notificationHandler)
		{
			this.GenerateErrorPayload(context, this.GetErrorMessage(ex), requestId, notificationHandler);
		}

		private string GetErrorMessage(Exception ex)
		{
			string text = ex.ToString();
			if (ex.InnerException != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(text);
				stringBuilder.Append(ex.InnerException.ToString());
				text = stringBuilder.ToString();
			}
			return text;
		}

		private void GenerateErrorPayload(string context, string errorMessage, long requestId, IInstantSearchNotificationHandler notificationHandler)
		{
			notificationHandler.DeliverInstantSearchPayload(new InstantSearchPayloadType(this.SessionId, requestId, InstantSearchResultType.Errors, new SearchPerfMarkerContainer())
			{
				Errors = new string[]
				{
					context + "\n" + errorMessage
				}
			});
		}

		private static T TryGetValueOrDefault<T>(IReadOnlyPropertyBag propertyBag, PropertyDefinition propertyDefinition, T defaultValue)
		{
			IStorePropertyBag storePropertyBag = propertyBag as IStorePropertyBag;
			object obj;
			if (storePropertyBag == null)
			{
				obj = propertyBag[propertyDefinition];
			}
			else
			{
				obj = storePropertyBag.TryGetProperty(propertyDefinition);
			}
			if (obj == null || obj is PropertyError || !(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private static string GetDateTimeProperty(IReadOnlyPropertyBag propertyBag, ExTimeZone timeZone, PropertyDefinition propertyDefinition)
		{
			ExDateTime exDateTime = InstantSearchSession.TryGetValueOrDefault<ExDateTime>(propertyBag, propertyDefinition, ExDateTime.MinValue);
			if (ExDateTime.MinValue.Equals(exDateTime))
			{
				return null;
			}
			return ExDateTimeConverter.ToOffsetXsdDateTime(exDateTime, timeZone);
		}

		private void AnnotateWithTimeStampsAndDeliverPayload(InstantSearchRequestContext updateQueryContext, QueryOptionsType queryOption, bool aSync, InstantSearchSession.CreatePayload invokeDelegate)
		{
			if (updateQueryContext.Request.IsWarmUpRequest)
			{
				return;
			}
			SearchPerfMarkerContainer deepCopy = updateQueryContext.PerfMarkers.GetDeepCopy();
			deepCopy.SetPerfMarker(InstantSearchPerfKey.InstantSearchAPICallback);
			InstantSearchPayloadType instantSearchPayloadType = invokeDelegate(deepCopy);
			instantSearchPayloadType.SearchPerfMarkerContainer.SetPerfMarker(InstantSearchPerfKey.NotificationHandlerPayloadDeliveryTimeStamp);
			bool flag = true;
			if (!aSync)
			{
				lock (updateQueryContext)
				{
					if (!updateQueryContext.ResponseSent)
					{
						flag = false;
						updateQueryContext.ResponseSent = true;
						updateQueryContext.Response = new PerformInstantSearchResponse(instantSearchPayloadType);
						if (updateQueryContext.SearchTerms != null)
						{
							instantSearchPayloadType.SearchTerms = updateQueryContext.SearchTerms;
						}
						updateQueryContext.SearchResultsReceivedEvent.Set();
					}
				}
			}
			if (flag)
			{
				updateQueryContext.NotificationHandler.DeliverInstantSearchPayload(instantSearchPayloadType);
			}
			lock (this.dataLock)
			{
				this.sessionWidePerfDataCollection[updateQueryContext.Request.SearchRequestId].Add(new SearchPathSnapshotType(queryOption, instantSearchPayloadType.SearchPerfMarkerContainer));
			}
		}

		internal const long ExplicitEndSessionCall = -1L;

		internal const long EndSessionDueToDispose = -2L;

		private const int SearchTimeOut = 30000;

		private const int DefaultMaxRefinersCount = 20;

		private static readonly StoreId[] EmptyStoreId = new StoreId[0];

		private static readonly string[] EmptyStringArray = new string[0];

		private readonly string searchSessionId;

		private readonly InstantSearch instantSearch;

		private readonly object dataLock = new object();

		private Dictionary<long, List<SearchPathSnapshotType>> sessionWidePerfDataCollection = new Dictionary<long, List<SearchPathSnapshotType>>();

		private readonly InstantSearchMailboxDataSnapshot mailboxData;

		private volatile ComparisonFilter[] exclusionFilters;

		private delegate InstantSearchPayloadType CreatePayload(SearchPerfMarkerContainer perfMarkerContainer);

		private static class InstantSearchPropertyDefinitionListFactory
		{
			public static ICollection<PropertyDefinition> GetPropertyListForItemType(InstantSearchItemType itemType)
			{
				if (itemType == InstantSearchItemType.CalendarItem)
				{
					return InstantSearchSession.InstantSearchPropertyDefinitionListFactory.calendarItemProperties;
				}
				return null;
			}

			private static readonly ICollection<PropertyDefinition> calendarItemProperties = new List<PropertyDefinition>
			{
				ItemSchema.Id,
				StoreObjectSchema.ParentItemId,
				ItemSchema.Subject,
				CalendarItemInstanceSchema.StartTime,
				CalendarItemInstanceSchema.EndTime,
				CalendarItemBaseSchema.Organizer,
				CalendarItemBaseSchema.AppointmentRecurring,
				CalendarItemBaseSchema.IsMeeting,
				ItemSchema.Sensitivity,
				ItemSchema.HasAttachment,
				ItemSchema.ReceivedTime,
				ItemSchema.Codepage
			}.Union(InternalRecurrence.RequiredRecurrenceProperties);
		}
	}
}
