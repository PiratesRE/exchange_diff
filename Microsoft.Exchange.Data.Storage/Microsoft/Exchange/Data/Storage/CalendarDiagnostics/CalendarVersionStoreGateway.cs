using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarVersionStoreGateway
	{
		private static bool ValidateDateRanges(ref ExDateTime? startDate, ref ExDateTime? endDate)
		{
			if (startDate != null && endDate != null && startDate.Value > endDate.Value)
			{
				return false;
			}
			if (startDate == null)
			{
				if (endDate == null)
				{
					startDate = new ExDateTime?(ExDateTime.Now.AddDays(-80.0));
				}
				else
				{
					startDate = new ExDateTime?(endDate.Value.AddDays(-80.0));
				}
			}
			if (endDate == null)
			{
				endDate = new ExDateTime?(startDate.Value.AddDays(80.0));
			}
			if (!startDate.Value.HasTimeZone)
			{
				startDate = new ExDateTime?(new ExDateTime(ExTimeZone.CurrentTimeZone, startDate.Value.UtcTicks));
			}
			if (!endDate.Value.HasTimeZone)
			{
				endDate = new ExDateTime?(new ExDateTime(ExTimeZone.CurrentTimeZone, endDate.Value.UtcTicks));
			}
			return true;
		}

		public CalendarVersionStoreQueryPolicy Policy { get; private set; }

		public CalendarVersionStoreGateway(CalendarVersionStoreQueryPolicy policy, bool useCachedPropertySetIfPresent)
		{
			this.Policy = policy;
			this.useCachedPropertySetIfPresent = useCachedPropertySetIfPresent;
		}

		public bool CheckCvsFolder(MailboxSession session)
		{
			return this.GetCvsFolderIdIfPresent(session) != null;
		}

		public void QueryBySubjectContains(MailboxSession session, string subject, string schemaKey, StorePropertyDefinition[] propertiesToFetch, Action<PropertyBag> matchFoundAction, ExDateTime? startDate = null, ExDateTime? endDate = null)
		{
			if (!CalendarVersionStoreGateway.ValidateDateRanges(ref startDate, ref endDate))
			{
				throw new ArgumentException("StartDate cannot be greater than EndDate", "startDate");
			}
			using (SearchFolder searchFolder = this.GetSearchFolder(session))
			{
				CalendarCorrelationMatch.QuerySubjectContains(searchFolder, subject, schemaKey, propertiesToFetch, this.useCachedPropertySetIfPresent, matchFoundAction, startDate.Value, endDate.Value);
			}
		}

		public void QueryByCleanGlobalObjectId(MailboxSession session, GlobalObjectId gobalObjectId, string schemaKey, StorePropertyDefinition[] propertiesToFetch, Func<PropertyBag, bool> matchFoundAction, bool fetchResultsInReverseChronologicalOrder, string[] itemClassFilter, ExDateTime? startDate = null, ExDateTime? endDate = null)
		{
			this.DirectQueryByGlobalObjectId(session, gobalObjectId, schemaKey, propertiesToFetch, matchFoundAction, fetchResultsInReverseChronologicalOrder, true, this.useCachedPropertySetIfPresent, itemClassFilter, startDate, endDate);
		}

		public void QueryByGlobalObjectId(MailboxSession session, GlobalObjectId globalObjectId, string schemaKey, StorePropertyDefinition[] propertiesToFetch, Func<PropertyBag, bool> matchFoundAction, bool fetchResultsInReverseChronologicalOrder, string[] itemClassFilter, ExDateTime? startDate = null, ExDateTime? endDate = null)
		{
			if (globalObjectId.IsCleanGlobalObjectId)
			{
				this.DirectQueryByGlobalObjectId(session, globalObjectId, schemaKey, propertiesToFetch, matchFoundAction, fetchResultsInReverseChronologicalOrder, false, this.useCachedPropertySetIfPresent, itemClassFilter, startDate, endDate);
				return;
			}
			Func<PropertyBag, bool> matchFoundAction2 = delegate(PropertyBag masterPropertyBag)
			{
				bool result;
				using (CalendarItemOccurrence occurrenceFromMaster = this.GetOccurrenceFromMaster(masterPropertyBag, globalObjectId.Date, session, propertiesToFetch))
				{
					result = (occurrenceFromMaster == null || matchFoundAction(occurrenceFromMaster.PropertyBag));
				}
				return result;
			};
			this.DirectQueryByGlobalObjectId(session, globalObjectId, "{3CA2A02A-6557-40af-9E0A-A95351BF9572}", Recurrence.RequiredRecurrenceProperties, matchFoundAction2, fetchResultsInReverseChronologicalOrder, true, true, itemClassFilter, startDate, endDate);
		}

		private void DirectQueryByGlobalObjectId(MailboxSession session, GlobalObjectId globalObjectId, string schemaKey, StorePropertyDefinition[] propertiesToFetch, Func<PropertyBag, bool> matchFoundAction, bool fetchResultsInReverseChronologicalOrder, bool useCleanGoid, bool useCachedPropertySetIfPresent, string[] itemClassFilter, ExDateTime? startDate = null, ExDateTime? endDate = null)
		{
			if (!CalendarVersionStoreGateway.ValidateDateRanges(ref startDate, ref endDate))
			{
				throw new ArgumentException("StartDate cannot be greater than EndDate", "startDate");
			}
			using (SearchFolder searchFolder = this.GetSearchFolder(session))
			{
				CalendarCorrelationMatch.QueryRelatedItems(searchFolder, globalObjectId, schemaKey, propertiesToFetch, useCachedPropertySetIfPresent, matchFoundAction, fetchResultsInReverseChronologicalOrder, !useCleanGoid, itemClassFilter, new ExDateTime?(startDate.Value), new ExDateTime?(endDate.Value));
			}
		}

		private CalendarItemOccurrence GetOccurrenceFromMaster(PropertyBag masterPropertyBag, ExDateTime occurrenceId, MailboxSession session, ICollection<PropertyDefinition> requiredProperties)
		{
			Recurrence recurrence;
			if (Recurrence.TryFromMasterPropertyBag(masterPropertyBag, session, out recurrence))
			{
				OccurrenceInfo occurrenceInfo = null;
				try
				{
					occurrenceInfo = recurrence.GetOccurrenceInfoByDateId(occurrenceId);
					return CalendarItemOccurrence.Bind(session, occurrenceInfo.VersionedId, requiredProperties);
				}
				catch (ObjectNotFoundException)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string, VersionedId>((long)this.GetHashCode(), "CalendarVersionStoreGateway::GetOccurrencePropertyBagFromMaster - Could not bind to the occurrence ID (Session Display Name: {0}; VersionedId: {1}).", session.DisplayName, (occurrenceInfo == null) ? null : occurrenceInfo.VersionedId);
					return null;
				}
			}
			ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string>((long)this.GetHashCode(), "CalendarVersionStoreGateway::GetOccurrencePropertyBagFromMaster - Could not get the recurrence object from the master (Session Display Name: {0}).", session.DisplayName);
			return null;
		}

		private SearchFolder GetSearchFolder(MailboxSession session)
		{
			bool flag = false;
			StoreObjectId storeObjectId = this.GetCvsFolderIdIfPresent(session);
			SearchFolder searchFolder = null;
			try
			{
				if (storeObjectId == null)
				{
					storeObjectId = session.CreateDefaultFolder(DefaultFolderType.CalendarVersionStore);
				}
				try
				{
					searchFolder = SearchFolder.Bind(session, storeObjectId);
				}
				catch (ObjectNotFoundException innerException)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceError<string, StoreObjectId>((long)this.GetHashCode(), "CalendarVersionStoreGateway::GetSearchFolder - No item is found with the CVS search folder ID (Session Display Name: {0}; Folder ID: {1}).", session.DisplayName, storeObjectId);
					throw new CalendarVersionStoreNotPopulatedException(false, SearchState.None, TimeSpan.Zero, innerException);
				}
				catch (WrongObjectTypeException innerException2)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceError<string, StoreObjectId>((long)this.GetHashCode(), "CalendarVersionStoreGateway::GetSearchFolder - The CVS search folder ID does not belong to a search folder (Session Display Name: {0}; Folder ID: {1}).", session.DisplayName, storeObjectId);
					throw new CalendarVersionStoreNotPopulatedException(false, SearchState.None, TimeSpan.Zero, innerException2);
				}
				SearchState folderState;
				if (!this.IsSearchFolderPopulated(searchFolder, out folderState))
				{
					if (this.Policy.WaitForPopulation)
					{
						using (SearchFolderAsyncSearch searchFolderAsyncSearch = new SearchFolderAsyncSearch(session, storeObjectId, null, null))
						{
							if (!this.IsSearchFolderPopulated(searchFolder, out folderState) && !searchFolderAsyncSearch.AsyncResult.AsyncWaitHandle.WaitOne(this.Policy.WaitTimeForPopulation))
							{
								ExTraceGlobals.DefaultFoldersTracer.TraceDebug<TimeSpan, string>((long)this.GetHashCode(), "CalendarVersionStoreGateway::GetSearchFolder - The CVS search folder is not populated, and timeout ({0}) is passed (Session Display Name: {1}).", this.Policy.WaitTimeForPopulation, session.DisplayName);
								throw new CalendarVersionStoreNotPopulatedException(true, folderState, this.Policy.WaitTimeForPopulation);
							}
							goto IL_151;
						}
					}
					ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string>((long)this.GetHashCode(), "CalendarVersionStoreGateway::GetSearchFolder - The CVS search folder is not populated, and query policy has no wait time specified (Session Display Name: {0}).", session.DisplayName);
					throw new CalendarVersionStoreNotPopulatedException(true, folderState, TimeSpan.Zero);
				}
				IL_151:
				flag = true;
			}
			finally
			{
				if (!flag && searchFolder != null)
				{
					searchFolder.Dispose();
				}
			}
			return searchFolder;
		}

		private StoreObjectId GetCvsFolderIdIfPresent(MailboxSession session)
		{
			return session.GetDefaultFolderId(DefaultFolderType.CalendarVersionStore);
		}

		private bool IsSearchFolderPopulated(SearchFolder folder, out SearchState state)
		{
			SearchFolderCriteria searchCriteria = folder.GetSearchCriteria();
			state = searchCriteria.SearchState;
			return (searchCriteria.SearchState & SearchState.Rebuild) != SearchState.Rebuild;
		}

		private const string MasterRecurrencePropertySetKey = "{3CA2A02A-6557-40af-9E0A-A95351BF9572}";

		private const int DefaultQueryDateRangeDays = 80;

		private static readonly PropertyDefinition[] cgoidProperty = new PropertyDefinition[]
		{
			InternalSchema.CleanGlobalObjectId
		};

		private bool useCachedPropertySetIfPresent;
	}
}
