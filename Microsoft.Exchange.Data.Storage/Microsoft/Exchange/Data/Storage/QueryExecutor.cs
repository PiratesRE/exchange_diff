using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class QueryExecutor
	{
		internal QueryExecutor(StoreSession session, QueryExecutor.GetMapiFolderDelegate getMapiFolderDelegate)
		{
			this.session = session;
			this.getMapiFolderDelegate = getMapiFolderDelegate;
		}

		public QueryResult ItemQuery(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			return this.ItemQuery(queryFlags, queryFilter, sortColumns, (ICollection<PropertyDefinition>)dataColumns);
		}

		public virtual QueryResult ItemQuery(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			Util.ThrowOnNullArgument(dataColumns, "dataColumns");
			return this.InternalItemQuery(QueryExecutor.ItemQueryTypeToContentsTableFlags(queryFlags), queryFilter, sortColumns, QueryExclusionType.Row, dataColumns, null);
		}

		public QueryResult FolderQuery(FolderQueryFlags queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			return this.FolderQuery(queryFlags, queryFilter, sortColumns, (ICollection<PropertyDefinition>)dataColumns);
		}

		public QueryResult FolderQuery(FolderQueryFlags queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			EnumValidator.ThrowIfInvalid<FolderQueryFlags>(queryFlags, "queryFlags");
			Util.ThrowOnNullArgument(dataColumns, "dataColumns");
			HierarchyTableFlags hierarchyTableFlags = HierarchyTableFlags.None;
			if ((queryFlags & FolderQueryFlags.NoNotifications) == FolderQueryFlags.NoNotifications)
			{
				hierarchyTableFlags |= HierarchyTableFlags.NoNotifications;
			}
			if ((queryFlags & FolderQueryFlags.SuppressNotificationsOnMyActions) == FolderQueryFlags.SuppressNotificationsOnMyActions)
			{
				hierarchyTableFlags |= HierarchyTableFlags.SuppressNotificationsOnMyActions;
			}
			if ((queryFlags & FolderQueryFlags.SoftDeleted) == FolderQueryFlags.SoftDeleted)
			{
				hierarchyTableFlags |= HierarchyTableFlags.ShowSoftDeletes;
			}
			if ((queryFlags & FolderQueryFlags.DeepTraversal) == FolderQueryFlags.DeepTraversal)
			{
				hierarchyTableFlags |= HierarchyTableFlags.ConvenientDepth;
			}
			else
			{
				hierarchyTableFlags |= HierarchyTableFlags.DeferredErrors;
			}
			if ((queryFlags & FolderQueryFlags.DeepTraversal) == FolderQueryFlags.DeepTraversal && sortColumns != null)
			{
				throw new NotSupportedException(ServerStrings.ExSortNotSupportedInDeepTraversalQuery);
			}
			MapiTable mapiTable = null;
			QueryResult queryResult = null;
			bool flag = false;
			QueryResult result;
			try
			{
				StoreSession storeSession = this.session;
				bool flag2 = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiTable = this.MapiFolder.GetHierarchyTable(hierarchyTableFlags);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetHierarchyTable, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::FolderQuery.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetHierarchyTable, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::FolderQuery.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag2)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				queryResult = this.CreateQueryResult(mapiTable, queryFilter, sortColumns, dataColumns);
				flag = true;
				result = queryResult;
			}
			finally
			{
				if (!flag)
				{
					if (queryResult != null)
					{
						queryResult.Dispose();
						queryResult = null;
					}
					if (mapiTable != null)
					{
						mapiTable.Dispose();
						mapiTable = null;
					}
				}
			}
			return result;
		}

		public GroupedQueryResult GroupedItemQuery(QueryFilter queryFilter, PropertyDefinition groupBy, GroupSort groupSort, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			if (groupBy == null)
			{
				throw new ArgumentNullException("groupBy");
			}
			if (groupSort == null)
			{
				throw new ArgumentNullException("groupSort");
			}
			if (dataColumns == null)
			{
				throw new ArgumentNullException("dataColumns");
			}
			int rowTypeColumnIndex = 0;
			bool flag = false;
			for (int i = 0; i < dataColumns.Length; i++)
			{
				if (InternalSchema.RowType.Equals(dataColumns[i]))
				{
					rowTypeColumnIndex = i;
					flag = true;
					break;
				}
			}
			PropertyDefinition[] array = dataColumns;
			if (!flag)
			{
				array = new PropertyDefinition[dataColumns.Length + 1];
				dataColumns.CopyTo(array, 0);
				array[dataColumns.Length] = InternalSchema.RowType;
				rowTypeColumnIndex = dataColumns.Length;
			}
			MapiTable mapiTable = this.GetContentsTable(ContentsTableFlags.DeferredErrors);
			GroupedQueryResult groupedQueryResult = null;
			bool flag2 = false;
			int estimatedItemCount = 0;
			GroupedQueryResult result;
			try
			{
				List<PropTag> alteredProperties = null;
				this.SetTableFilter(mapiTable, queryFilter);
				StoreSession storeSession = this.session;
				bool flag3 = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag3 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					estimatedItemCount = mapiTable.GetRowCount();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRowCount, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::GroupedItemQuery.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRowCount, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::GroupedItemQuery.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag3)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				SortOrder sortOrder = this.GetSortOrder(mapiTable, sortColumns, new GroupByAndOrder[]
				{
					new GroupByAndOrder(groupBy, groupSort)
				}, 1, ref alteredProperties);
				groupedQueryResult = new GroupedQueryResult(mapiTable, array, alteredProperties, rowTypeColumnIndex, flag, this.Session, estimatedItemCount, sortOrder);
				if (this.Session != null)
				{
					this.Session.MessagesWereDownloaded = true;
				}
				flag2 = true;
				result = groupedQueryResult;
			}
			finally
			{
				if (!flag2)
				{
					if (groupedQueryResult != null)
					{
						groupedQueryResult.Dispose();
						groupedQueryResult = null;
					}
					if (mapiTable != null)
					{
						mapiTable.Dispose();
						mapiTable = null;
					}
				}
			}
			return result;
		}

		public virtual QueryResult GroupedItemQuery(QueryFilter queryFilter, ItemQueryType queryFlags, GroupByAndOrder[] groupBy, int expandCount, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			EnumValidator.ThrowIfInvalid<ItemQueryType>(queryFlags, "queryFlags");
			if (groupBy == null)
			{
				throw new ArgumentNullException("groupBy");
			}
			if (dataColumns == null)
			{
				throw new ArgumentNullException("dataColumns");
			}
			if (expandCount > groupBy.Length)
			{
				throw new ArgumentException("expandCount should be less or equal to groupBy lengths");
			}
			ContentsTableFlags flags = QueryExecutor.ItemQueryTypeToContentsTableFlags(queryFlags) | ContentsTableFlags.DeferredErrors;
			MapiTable mapiTable = this.GetContentsTable(flags);
			QueryResult queryResult = null;
			bool flag = false;
			QueryResult result;
			try
			{
				List<PropTag> alteredProperties = null;
				this.SetTableFilter(mapiTable, queryFilter);
				SortOrder sortOrder = this.GetSortOrder(mapiTable, sortColumns, groupBy, expandCount, ref alteredProperties);
				queryResult = new QueryResult(mapiTable, dataColumns, alteredProperties, this.Session, true, sortOrder);
				if (this.Session != null)
				{
					this.Session.MessagesWereDownloaded = true;
				}
				flag = true;
				result = queryResult;
			}
			finally
			{
				if (!flag)
				{
					if (queryResult != null)
					{
						queryResult.Dispose();
						queryResult = null;
					}
					if (mapiTable != null)
					{
						mapiTable.Dispose();
						mapiTable = null;
					}
				}
			}
			return result;
		}

		private static SortBy[] GetNativeSortBy(SortBy[] originalSortBy)
		{
			List<SortBy> list = new List<SortBy>();
			for (int i = 0; i < originalSortBy.Length; i++)
			{
				SortBy sortBy = originalSortBy[i];
				if (sortBy == null)
				{
					ExTraceGlobals.StorageTracer.TraceError<int>(0L, "Folder.GetNativeSortBy. SortColumns[i] should not be Null. i = {0}.", i);
					throw new ArgumentException(ServerStrings.ExNullSortOrderParameter(i));
				}
				StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(sortBy.ColumnDefinition);
				SortBy[] nativeSortBy = storePropertyDefinition.GetNativeSortBy(sortBy.SortOrder);
				list.AddRange(nativeSortBy);
			}
			return list.ToArray();
		}

		private static PropTag SetMviFlagToMultivaluePropTag(PropTag propTag, ref List<PropTag> alteredProperties)
		{
			if ((propTag & (PropTag)4096U) == (PropTag)4096U)
			{
				if (alteredProperties == null)
				{
					alteredProperties = new List<PropTag>();
				}
				alteredProperties.Add(propTag);
				propTag |= (PropTag)12288U;
			}
			return propTag;
		}

		private static SortFlags SortOrderToSortFlags(SortOrder order)
		{
			switch (order)
			{
			case SortOrder.Ascending:
				return SortFlags.Ascend;
			case SortOrder.Descending:
				return SortFlags.Descend;
			default:
				throw new InvalidOperationException(ServerStrings.ExInvalidSortOrder);
			}
		}

		private static Set<NativeStorePropertyDefinition> CreateMessageHeaderPropertyDefinitionsSet()
		{
			return new Set<NativeStorePropertyDefinition>
			{
				InternalSchema.EntryId,
				InternalSchema.ParentEntryId,
				InternalSchema.DeletedOnTime,
				InternalSchema.SourceKey,
				InternalSchema.ChangeKey,
				InternalSchema.LastModifiedTime,
				InternalSchema.MapiHasAttachment,
				InternalSchema.Flags,
				InternalSchema.ItemClass,
				InternalSchema.MapiImportance,
				InternalSchema.MapiSensitivity,
				InternalSchema.MessageStatus,
				InternalSchema.ArticleId,
				InternalSchema.ReceivedTime,
				InternalSchema.CreationTime,
				InternalSchema.SentTime,
				InternalSchema.SearchKey,
				InternalSchema.SecureSubmitFlags,
				InternalSchema.ReplyTemplateId,
				InternalSchema.Size,
				InternalSchema.SubjectPrefixInternal,
				InternalSchema.NormalizedSubjectInternal,
				InternalSchema.InternetMessageId,
				InternalSchema.ConversationTopic,
				InternalSchema.ConversationIndex,
				InternalSchema.SentRepresentingDisplayName,
				InternalSchema.SentRepresentingEmailAddress,
				InternalSchema.IsResponseRequested,
				InternalSchema.MapiToDoItemFlag,
				InternalSchema.IconIndex,
				InternalSchema.MapiFlagStatus,
				InternalSchema.ReplyTime,
				InternalSchema.MapiPRStartDate,
				InternalSchema.MapiPREndDate,
				InternalSchema.ExpiryTime,
				InternalSchema.ContentFilterPcl,
				InternalSchema.ElcAutoCopyTag,
				InternalSchema.ElcMoveDate,
				InternalSchema.AssistantPhoneNumber,
				InternalSchema.FaxNumber,
				InternalSchema.BusinessHomePage,
				InternalSchema.BusinessPhoneNumber,
				InternalSchema.BusinessPhoneNumber2,
				InternalSchema.CarPhone,
				InternalSchema.OrganizationMainPhone,
				InternalSchema.CompanyName,
				InternalSchema.Department,
				InternalSchema.DisplayName,
				InternalSchema.DisplayNamePrefix,
				InternalSchema.GivenName,
				InternalSchema.MiddleName,
				InternalSchema.Surname,
				InternalSchema.Generation,
				InternalSchema.Title,
				InternalSchema.HomeFax,
				InternalSchema.HomePhone,
				InternalSchema.HomePhone2,
				InternalSchema.MobilePhone,
				InternalSchema.OtherTelephone,
				InternalSchema.Pager,
				InternalSchema.PersonalHomePage,
				InternalSchema.OtherFax,
				InternalSchema.PrimaryTelephoneNumber,
				InternalSchema.TtyTddPhoneNumber,
				InternalSchema.RuleId,
				InternalSchema.RuleFolderEntryId,
				InternalSchema.RuleError,
				InternalSchema.RuleIds,
				InternalSchema.RuleProvider,
				InternalSchema.OwnerAppointmentID,
				InternalSchema.InReplyTo,
				InternalSchema.ImapInternalDate,
				InternalSchema.Not822Renderable,
				InternalSchema.PopImapPoisonMessageStamp,
				InternalSchema.FileAsStringInternal,
				InternalSchema.HomeAddressInternal,
				InternalSchema.BusinessAddressInternal,
				InternalSchema.OtherAddressInternal,
				InternalSchema.OutlookCardDesign,
				InternalSchema.UserText1,
				InternalSchema.UserText2,
				InternalSchema.UserText3,
				InternalSchema.UserText4,
				InternalSchema.IMAddress,
				InternalSchema.Email1OriginalDisplayName,
				InternalSchema.Email2OriginalDisplayName,
				InternalSchema.Email3OriginalDisplayName,
				InternalSchema.FileAsId,
				InternalSchema.Email1DisplayName,
				InternalSchema.Email1AddrType,
				InternalSchema.Email1EmailAddress,
				InternalSchema.Email1OriginalEntryID,
				InternalSchema.Email2DisplayName,
				InternalSchema.Email2AddrType,
				InternalSchema.Email2EmailAddress,
				InternalSchema.Email2OriginalEntryID,
				InternalSchema.Email3DisplayName,
				InternalSchema.Email3AddrType,
				InternalSchema.Email3EmailAddress,
				InternalSchema.Email3OriginalEntryID,
				InternalSchema.FreeBusyStatus,
				InternalSchema.Location,
				InternalSchema.AppointmentColor,
				InternalSchema.AppointmentRecurrenceBlob,
				InternalSchema.AppointmentStateInternal,
				InternalSchema.AppointmentRecurring,
				InternalSchema.MapiStartTime,
				InternalSchema.MapiEndTime,
				InternalSchema.TimeZoneBlob,
				InternalSchema.MapiIsAllDayEvent,
				InternalSchema.ClipStartTime,
				InternalSchema.ClipEndTime,
				InternalSchema.MeetingRequestWasSent,
				InternalSchema.MeetingWorkspaceUrl,
				InternalSchema.MapiResponseType,
				InternalSchema.AppointmentSequenceNumber,
				InternalSchema.FlagRequest,
				InternalSchema.ReminderDueByInternal,
				InternalSchema.ReminderNextTime,
				InternalSchema.ReminderIsSetInternal,
				InternalSchema.SideEffects,
				InternalSchema.AllAttachmentsHidden,
				InternalSchema.UtcStartDate,
				InternalSchema.UtcDueDate,
				InternalSchema.OutlookInternalVersion,
				InternalSchema.XMsExchOrganizationAuthDomain,
				InternalSchema.CleanGlobalObjectId,
				InternalSchema.IsException,
				InternalSchema.GlobalObjectId,
				InternalSchema.IsRecurring,
				InternalSchema.Categories,
				InternalSchema.OutlookPhishingStamp,
				InternalSchema.OutlookSpoofingStamp,
				InternalSchema.TaskStatus,
				InternalSchema.LocalStartDate,
				InternalSchema.LocalDueDate,
				InternalSchema.IsComplete,
				InternalSchema.MapiConversationId,
				InternalSchema.BodyTag,
				InternalSchema.DisplayCcInternal,
				InternalSchema.DisplayBccInternal,
				InternalSchema.DisplayToInternal
			};
		}

		internal QueryResult InternalItemQuery(ContentsTableFlags flags, QueryFilter queryFilter, SortBy[] sortColumns, QueryExclusionType queryExclusionType, ICollection<PropertyDefinition> dataColumns, AggregationExtension aggregationExtension)
		{
			Util.ThrowOnNullArgument(dataColumns, "dataColumns");
			if ((flags & ContentsTableFlags.ExpandedConversationView) == ContentsTableFlags.ExpandedConversationView)
			{
				if ((flags & ContentsTableFlags.ShowConversations) != ContentsTableFlags.ShowConversations)
				{
					throw new ArgumentException("flags", "ContentsTableFlags.ExpandedConversationView requires ContentsTableFlags.ShowConversations");
				}
				foreach (PropertyDefinition propertyDefinition in dataColumns)
				{
					if (!(propertyDefinition is ApplicationAggregatedProperty))
					{
						throw new ArgumentException("dataColumns", "All columns must be ApplicationAggregatedProperty. Offending dataColumn: " + propertyDefinition.Name + ":" + propertyDefinition.GetType().Name);
					}
				}
			}
			flags |= ContentsTableFlags.DeferredErrors;
			MapiTable mapiTable = this.GetContentsTable(flags);
			QueryResult queryResult = null;
			bool flag = false;
			QueryResult result;
			try
			{
				MailboxSession mailboxSession = this.Session as MailboxSession;
				if (mailboxSession != null && mailboxSession.FilterPrivateItems && queryExclusionType == QueryExclusionType.Row)
				{
					if (queryFilter != null)
					{
						queryFilter = new AndFilter(true, new QueryFilter[]
						{
							QueryExecutor.privateItemsFilter,
							queryFilter
						});
					}
					else if ((flags & ContentsTableFlags.ShowSoftDeletes) != ContentsTableFlags.ShowSoftDeletes)
					{
						queryFilter = QueryExecutor.privateItemsFilter;
					}
				}
				if ((flags & ContentsTableFlags.ExpandedConversationView) == ContentsTableFlags.ExpandedConversationView)
				{
					queryResult = this.CreateConversationMembersQueryResult(mapiTable, queryFilter, sortColumns, dataColumns, aggregationExtension);
				}
				else
				{
					queryResult = this.CreateQueryResult(mapiTable, queryFilter, sortColumns, dataColumns);
				}
				if (this.Session != null)
				{
					this.Session.MessagesWereDownloaded = true;
				}
				flag = true;
				result = queryResult;
			}
			finally
			{
				if (!flag)
				{
					if (queryResult != null)
					{
						queryResult.Dispose();
						queryResult = null;
					}
					if (mapiTable != null)
					{
						mapiTable.Dispose();
						mapiTable = null;
					}
				}
			}
			return result;
		}

		internal event QueryExecutor.ContentsTableAccessedEventHandler OnContentsTableAccessed;

		internal static ContentsTableFlags ItemQueryTypeToContentsTableFlags(ItemQueryType itemQueryFlags)
		{
			EnumValidator.ThrowIfInvalid<ItemQueryType>(itemQueryFlags, "itemQueryFlags");
			ContentsTableFlags contentsTableFlags = ContentsTableFlags.None;
			if ((itemQueryFlags & ItemQueryType.Associated) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.Associated;
			}
			if ((itemQueryFlags & ItemQueryType.SoftDeleted) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.ShowSoftDeletes;
			}
			if ((itemQueryFlags & ItemQueryType.RetrieveFromIndex) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.RetrieveFromIndex;
			}
			if ((itemQueryFlags & ItemQueryType.ConversationViewMembers) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.ShowConversationMembers;
			}
			if ((itemQueryFlags & ItemQueryType.ConversationView) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.ShowConversations;
			}
			if ((itemQueryFlags & ItemQueryType.DocumentIdView) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.DocumentIdView;
			}
			if ((itemQueryFlags & ItemQueryType.PrereadExtendedProperties) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.PrereadExtendedProperties;
			}
			if ((itemQueryFlags & ItemQueryType.NoNotifications) != ItemQueryType.None)
			{
				contentsTableFlags |= ContentsTableFlags.NoNotifications;
			}
			return contentsTableFlags;
		}

		private MapiFolder MapiFolder
		{
			get
			{
				return this.getMapiFolderDelegate();
			}
		}

		private QueryResult CreateQueryResult(MapiTable mapiTable, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			List<PropTag> alteredProperties = null;
			this.SetTableFilter(mapiTable, queryFilter);
			SortOrder sortOrder = this.GetSortOrder(mapiTable, sortColumns, null, 0, ref alteredProperties);
			return new QueryResult(mapiTable, dataColumns, alteredProperties, this.Session, true, sortOrder);
		}

		private QueryResult CreateConversationMembersQueryResult(MapiTable mapiTable, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns, AggregationExtension aggregationExtension)
		{
			List<PropTag> alteredProperties = null;
			this.SetTableFilter(mapiTable, queryFilter);
			SortOrder sortOrder = this.GetSortOrder(mapiTable, sortColumns, null, 0, ref alteredProperties);
			return new ConversationMembersQueryResult(mapiTable, dataColumns, alteredProperties, this.Session, true, sortOrder, aggregationExtension);
		}

		private MapiTable GetContentsTable(ContentsTableFlags flags)
		{
			if (this.OnContentsTableAccessed != null)
			{
				this.OnContentsTableAccessed();
			}
			StoreSession storeSession = this.session;
			bool flag = false;
			MapiTable result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				MapiTable contentsTable = this.MapiFolder.GetContentsTable(flags);
				if (contentsTable == null)
				{
					throw new InvalidOperationException("MapiFolder.GetContentsTable() returned null.  This should never happen, and should be investigated.");
				}
				result = contentsTable;
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::GetContentsTable.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::GetContentsTable.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		private void SetTableFilter(MapiTable mapiTable, QueryFilter queryFilter)
		{
			QueryExecutor.SetTableFilter(this.Session, this.MapiFolder, mapiTable, queryFilter);
		}

		internal static void SetTableFilter(StoreSession session, MapiProp propertyReference, MapiTable mapiTable, QueryFilter queryFilter)
		{
			if (queryFilter != null)
			{
				Restriction restriction = FilterRestrictionConverter.CreateRestriction(session, session.ExTimeZone, propertyReference, queryFilter);
				object thisObject = null;
				bool flag = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiTable.Restrict(restriction);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::SetTableFilter. Failed to set a restriction.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::SetTableFilter. Failed to set a restriction.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (session != null)
						{
							session.EndMapiCall();
							if (flag)
							{
								session.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
		}

		private SortOrder GetSortOrder(MapiTable table, SortBy[] sortColumns, GroupByAndOrder[] groupBy, int expandCount, ref List<PropTag> alteredProperties)
		{
			if ((sortColumns == null || sortColumns.Length == 0) && (groupBy == null || groupBy.Length == 0))
			{
				return null;
			}
			SortOrder sortOrder = new SortOrder();
			if (groupBy != null)
			{
				for (int i = 0; i < groupBy.Length; i++)
				{
					NativeStorePropertyDefinition nativeGroupBy = InternalSchema.ToStorePropertyDefinition(groupBy[i].GroupByColumn).GetNativeGroupBy();
					PropTag sortPropertyTag = this.GetSortPropertyTag(nativeGroupBy, ref alteredProperties);
					GroupSort nativeGroupSort = InternalSchema.ToStorePropertyDefinition(groupBy[i].GroupSortColumn.ColumnDefinition).GetNativeGroupSort(groupBy[i].GroupSortColumn.SortOrder, groupBy[i].GroupSortColumn.Aggregate);
					PropTag sortPropertyTag2 = this.GetSortPropertyTag((NativeStorePropertyDefinition)nativeGroupSort.ColumnDefinition, ref alteredProperties);
					SortFlags sortFlag;
					switch (nativeGroupSort.Aggregate)
					{
					case Aggregate.Min:
						sortFlag = SortFlags.CategoryMin;
						break;
					case Aggregate.Max:
						sortFlag = SortFlags.CategoryMax;
						break;
					default:
						throw new ArgumentOutOfRangeException("groupBy", nativeGroupSort.Aggregate, ServerStrings.ExInvalidAggregate);
					}
					sortOrder.AddCategory(sortPropertyTag, QueryExecutor.SortOrderToSortFlags(nativeGroupSort.SortOrder));
					if (sortPropertyTag != sortPropertyTag2)
					{
						sortOrder.Add(sortPropertyTag2, sortFlag);
					}
				}
				sortOrder.ExpandCount = expandCount;
			}
			if (sortOrder.GetSortCount() > 4)
			{
				throw new ArgumentException(ServerStrings.ExTooComplexGroupSortParameter, "groupBy");
			}
			if (sortColumns != null && sortColumns.Length > 0)
			{
				SortBy[] nativeSortBy = QueryExecutor.GetNativeSortBy(sortColumns);
				if (nativeSortBy.Length + sortOrder.GetSortCount() > 6)
				{
					throw new ArgumentOutOfRangeException("sortColumns", ServerStrings.ExTooManySortColumns);
				}
				for (int j = 0; j < nativeSortBy.Length; j++)
				{
					PropertyDefinition columnDefinition = nativeSortBy[j].ColumnDefinition;
					PropTag sortPropertyTag3 = this.GetSortPropertyTag((NativeStorePropertyDefinition)columnDefinition, ref alteredProperties);
					sortOrder.Add(sortPropertyTag3, QueryExecutor.SortOrderToSortFlags(nativeSortBy[j].SortOrder));
				}
			}
			return sortOrder;
		}

		private PropTag GetSortPropertyTag(NativeStorePropertyDefinition propertyDefinition, ref List<PropTag> alteredProperties)
		{
			PropTag propTag = PropertyTagCache.Cache.PropTagFromPropertyDefinition(this.MapiFolder, this.Session, propertyDefinition);
			return QueryExecutor.SetMviFlagToMultivaluePropTag(propTag, ref alteredProperties);
		}

		private StoreSession Session
		{
			get
			{
				return this.session;
			}
		}

		internal const int MviFlag = 12288;

		internal const int MultivaluePropTagFlag = 4096;

		public const int MaximumOfCategoriesAndAggregations = 4;

		public const int MaximumOfSortEntries = 6;

		protected readonly StoreSession session;

		private readonly QueryExecutor.GetMapiFolderDelegate getMapiFolderDelegate;

		private static readonly QueryFilter privateItemsFilter = new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.NotEqual, InternalSchema.MapiSensitivity, Sensitivity.Private),
			new NotFilter(new ExistsFilter(InternalSchema.MapiSensitivity))
		});

		internal static readonly Set<NativeStorePropertyDefinition> MessageHeaderPropertyDefinitions = QueryExecutor.CreateMessageHeaderPropertyDefinitionsSet();

		internal delegate void ContentsTableAccessedEventHandler();

		internal delegate MapiFolder GetMapiFolderDelegate();
	}
}
