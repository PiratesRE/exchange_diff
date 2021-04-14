using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarImporter
	{
		public CalendarImporter(Stream inputStream, string charset, InboundConversionOptions options, CalendarFolder folder, ImportCalendarResults results, Deadline deadline, ExDateTime importWindowStart, ExDateTime importWindowEnd)
		{
			Util.ThrowOnNullArgument(inputStream, "inputStream");
			Util.ThrowOnNullOrEmptyArgument(charset, "charset");
			Util.ThrowOnNullArgument(folder, "folder");
			Util.ThrowOnNullArgument(results, "results");
			Util.ThrowOnNullArgument(deadline, "deadline");
			this.inputStream = inputStream;
			this.charset = charset;
			this.folder = folder;
			this.results = results;
			this.deadline = deadline;
			this.importWindowStart = importWindowStart;
			this.importWindowEnd = importWindowEnd;
			this.session = folder.Session;
			this.addressCache = new InboundAddressCache(options, new ConversionLimitsTracker(options.Limits), MimeMessageLevel.TopLevelMessage);
		}

		private static int QueryIndexOfProperty(PropertyDefinition property)
		{
			int num;
			if (!CalendarImporter.propertyToQueryIndex.TryGetValue(property, out num))
			{
				lock (CalendarImporter.propertyToQueryIndexLock)
				{
					if (!CalendarImporter.propertyToQueryIndex.TryGetValue(property, out num))
					{
						num = Array.IndexOf<PropertyDefinition>(InternetCalendarSchema.ImportQuery, property);
						CalendarImporter.propertyToQueryIndex[property] = num;
					}
				}
			}
			return num;
		}

		private static ExTimeZone GetRecurringTimeZoneFromQueryItem(object[] matchedItem)
		{
			string timeZoneDisplayName = matchedItem[CalendarImporter.QueryIndexOfProperty(CalendarItemBaseSchema.TimeZone)] as string;
			byte[] o11TimeZoneBlob = matchedItem[CalendarImporter.QueryIndexOfProperty(CalendarItemBaseSchema.TimeZoneBlob)] as byte[];
			byte[] o12TimeZoneBlob = matchedItem[CalendarImporter.QueryIndexOfProperty(CalendarItemBaseSchema.TimeZoneDefinitionRecurring)] as byte[];
			return TimeZoneHelper.GetTimeZoneFromProperties(timeZoneDisplayName, o11TimeZoneBlob, o12TimeZoneBlob);
		}

		private int QueryIndexId
		{
			get
			{
				return CalendarImporter.QueryIndexOfProperty(ItemSchema.Id);
			}
		}

		private int QueryIndexRecurring
		{
			get
			{
				return CalendarImporter.QueryIndexOfProperty(CalendarItemBaseSchema.AppointmentRecurring);
			}
		}

		private int QueryIndexGoid
		{
			get
			{
				return CalendarImporter.QueryIndexOfProperty(CalendarItemBaseSchema.GlobalObjectId);
			}
		}

		public void Run()
		{
			ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "CalendarImporter::Run. Begin.");
			this.results.Reset();
			try
			{
				this.RetrieveSnapshots();
				IEnumerable<Item> icalEnumerator = this.GetIcalEnumerator();
				using (IEnumerator<Item> enumerator = icalEnumerator.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Item promotedItem = enumerator.Current;
						if (this.deadline.IsOver)
						{
							this.results.SetTimeOut();
							ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "CalendarImporter::Run. Ran out of time to import items. Skipping the rest.");
							break;
						}
						try
						{
							GrayException.MapAndReportGrayExceptions(delegate()
							{
								ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2319854909U, promotedItem.TryGetProperty(ItemSchema.Subject) as string);
								this.ImportItem(promotedItem);
							});
						}
						catch (GrayException ex)
						{
							ExTraceGlobals.SharingTracer.TraceDebug<GrayException>((long)this.GetHashCode(), "CalendarImporter::Run.Found GrayException: {0}.", ex);
							this.results.RawErrors.Add(ServerStrings.ImportItemThrewGrayException(ex.ToString()));
						}
					}
				}
				if (!this.deadline.IsOver)
				{
					this.DeleteOutOfDateItems();
				}
			}
			finally
			{
				Util.DisposeIfPresent(this.itemCache);
			}
			ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "CalendarImporter::Run. End.");
		}

		private void RetrieveSnapshots()
		{
			this.itemSnapshots = new List<object[]>(this.folder.InternalGetCalendarView(this.importWindowStart, this.importWindowEnd, false, false, true, RecurrenceExpansionOption.IncludeMaster, InternetCalendarSchema.ImportQuery));
			this.itemsToDelete = new List<StoreId>(this.itemSnapshots.Count);
			foreach (object[] array in this.itemSnapshots)
			{
				VersionedId versionedId = (VersionedId)array[this.QueryIndexId];
				if (!(versionedId.ObjectId is OccurrenceStoreObjectId))
				{
					this.itemsToDelete.Add(versionedId.ObjectId);
				}
			}
		}

		private void DeleteOutOfDateItems()
		{
			ExTraceGlobals.SharingTracer.TraceDebug<int>((long)this.GetHashCode(), "CalendarImporter::DeleteOutOfDateItems. There are {0} items need to delete.", this.itemsToDelete.Count);
			this.session.Delete(DeleteItemFlags.SoftDelete, this.itemsToDelete.ToArray<StoreId>());
		}

		private IEnumerable<Item> GetIcalEnumerator()
		{
			string calendarName;
			IEnumerable<Item> result = CalendarDocument.InternalICalToItems(this.inputStream, this.charset, this.addressCache, CalendarImporter.maxICalBodyLength, true, delegate
			{
				Util.DisposeIfPresent(this.itemCache);
				this.itemCache = MessageItem.CreateInMemory(InternalSchema.ContentConversionProperties);
				this.itemCache.PropertyBag.ExTimeZone = this.session.ExTimeZone;
				this.itemCache.Sensitivity = Sensitivity.Normal;
				return this.itemCache;
			}, this.results.RawErrors, out calendarName);
			this.results.CalendarName = calendarName;
			return result;
		}

		private void ImportItem(Item promotedItem)
		{
			GlobalObjectId globalObjectId = new GlobalObjectId(promotedItem);
			object[] array = this.FindItemSnapshotByGoid(globalObjectId, true);
			if (array != null)
			{
				StoreObjectId objectId = ((VersionedId)array[this.QueryIndexId]).ObjectId;
				this.RemoveItemFromDeletionList(objectId);
			}
			CalendarItemBase calendarItemBase = this.ConvertToCalendarItem(globalObjectId, promotedItem, array);
			if (calendarItemBase != null)
			{
				using (calendarItemBase)
				{
					if (!this.IsInImportWindow(calendarItemBase))
					{
						ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "CalendarImporter::ImportItem. Item is outside of the import window. Ignoring. GOID:{0} WindowStart:{1} WindowEnd:{2}.calItem.StartTime:{2} calItem.EndTime:{3}", new object[]
						{
							globalObjectId,
							this.importWindowStart,
							this.importWindowEnd,
							calendarItemBase.StartTime,
							calendarItemBase.EndTime
						});
					}
					else
					{
						this.PostProcessing(calendarItemBase);
						if (calendarItemBase.Validate().Length != 0)
						{
							ExTraceGlobals.SharingTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "CalendarImporter::ImportItems. Validation error for item ID:{0}", promotedItem.Id);
							this.results.RawErrors.Add(ServerStrings.ValidationFailureAfterPromotion(calendarItemBase.GlobalObjectId.Uid));
						}
						else
						{
							try
							{
								ConflictResolutionResult conflictResolutionResult = calendarItemBase.Save(SaveMode.ResolveConflicts);
								if (conflictResolutionResult != ConflictResolutionResult.Success)
								{
									ExTraceGlobals.SharingTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "CalendarImporter::ImportItems. Failed to save item for ID:{0}", calendarItemBase.Id);
									this.results.RawErrors.Add(ServerStrings.SaveFailureAfterPromotion(calendarItemBase.GlobalObjectId.Uid));
								}
								else
								{
									ExTraceGlobals.SharingTracer.TraceDebug<GlobalObjectId>((long)this.GetHashCode(), "CalendarImporter::ImportItem. Saved item. GOID:{0}.", globalObjectId);
									this.results.Increment(true);
									VersionedId calendarItemId = this.folder.GetCalendarItemId(globalObjectId.Bytes);
									if (calendarItemId != null)
									{
										this.UpdateItemSnapshots(globalObjectId, calendarItemId, promotedItem);
									}
								}
							}
							catch (OccurrenceCrossingBoundaryException arg)
							{
								ExTraceGlobals.SharingTracer.TraceDebug<VersionedId, OccurrenceCrossingBoundaryException>((long)this.GetHashCode(), "CalendarImporter::ImportItems. Failed to save item for ID:{0}, Exception = {1}", calendarItemBase.Id, arg);
								this.results.RawErrors.Add(ServerStrings.SaveFailureAfterPromotion(calendarItemBase.GlobalObjectId.Uid));
							}
							catch (CorruptDataException arg2)
							{
								ExTraceGlobals.SharingTracer.TraceDebug<VersionedId, CorruptDataException>((long)this.GetHashCode(), "CalendarImporter::ImportItems. Got a corrupt data exception during Save for item for ID:{0}, Exception = {1}", calendarItemBase.Id, arg2);
								this.results.RawErrors.Add(ServerStrings.SaveFailureAfterPromotion(calendarItemBase.GlobalObjectId.Uid));
							}
							catch (VirusException arg3)
							{
								ExTraceGlobals.SharingTracer.TraceDebug<VersionedId, VirusException>((long)this.GetHashCode(), "CalendarImporter::ImportItems. Got a virus detected exception during Save for item for ID:{0}, Exception = {1}", calendarItemBase.Id, arg3);
								this.results.RawErrors.Add(ServerStrings.SaveFailureAfterPromotion(calendarItemBase.GlobalObjectId.Uid));
							}
							catch (RecurrenceException arg4)
							{
								ExTraceGlobals.SharingTracer.TraceDebug<VersionedId, RecurrenceException>((long)this.GetHashCode(), "CalendarImporter::ImportItems. Got a recurrence exception during Save for item for ID:{0}, Exception = {1}", calendarItemBase.Id, arg4);
								this.results.RawErrors.Add(ServerStrings.SaveFailureAfterPromotion(calendarItemBase.GlobalObjectId.Uid));
							}
						}
					}
					return;
				}
			}
			ExTraceGlobals.SharingTracer.TraceDebug<GlobalObjectId>((long)this.GetHashCode(), "CalendarImporter::ImportItem. No changes detected on input item. GOID:{0}.", globalObjectId);
			this.results.Increment(false);
		}

		private bool IsInImportWindow(CalendarItemBase calItemBase)
		{
			CalendarItem calendarItem = calItemBase as CalendarItem;
			if (calendarItem != null && calendarItem.Recurrence != null)
			{
				return this.IsInImportWindow(calendarItem.Recurrence.Range.StartDate, calendarItem.Recurrence.EndDate);
			}
			return this.IsInImportWindow(calItemBase.StartTime, calItemBase.EndTime);
		}

		private bool IsInImportWindow(ExDateTime startTime, ExDateTime endTime)
		{
			return !(endTime < this.importWindowStart) && !(startTime > this.importWindowEnd);
		}

		private object[] FindItemSnapshotByGoid(GlobalObjectId goid, bool shouldCheckCleanGoid)
		{
			ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "CalendarImporter::FindItemSnapshotByGoid. Looking for matched item of GOID:{0}.", GlobalObjectId.ByteArrayToHexString(goid.Bytes));
			object[] array = this.itemSnapshots.FirstOrDefault((object[] item) => Util.ValueEquals(goid.Bytes, item[this.QueryIndexGoid] as byte[]));
			if (shouldCheckCleanGoid && array == null && !goid.IsCleanGlobalObjectId)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "CalendarImporter::FindItemSnapshotByGoid. No match found according to goid then check the clean GOID:{0}.", GlobalObjectId.ByteArrayToHexString(goid.CleanGlobalObjectIdBytes));
				array = this.itemSnapshots.FirstOrDefault((object[] item) => Util.ValueEquals(goid.CleanGlobalObjectIdBytes, item[this.QueryIndexGoid] as byte[]) && true.Equals(item[this.QueryIndexRecurring]));
			}
			return array;
		}

		private void RemoveItemFromDeletionList(StoreId itemId)
		{
			OccurrenceStoreObjectId occurrenceStoreObjectId = itemId as OccurrenceStoreObjectId;
			if (occurrenceStoreObjectId != null)
			{
				itemId = occurrenceStoreObjectId.GetMasterStoreObjectId();
			}
			if (this.itemsToDelete.Contains(itemId))
			{
				this.itemsToDelete.Remove(itemId);
			}
		}

		private bool IsMeetingChanged(GlobalObjectId goid, Item promotedItem, object[] matchedItem)
		{
			for (int i = 0; i < InternetCalendarSchema.ImportCompare.Length; i++)
			{
				PropertyDefinition propertyDefinition = InternetCalendarSchema.ImportCompare[i];
				object obj = matchedItem[i];
				object obj2 = promotedItem.TryGetProperty(propertyDefinition);
				if (!Util.ValueEquals(obj, obj2))
				{
					if (!(obj is PropertyError) && !(obj2 is PropertyError))
					{
						if (propertyDefinition == CalendarItemBaseSchema.AppointmentRecurrenceBlob)
						{
							InternalRecurrence internalRecurrence = InternalRecurrence.InternalParse((byte[])obj, (VersionedId)matchedItem[this.QueryIndexId], CalendarImporter.GetRecurringTimeZoneFromQueryItem(matchedItem), promotedItem.PropertyBag.ExTimeZone, CalendarItem.DefaultCodePage);
							InternalRecurrence recurrenceFromItem = CalendarItem.GetRecurrenceFromItem(promotedItem);
							ExTraceGlobals.SharingTracer.TraceDebug<InternalRecurrence, InternalRecurrence, GlobalObjectId>((long)this.GetHashCode(), "CalendarImporter::IsMeetingChanged. Comparing property AppointmentRecurrenceBlob: old Recurrence is {0}; new Recurrence is {1}. GOID:{2}.", internalRecurrence, recurrenceFromItem, goid);
							if (Util.ValueEquals(internalRecurrence, recurrenceFromItem) && Util.ValueEquals(internalRecurrence.GetDeletedOccurrences(false), recurrenceFromItem.GetDeletedOccurrences(false)))
							{
								goto IL_214;
							}
						}
						if (true.Equals(promotedItem.TryGetProperty(CalendarItemBaseSchema.IsException)) && (propertyDefinition == ItemSchema.TimeZoneDefinitionStart || propertyDefinition == CalendarItemBaseSchema.TimeZoneDefinitionEnd))
						{
							ExTimeZone exTimeZone;
							O12TimeZoneFormatter.TryParseTimeZoneBlob((byte[])obj, string.Empty, out exTimeZone);
							ExTimeZone exTimeZone2;
							O12TimeZoneFormatter.TryParseTimeZoneBlob((byte[])obj2, string.Empty, out exTimeZone2);
							ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "CalendarImporter::IsMeetingChanged. Comparing property {0}: old TimeZone is {1}; new TimeZone is {2}. GOID:{3}.", new object[]
							{
								propertyDefinition.Name,
								(exTimeZone != null) ? exTimeZone.AlternativeId : "null",
								(exTimeZone2 != null) ? exTimeZone2.AlternativeId : "null",
								goid
							});
							if (exTimeZone != null && exTimeZone2 != null)
							{
								REG_TIMEZONE_INFO reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(exTimeZone, (ExDateTime)matchedItem[CalendarImporter.QueryIndexOfProperty(CalendarItemInstanceSchema.StartTime)]);
								REG_TIMEZONE_INFO reg_TIMEZONE_INFO2 = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(exTimeZone2, (ExDateTime)promotedItem.TryGetProperty(CalendarItemInstanceSchema.StartTime));
								if (Util.ValueEquals(reg_TIMEZONE_INFO, reg_TIMEZONE_INFO2))
								{
									goto IL_214;
								}
							}
						}
					}
					ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "CalendarImporter::IsMeetingChanged. Change is detected on property {0}: old value is {1}; new value is {2}. GOID:{3}; Subject:{4}; StartTime:{5}.", new object[]
					{
						propertyDefinition.Name,
						obj,
						obj2,
						goid,
						promotedItem.TryGetProperty(ItemSchema.Subject),
						promotedItem.TryGetProperty(CalendarItemInstanceSchema.StartTime)
					});
					return true;
				}
				IL_214:;
			}
			return false;
		}

		private void CopyToCalendarItem(Item promotedItem, CalendarItemBase calendarItem, bool itemCreated)
		{
			foreach (PropertyDefinition propertyDefinition in InternetCalendarSchema.ImportUpdate)
			{
				object obj = promotedItem.TryGetProperty(propertyDefinition);
				if (obj != null && !PropertyError.IsPropertyNotFound(obj))
				{
					if (propertyDefinition == CalendarItemBaseSchema.AppointmentRecurrenceBlob && !itemCreated)
					{
						CalendarItem calendarItem2 = calendarItem as CalendarItem;
						if (calendarItem2 != null)
						{
							InternalRecurrence recurrenceFromItem = CalendarItem.GetRecurrenceFromItem(promotedItem);
							InternalRecurrence internalRecurrence = new InternalRecurrence(recurrenceFromItem.Pattern, recurrenceFromItem.Range, calendarItem2, recurrenceFromItem.CreatedExTimeZone, recurrenceFromItem.ReadExTimeZone, recurrenceFromItem.StartOffset, recurrenceFromItem.EndOffset);
							RecurrenceBlobMerger.Merge(null, null, calendarItem.GlobalObjectId, recurrenceFromItem, internalRecurrence);
							calendarItem2.Recurrence = internalRecurrence;
							goto IL_9C;
						}
					}
					calendarItem[propertyDefinition] = obj;
				}
				else
				{
					calendarItem.Delete(propertyDefinition);
				}
				IL_9C:;
			}
			using (TextReader textReader = promotedItem.Body.OpenTextReader(BodyFormat.ApplicationRtf))
			{
				using (TextWriter textWriter = calendarItem.Body.OpenTextWriter(BodyFormat.ApplicationRtf))
				{
					Util.StreamHandler.CopyText(textReader, textWriter);
				}
			}
			calendarItem.CoreItem.Recipients.CopyRecipientsFrom(promotedItem.CoreItem.Recipients);
		}

		private CalendarItemBase ConvertToCalendarItem(GlobalObjectId goid, Item promotedItem, object[] matchedItem)
		{
			CalendarItemBase calendarItemBase = null;
			bool itemCreated = true;
			bool flag = false;
			promotedItem[InternalSchema.BodyTag] = promotedItem.Body.CalculateBodyTag();
			try
			{
				if (matchedItem == null)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<GlobalObjectId, object, object>((long)this.GetHashCode(), "CalendarImporter::ConvertToCalendarItem. No matched existing object found, then create a new one. GOID:{0}; Subject:{1}; StartTime:{2}.", goid, promotedItem.TryGetProperty(ItemSchema.Subject), promotedItem.TryGetProperty(CalendarItemInstanceSchema.StartTime));
					calendarItemBase = CalendarItem.Create(this.session, this.folder.Id);
				}
				else if (this.IsMeetingChanged(goid, promotedItem, matchedItem))
				{
					StoreObjectId storeObjectId = ((VersionedId)matchedItem[this.QueryIndexId]).ObjectId;
					if (!goid.IsCleanGlobalObjectId && true.Equals(matchedItem[this.QueryIndexRecurring]))
					{
						storeObjectId = new OccurrenceStoreObjectId(storeObjectId.ProviderLevelItemId, goid.Date);
					}
					try
					{
						calendarItemBase = CalendarItemBase.Bind(this.session, storeObjectId);
						calendarItemBase.OpenAsReadWrite();
						itemCreated = false;
					}
					catch (OccurrenceNotFoundException)
					{
						ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "CalendarImporter::ConvertToCalendarItem. Tried to bind to an occurrence not existing", storeObjectId.ToString());
						calendarItemBase = CalendarItem.Create(this.session, this.folder.Id);
					}
				}
				if (calendarItemBase != null)
				{
					try
					{
						this.CopyToCalendarItem(promotedItem, calendarItemBase, itemCreated);
						flag = true;
						goto IL_154;
					}
					catch (RecurrenceException arg)
					{
						ExTraceGlobals.SharingTracer.TraceDebug<GlobalObjectId, RecurrenceException>((long)this.GetHashCode(), "CalendarImporter::ConvertToCalendarItem. Failed to copy the iCal to a calendarItem for ID:{0}, Exception = {1}", goid, arg);
						flag = false;
						goto IL_154;
					}
					catch (AttachmentExceededException arg2)
					{
						ExTraceGlobals.SharingTracer.TraceDebug<GlobalObjectId, AttachmentExceededException>((long)this.GetHashCode(), "CalendarImporter::ConvertToCalendarItem. Failed to copy the iCal to a calendarItem for ID:{0}, Exception = {1}", goid, arg2);
						flag = false;
						goto IL_154;
					}
				}
				flag = true;
				IL_154:;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<GlobalObjectId>((long)this.GetHashCode(), "CalendarImporter::ConvertToCalendarItem. Failed to convert to XSO calendar item. GOID:{0}.", goid);
					this.results.RawErrors.Add(ServerStrings.SyncFailedToCreateNewItemOrBindToExistingOne);
					Util.DisposeIfPresent(calendarItemBase);
					calendarItemBase = null;
				}
			}
			return calendarItemBase;
		}

		private void PostProcessing(CalendarItemBase calendarItem)
		{
			calendarItem.Reminder.Disable();
		}

		private void UpdateItemSnapshots(GlobalObjectId goid, VersionedId newItemId, Item promotedItem)
		{
			object[] array = this.FindItemSnapshotByGoid(goid, false);
			bool flag = array == null;
			if (flag)
			{
				array = new object[InternetCalendarSchema.ImportQuery.Length];
				this.itemSnapshots.Add(array);
			}
			for (int i = 0; i < InternetCalendarSchema.ImportQuery.Length; i++)
			{
				array[i] = promotedItem.TryGetProperty(InternetCalendarSchema.ImportQuery[i]);
			}
			array[this.QueryIndexId] = newItemId;
		}

		private static readonly uint? maxICalBodyLength = new uint?(5000U);

		private readonly Deadline deadline;

		private readonly ExDateTime importWindowStart;

		private readonly ExDateTime importWindowEnd;

		private static Dictionary<PropertyDefinition, int> propertyToQueryIndex = new Dictionary<PropertyDefinition, int>();

		private static object propertyToQueryIndexLock = new object();

		private ICollection<object[]> itemSnapshots;

		private ICollection<StoreId> itemsToDelete;

		private Item itemCache;

		private Stream inputStream;

		private string charset;

		private InboundAddressCache addressCache;

		private StoreSession session;

		private CalendarFolder folder;

		private ImportCalendarResults results;
	}
}
