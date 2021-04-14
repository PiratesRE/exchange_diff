using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InternalRecurrence : Recurrence
	{
		public InternalRecurrence(RecurrencePattern pattern, RecurrenceRange range, CalendarItem item, ExTimeZone createTimeZone, ExTimeZone readTimeZone, TimeSpan startOffset, TimeSpan endOffset) : this(pattern, range, createTimeZone, readTimeZone, startOffset, endOffset, null, null, null)
		{
			this.masterItem = item;
			this.masterItemId = null;
			ExTraceGlobals.RecurrenceTracer.Information<int>((long)this.GetHashCode(), "InternalRecurrence::InternalRecurrence, recurrence created for masterItem hashCode: {0}", (this.masterItem == null) ? -1 : this.masterItem.GetHashCode());
		}

		internal InternalRecurrence(RecurrencePattern pattern, RecurrenceRange range, Item item, ExTimeZone createTimeZone, ExTimeZone readTimeZone, TimeSpan startOffset, TimeSpan endOffset, ExDateTime? endDateOverride) : this(pattern, range, createTimeZone, readTimeZone, startOffset, endOffset, null, null, endDateOverride)
		{
			this.masterItem = item;
			this.masterItemId = null;
			ExTraceGlobals.RecurrenceTracer.Information<int>((long)this.GetHashCode(), "InternalRecurrence::InternalRecurrence, recurrence created for masterItem hashCode: {0}", this.masterItem.GetHashCode());
		}

		private InternalRecurrence(RecurrencePattern pattern, RecurrenceRange range, ExTimeZone createTimeZone, ExTimeZone readTimeZone, TimeSpan startOffset, TimeSpan endOffset, SortedDictionary<ExDateTime, object> deletions, SortedDictionary<ExDateTime, ExceptionInfo> exceptions, ExDateTime? endDateOverride) : base(pattern, range, startOffset, endOffset, createTimeZone, readTimeZone, endDateOverride)
		{
			this.deletions = (deletions ?? new SortedDictionary<ExDateTime, object>());
			this.exceptions = (exceptions ?? new SortedDictionary<ExDateTime, ExceptionInfo>());
			List<ExDateTime> list = new List<ExDateTime>();
			foreach (ExDateTime exDateTime in this.deletions.Keys)
			{
				if (base.GetNextOccurrenceDateId(exDateTime.IncrementDays(-1)) != exDateTime)
				{
					list.Add(exDateTime);
				}
				ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::InternalRecurrence, deleted occurrence dateId {0}", exDateTime);
			}
			for (int i = 0; i < list.Count; i++)
			{
				ExTraceGlobals.RecurrenceTracer.TraceDebug<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::InternalRecurrence, Removing ({0}) out of range/pattern from deletions list", list[i]);
				this.deletions.Remove(list[i]);
			}
			this.firstInstanceSince1601 = this.GetFirstOccurrenceAfter1601();
		}

		public AnomaliesFlags Anomalies
		{
			get
			{
				return this.anomalies;
			}
		}

		internal Item MasterItem
		{
			get
			{
				return this.masterItem;
			}
		}

		internal int RemainingOccurrences
		{
			get
			{
				if (base.EndDate <= Recurrence.MaximumDateForRecurrenceEnd)
				{
					return base.NumberOfOccurrences - this.deletions.Count;
				}
				return int.MaxValue;
			}
		}

		private VersionedId MasterItemId
		{
			get
			{
				if (this.masterItem != null)
				{
					return this.masterItem.Id;
				}
				return this.masterItemId;
			}
		}

		public static InternalRecurrence FromMasterPropertyBag(IStorePropertyBag masterPropertyBag, StoreSession session, VersionedId masterItemId, RecurringItemLatencyInformation latencyInformation)
		{
			return InternalRecurrence.FromMasterPropertyBag(masterPropertyBag, session, masterItemId, masterPropertyBag.GetValueOrDefault<int>(ItemSchema.Codepage, CalendarItem.DefaultCodePage), latencyInformation);
		}

		public static InternalRecurrence FromMasterPropertyBag(IStorePropertyBag masterPropertyBag, StoreSession session, VersionedId masterItemId)
		{
			RecurringItemLatencyInformation latencyInformation = new RecurringItemLatencyInformation();
			return InternalRecurrence.FromMasterPropertyBag(masterPropertyBag, session, masterItemId, masterPropertyBag.GetValueOrDefault<int>(ItemSchema.Codepage, CalendarItem.DefaultCodePage), latencyInformation);
		}

		public static bool HasRecurrenceBlob(ICorePropertyBag propertyBag)
		{
			object obj = propertyBag.TryGetProperty(InternalSchema.AppointmentRecurrenceBlob);
			return obj is byte[] || PropertyError.IsPropertyValueTooBig(obj);
		}

		public bool HasModifiedOccurrences()
		{
			return this.exceptions.Count > 0;
		}

		public override IList<OccurrenceInfo> GetModifiedOccurrences()
		{
			OccurrenceInfo[] array = new OccurrenceInfo[this.exceptions.Count];
			int num = 0;
			foreach (ExDateTime occurrenceId in this.exceptions.Keys)
			{
				array[num++] = this.GetOccurrenceInfoByDateId(occurrenceId);
			}
			return array;
		}

		public override ExDateTime[] GetDeletedOccurrences(bool convertToReadTimeZone)
		{
			ExDateTime[] array = new ExDateTime[this.deletions.Count];
			this.deletions.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				ExDateTime exDateTime = new ExDateTime(base.CreatedExTimeZone, array[i].LocalTime + base.StartOffset);
				array[i] = (convertToReadTimeZone ? this.ReadExTimeZone.ConvertDateTime(exDateTime) : exDateTime);
			}
			return array;
		}

		public override ExDateTime[] GetDeletedOccurrences()
		{
			return this.GetDeletedOccurrences(true);
		}

		public IList<OccurrenceInfo> GetExceptionInfoFromAttachComparison(Item master)
		{
			IList<OccurrenceInfo> modifiedOccurrences = this.GetModifiedOccurrences();
			AttachmentCollection attachmentCollection = master.AttachmentCollection;
			PropertyDefinition[] array = new PropertyDefinition[]
			{
				InternalSchema.MapiStartTime,
				InternalSchema.MapiEndTime,
				InternalSchema.SubjectPrefixInternal,
				InternalSchema.NormalizedSubjectInternal,
				InternalSchema.MapiSubject,
				InternalSchema.Location,
				InternalSchema.AppointmentColor,
				InternalSchema.MapiIsAllDayEvent,
				InternalSchema.MapiHasAttachment,
				InternalSchema.FreeBusyStatus,
				InternalSchema.ReminderIsSet,
				InternalSchema.ReminderMinutesBeforeStartInternal,
				InternalSchema.AppointmentState
			};
			foreach (OccurrenceInfo occurrenceInfo in modifiedOccurrences)
			{
				if (occurrenceInfo is ExceptionInfo)
				{
					ExceptionInfo exceptionInfo = occurrenceInfo as ExceptionInfo;
					ItemAttachment itemAttachment = null;
					object[] array2;
					using (Item item = RecurrenceManager.OpenEmbeddedMessageAndAttachment(attachmentCollection, TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(attachmentCollection.ContainerItem.PropertyBag), exceptionInfo.StartTime, exceptionInfo.EndTime, out itemAttachment, array))
					{
						if (item != null)
						{
							array2 = item.GetProperties(array);
							itemAttachment.Dispose();
						}
						else
						{
							array2 = new object[array.Length];
						}
					}
					for (int i = 0; i < array.Length; i++)
					{
						DifferencesBetweenBlobAndAttach differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.None;
						if (array[i].Equals(InternalSchema.MapiStartTime))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.StartTime;
						}
						else if (array[i].Equals(InternalSchema.MapiEndTime))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.EndTime;
						}
						else if (array[i].Equals(InternalSchema.SubjectPrefixInternal))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.Subject;
						}
						else if (array[i].Equals(InternalSchema.NormalizedSubjectInternal))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.Subject;
						}
						else if (array[i].Equals(InternalSchema.MapiSubject))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.Subject;
						}
						else if (array[i].Equals(InternalSchema.Location))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.Location;
						}
						else if (array[i].Equals(InternalSchema.AppointmentColor))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.AppointmentColor;
						}
						else if (array[i].Equals(InternalSchema.MapiIsAllDayEvent))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.IsAllDayEvent;
						}
						else if (array[i].Equals(InternalSchema.MapiHasAttachment))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.HasAttachment;
						}
						else if (array[i].Equals(InternalSchema.FreeBusyStatus))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.FreeBusyStatus;
						}
						else if (array[i].Equals(InternalSchema.ReminderIsSet))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.ReminderIsSet;
						}
						else if (array[i].Equals(InternalSchema.ReminderMinutesBeforeStartInternal))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.ReminderMinutesBeforeStartInternal;
						}
						else if (array[i].Equals(InternalSchema.AppointmentState))
						{
							differencesBetweenBlobAndAttach = DifferencesBetweenBlobAndAttach.AppointmentState;
						}
						if (differencesBetweenBlobAndAttach != DifferencesBetweenBlobAndAttach.None)
						{
							object obj = exceptionInfo.PropertyBag.TryGetProperty(array[i]);
							if (!(obj is PropertyError) && array2[i] != null && !(array2[i] is PropertyError) && !obj.Equals(array2[i]))
							{
								exceptionInfo.BlobDifferences |= differencesBetweenBlobAndAttach;
							}
						}
					}
				}
			}
			return modifiedOccurrences;
		}

		public override bool IsOccurrenceDeleted(ExDateTime occurrenceId)
		{
			return this.deletions.ContainsKey(occurrenceId);
		}

		internal static InternalRecurrence FromMasterPropertyBag(PropertyBag masterPropertyBag, StoreSession session, VersionedId masterItemId, int usingCodePageId, RecurringItemLatencyInformation latencyInformation)
		{
			return InternalRecurrence.FromMasterPropertyBag(masterPropertyBag.AsIStorePropertyBag(), session, masterItemId, usingCodePageId, latencyInformation);
		}

		internal static InternalRecurrence FromMasterPropertyBag(IStorePropertyBag masterPropertyBag, StoreSession session, VersionedId masterItemId, int usingCodePageId, RecurringItemLatencyInformation latencyInformation)
		{
			if (masterItemId != null && masterItemId.ObjectId != null && masterItemId.ObjectId.ObjectType != StoreObjectType.CalendarItem)
			{
				return null;
			}
			if (!(masterPropertyBag.TryGetProperty(InternalSchema.AppointmentRecurrenceBlob) is byte[]))
			{
				return null;
			}
			ExTimeZone recurringTimeZoneFromPropertyBag = TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(masterPropertyBag);
			if (recurringTimeZoneFromPropertyBag == null)
			{
				return null;
			}
			return InternalRecurrence.GetRecurrence(masterItemId, session, masterPropertyBag.TryGetProperty(InternalSchema.AppointmentRecurrenceBlob) as byte[], recurringTimeZoneFromPropertyBag, usingCodePageId, latencyInformation);
		}

		internal static InternalRecurrence GetRecurrence(VersionedId versionedId, StoreSession session, byte[] recurrenceBlob, ExTimeZone createTimeZone, int usingCodePageId)
		{
			RecurringItemLatencyInformation latencyInformation = new RecurringItemLatencyInformation();
			return InternalRecurrence.GetRecurrence(versionedId, session, recurrenceBlob, createTimeZone, usingCodePageId, latencyInformation);
		}

		internal static InternalRecurrence GetRecurrence(VersionedId versionedId, StoreSession session, byte[] recurrenceBlob, ExTimeZone createTimeZone, int usingCodePageId, RecurringItemLatencyInformation latencyInformation)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (versionedId != null && versionedId.ObjectId != null && recurrenceBlob != null && (recurrenceBlob.Length == 510 || recurrenceBlob.Length == 25600))
			{
				ExTraceGlobals.RecurrenceTracer.Information<VersionedId>(0L, "InternalRecurrence::GetRecurrence, Recurrence for calendarItem with Id {0} too big to fit in view. Opening calendarItem", versionedId);
				using (MapiProp mapiProp = session.GetMapiProp(versionedId.ObjectId))
				{
					using (StoreObjectPropertyBag storeObjectPropertyBag = new StoreObjectPropertyBag(session, mapiProp, CalendarItemBaseSchema.Instance.AutoloadProperties))
					{
						long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
						recurrenceBlob = storeObjectPropertyBag.GetLargeBinaryProperty(InternalSchema.AppointmentRecurrenceBlob);
						long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
						latencyInformation.BlobStreamTime = elapsedMilliseconds2 - elapsedMilliseconds;
					}
				}
			}
			if (recurrenceBlob == null)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError(0L, "InternalRecurrence::GetRecurrence, corrupted recurrence blob");
				throw new CorruptDataException(ServerStrings.ExCorruptedRecurringCalItem);
			}
			ExTimeZone exTimeZone = session.ExTimeZone;
			if (createTimeZone == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "InternalRecurrence::GetRecurrence, Error getting timezone blob. Using user time zone intead.");
				createTimeZone = exTimeZone;
			}
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			InternalRecurrence result;
			try
			{
				result = InternalRecurrence.InternalParse(recurrenceBlob, versionedId, createTimeZone, exTimeZone, usingCodePageId);
			}
			finally
			{
				stopwatch.Stop();
				latencyInformation.BlobParseTime = stopwatch.ElapsedMilliseconds - elapsedMilliseconds3;
			}
			return result;
		}

		internal static InternalRecurrence InternalParse(byte[] blob, VersionedId masterItemId, ExTimeZone createTimeZone, ExTimeZone readTimeZone, int usingCodePageId)
		{
			if (createTimeZone == null)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "failed Recurrence parsing because of null timeZoneBlob");
				throw new CorruptDataException(ServerStrings.ExCorruptedTimeZone);
			}
			return InternalRecurrence.InternalParse(blob, null, masterItemId, createTimeZone, readTimeZone, false, usingCodePageId);
		}

		internal static InternalRecurrence InternalParse(byte[] blob, Item masterItem, ExTimeZone createTimeZone, ExTimeZone readTimeZone, int usingCodePageId)
		{
			if (createTimeZone == null)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)((masterItem != null) ? masterItem.GetHashCode() : 0), "failed Recurrence parsing because of null timeZoneBlob");
				throw new CorruptDataException(ServerStrings.ExCorruptedTimeZone);
			}
			return InternalRecurrence.InternalParse(blob, masterItem, null, createTimeZone, readTimeZone, false, usingCodePageId);
		}

		internal static TaskRecurrence InternalParseTask(byte[] blob, Item masterItem, ExTimeZone createTimeZone, ExTimeZone readTimeZone)
		{
			InternalRecurrence internalRecurrence = InternalRecurrence.InternalParse(blob, masterItem, null, createTimeZone, readTimeZone, true, CalendarItem.DefaultCodePage);
			return new TaskRecurrence(internalRecurrence.Pattern, internalRecurrence.Range, internalRecurrence.StartOffset, internalRecurrence.EndOffset, createTimeZone, readTimeZone, new ExDateTime?(internalRecurrence.EndDate));
		}

		internal InternalRecurrence.IOccurrenceIterator CreateIterator(ExDateTime approximateStartDate, bool exceptionIterator)
		{
			if (exceptionIterator)
			{
				IList<OccurrenceInfo> modifiedOccurrences = this.GetModifiedOccurrences();
				int num = 0;
				while (num < modifiedOccurrences.Count && !(modifiedOccurrences[num].StartTime >= approximateStartDate))
				{
					num++;
				}
				if (num == modifiedOccurrences.Count)
				{
					num = modifiedOccurrences.Count - 1;
				}
				return new InternalRecurrence.ExceptionOccurrenceIterator(modifiedOccurrences, num);
			}
			approximateStartDate = this.ReadExTimeZone.ConvertDateTime(approximateStartDate);
			approximateStartDate = base.CreatedExTimeZone.ConvertDateTime(approximateStartDate);
			ExDateTime exDateTime = this.GetNextOccurrenceDateId(approximateStartDate);
			if (exDateTime == ExDateTime.MinValue)
			{
				exDateTime = this.GetNextOccurrenceDateId(exDateTime);
			}
			else if (exDateTime == ExDateTime.MaxValue)
			{
				exDateTime = this.GetPreviousOccurrenceDateId(exDateTime);
			}
			return new InternalRecurrence.OccurrenceIterator(this, exDateTime);
		}

		internal override OccurrenceInfo GetOccurrenceInfoByDateId(ExDateTime occurrenceDate)
		{
			if (!base.IsValidOccurrenceId(occurrenceDate) || this.deletions.ContainsKey(occurrenceDate))
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "OccurrenceInfo requested for either invalid occurrenceId or deleted occurrence.");
				OccurrenceStoreObjectId occId = null;
				if (this.MasterItemId != null)
				{
					occId = new OccurrenceStoreObjectId(this.MasterItemId.ObjectId.ProviderLevelItemId, occurrenceDate);
				}
				throw new OccurrenceNotFoundException(ServerStrings.ExOccurrenceNotPresent(occId));
			}
			return this.MakeOccurrenceInfo(occurrenceDate);
		}

		internal bool IsEmpty()
		{
			return base.NumberOfOccurrences <= this.deletions.Count;
		}

		public bool TryDeleteOccurrence(ExDateTime occurrenceId)
		{
			if (!base.IsValidOccurrenceId(occurrenceId) || this.IsOccurrenceDeleted(occurrenceId))
			{
				ExTraceGlobals.RecurrenceTracer.TraceDebug<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::TryDeleteOccurrence, could not delete OccurrenceId: {0:d}", occurrenceId);
				return false;
			}
			if (this.exceptions.ContainsKey((ExDateTime)occurrenceId.LocalTime))
			{
				this.exceptions.Remove((ExDateTime)occurrenceId.LocalTime);
			}
			this.deletions.Add(occurrenceId, null);
			ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::TryDeleteOccurrence, occurrenceId: {0:d} deleted", occurrenceId);
			return true;
		}

		public void ModifyOccurrence(ExceptionInfo exceptionInfo)
		{
			ExDateTime date = base.CreatedExTimeZone.ConvertDateTime(exceptionInfo.OriginalStartTime).Date;
			if (!base.IsValidOccurrenceId(date))
			{
				throw new OccurrenceNotFoundException(ServerStrings.ExOccurrenceNotPresent(date));
			}
			ExDateTime exDateTime = date;
			ExDateTime date2 = base.CreatedExTimeZone.ConvertDateTime(exceptionInfo.StartTime).Date;
			exDateTime = this.GetPreviousOccurrenceDateId(exDateTime);
			if (exDateTime != ExDateTime.MinValue)
			{
				OccurrenceInfo occurrenceInfoByDateId = this.GetOccurrenceInfoByDateId(exDateTime);
				if (exceptionInfo.StartTime < occurrenceInfoByDateId.EndTime)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ModifyOccurrence, Modified occurrence is crossing occurrence before it. ExceptionInfo.dateId {0}, StartTime {1}, EndTime {2}, PrevOcc.dateId {3}, StartTime {4} and EndTime {5}.", new object[]
					{
						exceptionInfo.OccurrenceDateId,
						exceptionInfo.StartTime,
						exceptionInfo.EndTime,
						occurrenceInfoByDateId.OccurrenceDateId,
						occurrenceInfoByDateId.StartTime,
						occurrenceInfoByDateId.EndTime
					});
					throw new OccurrenceCrossingBoundaryException(exceptionInfo, occurrenceInfoByDateId, ServerStrings.ExModifiedOccurrenceCrossingAdjacentOccurrenceBoundary(exceptionInfo.StartTime, exceptionInfo.EndTime, occurrenceInfoByDateId.StartTime, occurrenceInfoByDateId.EndTime), date2 == base.CreatedExTimeZone.ConvertDateTime(occurrenceInfoByDateId.StartTime).Date);
				}
				if (date2 <= base.CreatedExTimeZone.ConvertDateTime(occurrenceInfoByDateId.StartTime).Date)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ModifyOccurrence, Modified occurrence starts on sameDay as previous occurrence, ExceptionInfo.dateId {0}, startTime(readTimeZone) {1} otherOcc.dateId {2}, startTime(readTimeZone) {3}", new object[]
					{
						exceptionInfo.OccurrenceDateId,
						exceptionInfo.StartTime,
						occurrenceInfoByDateId.OccurrenceDateId,
						occurrenceInfoByDateId.StartTime
					});
					throw new OccurrenceCrossingBoundaryException(exceptionInfo, occurrenceInfoByDateId, ServerStrings.ExModifiedOccurrenceCantHaveStartDateAsAdjacentOccurrence(date2, base.CreatedExTimeZone.ConvertDateTime(occurrenceInfoByDateId.StartTime).Date), true);
				}
			}
			exDateTime = date;
			exDateTime = this.GetNextOccurrenceDateId(exDateTime);
			if (exDateTime != ExDateTime.MaxValue)
			{
				OccurrenceInfo occurrenceInfoByDateId2 = this.GetOccurrenceInfoByDateId(exDateTime);
				if (exceptionInfo.EndTime > occurrenceInfoByDateId2.StartTime)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ModifyOccurrence, Modified occurrence is crossing occurrence after it. ExceptionInfo.dateId {0}, StartTime {1}, EndTime {2}, NextOcc.dateId {3}, StartTime {4} and EndTime {5}.", new object[]
					{
						exceptionInfo.OccurrenceDateId,
						exceptionInfo.StartTime,
						exceptionInfo.EndTime,
						occurrenceInfoByDateId2.OccurrenceDateId,
						occurrenceInfoByDateId2.StartTime,
						occurrenceInfoByDateId2.EndTime
					});
					throw new OccurrenceCrossingBoundaryException(exceptionInfo, occurrenceInfoByDateId2, ServerStrings.ExModifiedOccurrenceCrossingAdjacentOccurrenceBoundary(exceptionInfo.StartTime, exceptionInfo.EndTime, occurrenceInfoByDateId2.StartTime, occurrenceInfoByDateId2.EndTime), date2 == base.CreatedExTimeZone.ConvertDateTime(occurrenceInfoByDateId2.StartTime).Date);
				}
				if (date2 >= base.CreatedExTimeZone.ConvertDateTime(occurrenceInfoByDateId2.StartTime).Date)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ModifyOccurrence, Modified occurrence starts on sameDay as next occurrence, ExceptionInfo.dateId {0}, startTime(readTimeZone) {1} otherOcc.dateId {2}, startTime(readTimeZone) {3}", new object[]
					{
						exceptionInfo.OccurrenceDateId,
						exceptionInfo.StartTime,
						occurrenceInfoByDateId2.OccurrenceDateId,
						occurrenceInfoByDateId2.StartTime
					});
					throw new OccurrenceCrossingBoundaryException(exceptionInfo, occurrenceInfoByDateId2, ServerStrings.ExModifiedOccurrenceCantHaveStartDateAsAdjacentOccurrence(date2, base.CreatedExTimeZone.ConvertDateTime(occurrenceInfoByDateId2.StartTime).Date), true);
				}
			}
			ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::ModifyOccurrence, Exception with dateId {0} added or modified.", date);
			this.exceptions[date] = new ExceptionInfo(exceptionInfo.VersionedId, exceptionInfo);
		}

		internal bool TryUndeleteOccurrence(ExDateTime occurrenceId)
		{
			bool flag = this.deletions.Remove(occurrenceId);
			ExTraceGlobals.RecurrenceTracer.Information<ExDateTime, bool>((long)this.GetHashCode(), "InternalRecurrence::TryUndelete, Occurrence undelete attempted for {0} with result {1}", occurrenceId, flag);
			return flag;
		}

		internal bool IsTimeOffsetOutOfDate()
		{
			bool result = false;
			if (this.MasterItem == null)
			{
				result = true;
			}
			else
			{
				PropertyDefinition propertyDefinition;
				PropertyDefinition propertyDefinition2;
				if (this.MasterItem is Task)
				{
					propertyDefinition = InternalSchema.StartDate;
					propertyDefinition2 = InternalSchema.DueDate;
				}
				else
				{
					propertyDefinition = InternalSchema.StartTime;
					propertyDefinition2 = InternalSchema.EndTime;
				}
				if (this.masterItem.IsPropertyDirty(propertyDefinition) || this.masterItem.IsPropertyDirty(propertyDefinition2))
				{
					ExDateTime? valueAsNullable = this.masterItem.GetValueAsNullable<ExDateTime>(propertyDefinition);
					ExDateTime? valueAsNullable2 = this.masterItem.GetValueAsNullable<ExDateTime>(propertyDefinition2);
					if (valueAsNullable != null && valueAsNullable2 != null)
					{
						ExDateTime exDateTime = base.CreatedExTimeZone.ConvertDateTime(valueAsNullable.Value);
						ExDateTime exDateTime2 = base.CreatedExTimeZone.ConvertDateTime(valueAsNullable2.Value);
						TimeSpan t = exDateTime.LocalTime - exDateTime.LocalTime.Date;
						TimeSpan t2 = exDateTime2.LocalTime - exDateTime.LocalTime.Date;
						try
						{
							result = (Convert.ToInt32(t.TotalMinutes) != Convert.ToInt32(base.StartOffset.TotalMinutes) || Convert.ToInt32(t2.TotalMinutes) != Convert.ToInt32(base.EndOffset.TotalMinutes));
						}
						catch (OverflowException)
						{
							throw new OccurrenceTimeSpanTooBigException(t2 - t, TimeSpan.FromDays(30.0), ServerStrings.ExMeetingCantCrossOtherOccurrences(TimeSpan.FromTicks(2147483647L), TimeSpan.FromDays(14.0)));
						}
					}
				}
			}
			return result;
		}

		public byte[] ToByteArray()
		{
			return this.ToByteArray(false);
		}

		internal byte[] ToByteArray(bool generatePrimaryBlobOnly)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(512))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.GeneratePrimaryRecurrenceBlob(binaryWriter);
					if (!generatePrimaryBlobOnly)
					{
						this.GenerateExceptionInfo(binaryWriter);
						this.GenerateExtendedExceptionInfo(binaryWriter);
					}
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		protected OccurrenceInfo MakeOccurrenceInfo(ExDateTime occurrenceId)
		{
			occurrenceId.CheckExpectedTimeZone(base.CreatedExTimeZone);
			VersionedId versionedId = null;
			if (this.MasterItemId != null)
			{
				versionedId = new VersionedId(new OccurrenceStoreObjectId(this.MasterItemId.ObjectId.ProviderLevelItemId, occurrenceId), this.MasterItemId.ChangeKeyAsByteArray());
			}
			if (this.exceptions.ContainsKey((ExDateTime)occurrenceId.LocalTime))
			{
				return new ExceptionInfo(versionedId, this.exceptions[(ExDateTime)occurrenceId.LocalTime]);
			}
			OccurrenceInfo result;
			try
			{
				ExDateTime exDateTime;
				ExDateTime endTime;
				base.ComputeStartAndEndForInstance(occurrenceId, out exDateTime, out endTime);
				result = new OccurrenceInfo(versionedId, occurrenceId, exDateTime, exDateTime, endTime);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError<ExDateTime, TimeSpan, TimeSpan>((long)this.GetHashCode(), "InternalRecurrence::MakeOccurrenceInfo, ArgumentOutOfRangeException with occurrenceId {0}, StartOffset {1}, EndOffset {2}", occurrenceId, base.StartOffset, base.EndOffset);
				ex.Data.Add("occurenceId", occurrenceId.ToString());
				ex.Data.Add("StartOffset", base.StartOffset.ToString());
				ex.Data.Add("EndOffset", base.EndOffset.ToString());
				throw new CorruptDataException(ServerStrings.RecurrenceBlobCorrupted, ex);
			}
			return result;
		}

		protected override ExDateTime GetNextOccurrenceDateId(ExDateTime date)
		{
			ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::GetNextOccurrenceDateId, GetNextOccurrenceDateId called with dateId {0}", date);
			while (date != ExDateTime.MaxValue)
			{
				date = base.GetNextOccurrenceDateId(date);
				if (!this.deletions.ContainsKey(date))
				{
					ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::GetNextOccurrenceDateId, NextOccurrenceId {0}", date);
					return date;
				}
				ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::GetNextOccurrenceDateId, Skipping dateId {0} because it is deleted", date);
			}
			return date;
		}

		protected override ExDateTime GetPreviousOccurrenceDateId(ExDateTime date)
		{
			ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::GetPreviousOccurrenceDateId, GetPreviousOccurrenceDateId called with dateId {0}", date);
			while (date != ExDateTime.MinValue)
			{
				date = base.GetPreviousOccurrenceDateId(date);
				if (!this.deletions.ContainsKey(date))
				{
					ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::GetPreviousOccurrenceDateId, PreviousOccurrenceId {0}", date);
					return date;
				}
				ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::GetPreviousOccurrenceDateId, Skipping dateId {0} because it is deleted", date);
			}
			return date;
		}

		private static Dictionary<NativeStorePropertyDefinition, ModificationType> BuildPropertyModification(Dictionary<NativeStorePropertyDefinition, ModificationType> propertyModification)
		{
			return new Dictionary<NativeStorePropertyDefinition, ModificationType>
			{
				{
					InternalSchema.NormalizedSubjectInternal,
					ModificationType.Subject
				},
				{
					InternalSchema.SubjectPrefixInternal,
					ModificationType.Subject
				},
				{
					InternalSchema.MapiSubject,
					ModificationType.Subject
				},
				{
					InternalSchema.Location,
					ModificationType.Location
				},
				{
					InternalSchema.AppointmentColor,
					ModificationType.Color
				},
				{
					InternalSchema.MapiIsAllDayEvent,
					ModificationType.SubType
				},
				{
					InternalSchema.MapiHasAttachment,
					ModificationType.Attachment
				},
				{
					InternalSchema.FreeBusyStatus,
					ModificationType.BusyStatus
				},
				{
					InternalSchema.ReminderIsSetInternal,
					ModificationType.Reminder
				},
				{
					InternalSchema.ReminderMinutesBeforeStartInternal,
					ModificationType.ReminderDelta
				},
				{
					InternalSchema.AppointmentStateInternal,
					ModificationType.MeetingType
				}
			};
		}

		private static Dictionary<CalendarType, ExDateTime[]> GenerateTableForMonthStartDatesAfter1601()
		{
			Dictionary<CalendarType, ExDateTime[]> dictionary = new Dictionary<CalendarType, ExDateTime[]>();
			ExDateTime[] value = new ExDateTime[]
			{
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 2, 2),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 3, 4),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 4, 2),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 5, 2),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 5, 31),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 6, 30),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 7, 30),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 8, 28),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 9, 27),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 10, 27),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 11, 25),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 12, 25),
				new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1602, 2, 14)
			};
			dictionary.Add(CalendarType.ChineseLunar, value);
			dictionary.Add(CalendarType.JapanLunar, value);
			dictionary.Add(CalendarType.KoreaLunar, InternalRecurrence.GetYearStartAfter1601(CalendarType.KoreaLunar));
			dictionary.Add(CalendarType.Hebrew, InternalRecurrence.GetYearStartAfter1601(CalendarType.Hebrew));
			dictionary.Add(CalendarType.Hijri, InternalRecurrence.GetYearStartAfter1601(CalendarType.Hijri));
			value = InternalRecurrence.GetYearStartAfter1601(CalendarType.Gregorian);
			dictionary.Add(CalendarType.Default, value);
			dictionary.Add(CalendarType.Korea, value);
			dictionary.Add(CalendarType.Japan, value);
			dictionary.Add(CalendarType.Taiwan, value);
			dictionary.Add(CalendarType.Thai, value);
			dictionary.Add(CalendarType.Gregorian, value);
			dictionary.Add(CalendarType.Gregorian_us, value);
			dictionary.Add(CalendarType.GregorianArabic, value);
			dictionary.Add(CalendarType.GregorianMeFrench, value);
			dictionary.Add(CalendarType.GregorianXlitEnglish, value);
			dictionary.Add(CalendarType.GregorianXlitFrench, value);
			return dictionary;
		}

		private static ExDateTime[] GetYearStartAfter1601(CalendarType calendarType)
		{
			ExDateTime[] array = new ExDateTime[13];
			ExDateTime time = Util.Date1601;
			ExCalendar calendar = Recurrence.GetCalendar(calendarType);
			if (calendar != Recurrence.ExGregorianCalendar)
			{
				time = Util.Date1601;
				if (calendar.GetDayOfMonth(time) != 1 || calendar.GetMonth(time) != 1)
				{
					time = calendar.AddYears(time, 1);
					time = calendar.ToDateTime(calendar.GetYear(time), 1, 1, time.Hour, time.Minute, time.Second, time.Millisecond, calendar.GetEra(time));
				}
			}
			for (int i = 0; i < 13; i++)
			{
				array[i] = calendar.AddMonths(time, i);
			}
			return array;
		}

		private static KeyValuePair<int, bool> GetMonthOfYear(CalendarType calendarType, ExDateTime firstMonthAfter1601, ExDateTime startDate)
		{
			if (Recurrence.IsGregorianCompatible(calendarType))
			{
				return new KeyValuePair<int, bool>(firstMonthAfter1601.Month, false);
			}
			ExCalendar calendar = Recurrence.GetCalendar(calendarType);
			ExDateTime[] array = InternalRecurrence.firstMonthsAfter1601[calendarType];
			if (startDate < calendar.MinSupportedDateTime || startDate > calendar.MaxSupportedDateTime)
			{
				throw new ArgumentOutOfRangeException("startDate for recurrence is out of range of supported date times for given calendarType");
			}
			bool value = false;
			int num = calendar.GetMonth(startDate);
			int num2 = calendar.GetLeapMonth(calendar.GetYear(startDate), calendar.GetEra(startDate));
			num2 = ((num2 == 0) ? int.MaxValue : num2);
			if (num2 == num)
			{
				value = true;
			}
			if (num2 <= num)
			{
				num--;
			}
			return new KeyValuePair<int, bool>(num, value);
		}

		private static ExDateTime GetFirstMonthStartAfter1601(IYearlyPatternInfo yearlyPatternInfo, ExDateTime rangeStartDate)
		{
			int num = yearlyPatternInfo.Month;
			ExCalendar calendar = Recurrence.GetCalendar(yearlyPatternInfo.CalendarType);
			int num2 = calendar.GetLeapMonth(calendar.GetYear(rangeStartDate), calendar.GetEra(rangeStartDate));
			num2 = ((num2 == 0) ? int.MaxValue : num2);
			if (num > num2 || (yearlyPatternInfo.IsLeapMonth && num == num2 - 1))
			{
				num++;
			}
			return InternalRecurrence.firstMonthsAfter1601[yearlyPatternInfo.CalendarType][num - 1];
		}

		private static int DateTimeToFileTime(ExDateTime date)
		{
			return Util.ConvertDateTimeToRTime(date);
		}

		private static ExDateTime FileTimeToDateTime(int fileTime, Stream stream)
		{
			ExDateTime result;
			if (Util.TryConvertRTimeToDateTime(fileTime, out result))
			{
				return result;
			}
			ExTraceGlobals.StorageTracer.TraceError<int>(0L, "InternalRecurrence::FileTimeToDateTime: fileTime value is more then maximum value = {0}", fileTime);
			throw new RecurrenceFormatException(ServerStrings.ExInvalidFileTimeInRecurrenceBlob(fileTime), stream);
		}

		private static InternalRecurrence InternalParse(byte[] blob, Item masterItem, VersionedId masterItemId, ExTimeZone createTimeZone, ExTimeZone readTimeZone, bool expectNoException, int usingCodePageId)
		{
			InternalRecurrence internalRecurrence = null;
			using (MemoryStream memoryStream = new MemoryStream(blob))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					try
					{
						RecurrenceVersion recurrenceVersion;
						internalRecurrence = InternalRecurrence.ParsePrimaryRecurrenceBlob(binaryReader, masterItem, masterItemId, createTimeZone, readTimeZone, expectNoException, out recurrenceVersion);
						if (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
						{
							internalRecurrence.ParseExceptionInfo(binaryReader, recurrenceVersion, usingCodePageId);
						}
						if (expectNoException && internalRecurrence.exceptions.Count != 0)
						{
							throw new RecurrenceFormatException(ServerStrings.ExIncompleteBlob, binaryReader.BaseStream);
						}
					}
					catch (EndOfStreamException innerException)
					{
						throw new RecurrenceFormatException(ServerStrings.ExIncompleteBlob, memoryStream, innerException);
					}
					catch (RecurrenceException ex)
					{
						throw new RecurrenceFormatException(ex.LocalizedString, memoryStream, ex);
					}
				}
			}
			return internalRecurrence;
		}

		private static void CheckFieldLength(int length, Stream baseStream)
		{
			if (length < 0 || length > 32768)
			{
				throw new RecurrenceFormatException(ServerStrings.ExIncompleteBlob, baseStream);
			}
		}

		private static void SkipBlock(BinaryReader blobReader)
		{
			for (int i = blobReader.ReadInt32(); i > 0; i = blobReader.ReadInt32())
			{
				blobReader.BaseStream.Seek((long)i, SeekOrigin.Current);
			}
		}

		private static InternalRecurrence ParsePrimaryRecurrenceBlob(BinaryReader blobReader, Item masterItem, VersionedId masterItemId, ExTimeZone createTimeZone, ExTimeZone readTimeZone, bool expectNoException, out RecurrenceVersion recurrenceVersion)
		{
			blobReader.BaseStream.Seek(0L, SeekOrigin.Begin);
			AnomaliesFlags anomaliesFlags = AnomaliesFlags.None;
			int num = (int)blobReader.ReadInt16();
			int num2 = (int)blobReader.ReadInt16();
			RecurrenceGroup recurrenceGroup = (RecurrenceGroup)blobReader.ReadInt16();
			RecurrenceTypeInBlob recurrenceTypeInBlob = (RecurrenceTypeInBlob)blobReader.ReadInt16();
			CalendarType calendarType = (CalendarType)blobReader.ReadInt16();
			try
			{
				Recurrence.GetCalendar(calendarType);
			}
			catch (NotSupportedException ex)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), ex.Message);
				throw new RecurrenceCalendarTypeNotSupportedException(ServerStrings.ExCalendarTypeNotSupported(calendarType), calendarType, blobReader.BaseStream);
			}
			ExDateTime exDateTime = InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream);
			if (createTimeZone != null)
			{
				exDateTime = createTimeZone.Assign(exDateTime);
			}
			int num3 = blobReader.ReadInt32();
			ExTraceGlobals.RecurrenceTracer.Information((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, PrimaryBlob RecurrenceVersionRead {0}, RecurrenceVersionWrite {1}, Group {2}, Type {3}, calendarType {4}, firstDateAfter1601 {5}, period {6}", new object[]
			{
				num,
				num2,
				recurrenceGroup,
				recurrenceTypeInBlob,
				calendarType,
				exDateTime,
				num3
			});
			if (recurrenceGroup == RecurrenceGroup.Yearly && num3 % 12 != 0)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, period in months for yearly occurrence is not 12, period = {0}", num3);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidYearlyRecurrencePeriod(num3), blobReader.BaseStream);
			}
			RecurrencePatternType recurrencePatternType = (RecurrencePatternType)blobReader.ReadInt32();
			DaysOfWeek daysOfWeek = DaysOfWeek.None;
			int num4 = 0;
			int num5 = 0;
			switch (recurrenceTypeInBlob)
			{
			case RecurrenceTypeInBlob.Minute:
				goto IL_21D;
			case RecurrenceTypeInBlob.Week:
				daysOfWeek = (DaysOfWeek)blobReader.ReadInt32();
				goto IL_21D;
			case RecurrenceTypeInBlob.Month:
			case RecurrenceTypeInBlob.HjMonth:
				num4 = blobReader.ReadInt32();
				goto IL_21D;
			case RecurrenceTypeInBlob.MonthNth:
			case RecurrenceTypeInBlob.HjMonthNth:
				daysOfWeek = (DaysOfWeek)blobReader.ReadInt32();
				num5 = blobReader.ReadInt32();
				if (recurrencePatternType != RecurrencePatternType.Regenerating)
				{
					num5 = ((num5 == 5) ? -1 : num5);
					goto IL_21D;
				}
				if (num5 == 0)
				{
					num5 = -1;
					goto IL_21D;
				}
				goto IL_21D;
			case RecurrenceTypeInBlob.MonthEnd:
			case RecurrenceTypeInBlob.HjMonthEnd:
				blobReader.ReadInt32();
				goto IL_21D;
			}
			ExTraceGlobals.StorageTracer.TraceError<RecurrenceTypeInBlob>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, Unknown RecurrenceType In blob, RecurrenceType={0}", recurrenceTypeInBlob);
			throw new RecurrenceFormatException(ServerStrings.ExUnknownRecurrenceBlobType, blobReader.BaseStream);
			IL_21D:
			RecurrenceRangeType recurrenceRangeType = (RecurrenceRangeType)blobReader.ReadInt32();
			int num6 = blobReader.ReadInt32();
			int num7 = blobReader.ReadInt32();
			SortedDictionary<ExDateTime, object> sortedDictionary = new SortedDictionary<ExDateTime, object>();
			for (int i = blobReader.ReadInt32(); i > 0; i--)
			{
				ExDateTime exDateTime2 = InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream).Date;
				if (createTimeZone != null)
				{
					exDateTime2 = createTimeZone.Assign(exDateTime2);
				}
				if (!sortedDictionary.ContainsKey(exDateTime2))
				{
					sortedDictionary.Add(exDateTime2, null);
				}
			}
			SortedDictionary<ExDateTime, ExceptionInfo> sortedDictionary2 = new SortedDictionary<ExDateTime, ExceptionInfo>();
			for (int j = blobReader.ReadInt32(); j > 0; j--)
			{
				ExDateTime exDateTime3 = InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream).Date;
				if (createTimeZone != null)
				{
					exDateTime3 = createTimeZone.Assign(exDateTime3);
				}
				if (sortedDictionary2.ContainsKey(exDateTime3))
				{
					ExTraceGlobals.RecurrenceTracer.TraceError((long)((masterItem == null) ? -1 : masterItem.GetHashCode()), ServerStrings.ExInvalidExceptionListWithDoubleEntry(exDateTime3));
					anomaliesFlags |= AnomaliesFlags.MultipleExceptionsWithSameDate;
				}
				else
				{
					ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)((masterItem == null) ? -1 : masterItem.GetHashCode()), "Adding exceptionDate to exceptions list {0}", exDateTime3);
					sortedDictionary2.Add(exDateTime3, null);
				}
			}
			ExDateTime exDateTime4 = InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream).Date;
			if (createTimeZone != null)
			{
				exDateTime4 = createTimeZone.Assign(exDateTime4);
			}
			if (exDateTime4 < Recurrence.MinimumDateForRecurrenceStart)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), ServerStrings.ExStartDateCantBeLessThanMinimum(exDateTime4, Recurrence.MinimumDateForRecurrenceStart));
				throw new RecurrenceFormatException(ServerStrings.ExStartDateCantBeLessThanMinimum(exDateTime4, Recurrence.MinimumDateForRecurrenceStart), blobReader.BaseStream);
			}
			if (exDateTime4 > Recurrence.MaximumDateForRecurrenceEnd)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), ServerStrings.ExStartDateCantBeGreaterThanMaximum(exDateTime4, Recurrence.MaximumDateForRecurrenceEnd));
				throw new RecurrenceFormatException(ServerStrings.ExStartDateCantBeGreaterThanMaximum(exDateTime4, Recurrence.MaximumDateForRecurrenceEnd), blobReader.BaseStream);
			}
			ExDateTime exDateTime5 = InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream).Date;
			if (createTimeZone != null)
			{
				exDateTime5 = createTimeZone.Assign(exDateTime5);
			}
			ExTraceGlobals.RecurrenceTracer.Information((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, PrimaryRecurrenceBlob range: {0}, numberOfOccurrence: {1}, startDate: {2}, endDate: {3} firstDayOfWeek: {4}", new object[]
			{
				recurrenceRangeType,
				num6,
				exDateTime4,
				exDateTime5,
				num7
			});
			RecurrenceRangeType recurrenceRangeType2 = recurrenceRangeType;
			RecurrenceRange range;
			if (recurrenceRangeType2 != RecurrenceRangeType.AlternateOutlookNoEnd)
			{
				switch (recurrenceRangeType2)
				{
				case RecurrenceRangeType.End:
					if (exDateTime4 > exDateTime5)
					{
						ExTraceGlobals.StorageTracer.TraceError<ExDateTime, ExDateTime>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, startDate({0}) is earlier then endDate in recurrence blob({1}).", exDateTime4, exDateTime5);
						throw new RecurrenceFormatException(ServerStrings.ExEndDateEarlierThanStartDate, blobReader.BaseStream);
					}
					if (exDateTime5 > Recurrence.MaximumDateForRecurrenceEnd)
					{
						ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), ServerStrings.ExEndDateCantExceedMaxDate(exDateTime5, Recurrence.MaximumDateForRecurrenceEnd));
						throw new RecurrenceFormatException(ServerStrings.ExEndDateCantExceedMaxDate(exDateTime5, Recurrence.MaximumDateForRecurrenceEnd), blobReader.BaseStream);
					}
					range = new EndDateRecurrenceRange(exDateTime4, exDateTime5);
					goto IL_640;
				case RecurrenceRangeType.AfterNOccur:
					if (num6 == 0)
					{
						ExTraceGlobals.RecurrenceTracer.TraceDebug<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, number of occurrences ({0}) in blob is zero and we treat as NoEndRecurrenceRange as Outlook.", num6);
						range = new NoEndRecurrenceRange(exDateTime4);
						anomaliesFlags |= AnomaliesFlags.NumOccurIsZeroSoTreatAsNoEndRange;
						goto IL_640;
					}
					if (num6 >= 1 && num6 <= 999)
					{
						range = new NumberedRecurrenceRange(exDateTime4, num6);
						goto IL_640;
					}
					ExTraceGlobals.RecurrenceTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, number of occurrences ({0}) in blob is less then 1", num6);
					throw new RecurrenceFormatException(ServerStrings.ExInvalidNumberOfOccurrences, blobReader.BaseStream);
				case RecurrenceRangeType.NoEnd:
					break;
				default:
					ExTraceGlobals.StorageTracer.TraceError<RecurrenceRangeType>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, Unknown recurrence range in recurrence blob, range = {0}", recurrenceRangeType);
					throw new RecurrenceFormatException(ServerStrings.ExUnknownRecurrenceBlobRange, blobReader.BaseStream);
				}
			}
			range = new NoEndRecurrenceRange(exDateTime4);
			IL_640:
			int num8 = 1440;
			int num9 = 0;
			RecurrenceTypeInBlob recurrenceTypeInBlob2 = recurrenceTypeInBlob;
			RecurrencePattern pattern;
			switch (recurrenceTypeInBlob2)
			{
			case RecurrenceTypeInBlob.Minute:
				if (num3 % num8 != 0 || num3 <= 0)
				{
					ExTraceGlobals.StorageTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, period in minutes for daily recurrence is not multiple of MinutesPerDay, period={0}", num3);
					throw new RecurrenceFormatException(ServerStrings.ExInvalidMinutePeriod(num3), blobReader.BaseStream);
				}
				switch (recurrenceGroup)
				{
				case RecurrenceGroup.Daily:
					num9 = num3 / num8;
					if (num9 > StorageLimits.Instance.RecurrenceMaximumInterval)
					{
						throw new RecurrenceFormatException(ServerStrings.ExInvalidRecurrenceInterval(num9), blobReader.BaseStream);
					}
					pattern = new DailyRecurrencePattern(num3 / num8);
					goto IL_C86;
				case RecurrenceGroup.Weekly:
					if (recurrencePatternType == RecurrencePatternType.Regenerating)
					{
						num9 = num3 / num8 / 7;
						if (num9 > StorageLimits.Instance.RecurrenceMaximumInterval)
						{
							throw new RecurrenceFormatException(ServerStrings.ExInvalidRecurrenceInterval(num9), blobReader.BaseStream);
						}
						pattern = new WeeklyRecurrencePattern(DaysOfWeek.Monday, num9);
						goto IL_C86;
					}
					break;
				}
				ExTraceGlobals.StorageTracer.TraceError<RecurrenceGroup>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, For recurrenceTypeInBlob of minutes, Pattern type is neither daily nor weekly, pattern={0}", recurrenceGroup);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidTypeGroupCombination(recurrenceTypeInBlob, recurrenceGroup), blobReader.BaseStream);
			case RecurrenceTypeInBlob.Week:
				if ((daysOfWeek & ~(DaysOfWeek.Sunday | DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday | DaysOfWeek.Saturday)) != DaysOfWeek.None || daysOfWeek == DaysOfWeek.None)
				{
					ExTraceGlobals.StorageTracer.TraceError<DaysOfWeek>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, days mask ({0}) for weekly recurrence has no valid flags set", daysOfWeek);
					throw new RecurrenceFormatException(ServerStrings.ExInvalidWeeklyDayMask(daysOfWeek), blobReader.BaseStream);
				}
				switch (recurrenceGroup)
				{
				case RecurrenceGroup.Daily:
				case RecurrenceGroup.Weekly:
					if (num3 <= 0 || num3 > StorageLimits.Instance.RecurrenceMaximumInterval)
					{
						ExTraceGlobals.StorageTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, period({0}) <= 0", num3);
						throw new RecurrenceFormatException(ServerStrings.ExInvalidRecurrenceInterval(num3), blobReader.BaseStream);
					}
					pattern = new WeeklyRecurrencePattern(daysOfWeek, num3, exDateTime.DayOfWeek);
					goto IL_C86;
				default:
					ExTraceGlobals.StorageTracer.TraceError<RecurrenceGroup>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, invalid group({0}) for RecurrenceTypeInBlob.Week", recurrenceGroup);
					throw new RecurrenceFormatException(ServerStrings.ExInvalidTypeGroupCombination(recurrenceTypeInBlob, recurrenceGroup), blobReader.BaseStream);
				}
				break;
			case RecurrenceTypeInBlob.Month:
				break;
			case RecurrenceTypeInBlob.MonthNth:
				goto IL_A3F;
			default:
				switch (recurrenceTypeInBlob2)
				{
				case RecurrenceTypeInBlob.HjMonth:
					break;
				case RecurrenceTypeInBlob.HjMonthNth:
					goto IL_A3F;
				default:
					ExTraceGlobals.StorageTracer.TraceError<RecurrenceTypeInBlob>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, Invalid RecurrenceTypInBlob: {0}", recurrenceTypeInBlob);
					throw new RecurrenceFormatException(ServerStrings.ExInvalidTypeInBlob(recurrenceTypeInBlob), blobReader.BaseStream);
				}
				break;
			}
			if (recurrenceTypeInBlob == RecurrenceTypeInBlob.HjMonth)
			{
				ExTraceGlobals.RecurrenceTracer.Information((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, HjMonth, changing calendarType");
				calendarType = CalendarType.Hijri;
				Recurrence.GetCalendar(calendarType);
			}
			if (num4 < 1 || num4 > 31)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, DayOfMonth is not in range: {0}", num4);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidMonthlyDayOfMonth(num4), blobReader.BaseStream);
			}
			switch (recurrenceGroup)
			{
			case RecurrenceGroup.Monthly:
				if (num3 <= 0 || num3 > StorageLimits.Instance.RecurrenceMaximumInterval)
				{
					ExTraceGlobals.StorageTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, period({0}) <= 0", num3);
					throw new RecurrenceFormatException(ServerStrings.ExInvalidRecurrenceInterval(num3), blobReader.BaseStream);
				}
				pattern = new MonthlyRecurrencePattern(num4, num3, calendarType);
				goto IL_C86;
			case RecurrenceGroup.Yearly:
			{
				if (num3 % 12 != 0)
				{
					throw new RecurrenceFormatException(ServerStrings.ExInvalidRecurrenceInterval(num3), blobReader.BaseStream);
				}
				num9 = num3 / 12;
				KeyValuePair<int, bool> monthOfYear;
				try
				{
					monthOfYear = InternalRecurrence.GetMonthOfYear(calendarType, exDateTime, exDateTime4);
				}
				catch (ArgumentOutOfRangeException ex2)
				{
					ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), ex2.Message);
					throw new RecurrenceCalendarTypeNotSupportedException(ServerStrings.ExCalendarTypeNotSupported(calendarType), calendarType, blobReader.BaseStream);
				}
				ExTraceGlobals.RecurrenceTracer.Information<int, bool>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, YearlyPattern: monthOfYear: {0} isLeapMonth: {1}", monthOfYear.Key, monthOfYear.Value);
				pattern = new YearlyRecurrencePattern(num4, monthOfYear.Key, monthOfYear.Value, num9, calendarType);
				goto IL_C86;
			}
			default:
				ExTraceGlobals.StorageTracer.TraceError<RecurrenceGroup>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, Invalid group for RecurrenceTypeInBlob.Month: {0}", recurrenceGroup);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidTypeGroupCombination(recurrenceTypeInBlob, recurrenceGroup), blobReader.BaseStream);
			}
			IL_A3F:
			if (recurrenceTypeInBlob == RecurrenceTypeInBlob.HjMonthNth)
			{
				ExTraceGlobals.RecurrenceTracer.Information((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, HjMonthNth, changing calendarType");
				calendarType = CalendarType.Hijri;
				Recurrence.GetCalendar(calendarType);
			}
			if ((num5 < 1 || num5 > 4) && num5 != -1)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, order is not in range for RecurenceTypeInBlob.MonthNth: {0}", num5);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidMonthNthOccurence(num5), blobReader.BaseStream);
			}
			if ((daysOfWeek & ~(DaysOfWeek.Sunday | DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday | DaysOfWeek.Saturday)) != DaysOfWeek.None || daysOfWeek == DaysOfWeek.None)
			{
				ExTraceGlobals.StorageTracer.TraceError<DaysOfWeek>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, dayMask is not valid: {0}", daysOfWeek);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidMonthNthDayMask(daysOfWeek), blobReader.BaseStream);
			}
			switch (recurrenceGroup)
			{
			case RecurrenceGroup.Monthly:
				if (num3 <= 0 || num3 > StorageLimits.Instance.RecurrenceMaximumInterval)
				{
					ExTraceGlobals.StorageTracer.TraceError<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, period({0}) <= 0", num3);
					throw new RecurrenceFormatException(ServerStrings.ExInvalidRecurrenceInterval(num3), blobReader.BaseStream);
				}
				pattern = new MonthlyThRecurrencePattern(daysOfWeek, (RecurrenceOrderType)num5, num3, calendarType);
				break;
			case RecurrenceGroup.Yearly:
			{
				if (num3 % 12 != 0)
				{
					throw new RecurrenceFormatException(ServerStrings.ExInvalidRecurrenceInterval(num3), blobReader.BaseStream);
				}
				num9 = num3 / 12;
				KeyValuePair<int, bool> monthOfYear2;
				try
				{
					monthOfYear2 = InternalRecurrence.GetMonthOfYear(calendarType, exDateTime, exDateTime4);
				}
				catch (ArgumentOutOfRangeException ex3)
				{
					ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), ex3.Message);
					throw new RecurrenceCalendarTypeNotSupportedException(ServerStrings.ExCalendarTypeNotSupported(calendarType), calendarType, blobReader.BaseStream);
				}
				ExTraceGlobals.RecurrenceTracer.Information<int, bool>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, YearlyPattern: monthOfYear: {0} isLeapMonth: {1}", monthOfYear2.Key, monthOfYear2.Value);
				pattern = new YearlyThRecurrencePattern(daysOfWeek, (RecurrenceOrderType)num5, monthOfYear2.Key, monthOfYear2.Value, num9, calendarType);
				break;
			}
			default:
				ExTraceGlobals.StorageTracer.TraceError<RecurrenceGroup>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, invalid group for RecurrenctTypInBlob.Month: {0}", recurrenceGroup);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidTypeGroupCombination(recurrenceTypeInBlob, recurrenceGroup), blobReader.BaseStream);
			}
			IL_C86:
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			recurrenceVersion = RecurrenceVersion.Outlook11;
			try
			{
				int num10 = blobReader.ReadInt32();
				recurrenceVersion = (RecurrenceVersion)blobReader.ReadInt32();
				timeSpan = TimeSpan.FromMinutes((double)blobReader.ReadInt32());
				timeSpan2 = TimeSpan.FromMinutes((double)blobReader.ReadInt32());
				ExTraceGlobals.RecurrenceTracer.Information((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::ParseRecurrenceBlob, readVersion: {0}, writeVersion: {1} startOffset: {2} endOffset: {3}", new object[]
				{
					num10,
					recurrenceVersion,
					timeSpan,
					timeSpan2
				});
			}
			catch (EndOfStreamException)
			{
			}
			InternalRecurrence result;
			try
			{
				if (recurrencePatternType == RecurrencePatternType.Regenerating)
				{
					pattern = Recurrence.TransformRecurrenceToRegenerating(pattern);
				}
				InternalRecurrence internalRecurrence = new InternalRecurrence(pattern, range, createTimeZone, readTimeZone, timeSpan, timeSpan2, sortedDictionary, sortedDictionary2, new ExDateTime?(exDateTime5));
				internalRecurrence.masterItem = masterItem;
				internalRecurrence.masterItemId = masterItemId;
				internalRecurrence.anomalies = anomaliesFlags;
				if (masterItem != null)
				{
					ExTraceGlobals.RecurrenceTracer.Information<int>((long)masterItem.GetHashCode(), "InternalRecurrence::InternalRecurrence, recurrence with hashCode: {0} created", internalRecurrence.GetHashCode());
				}
				else
				{
					ExTraceGlobals.RecurrenceTracer.Information<int>((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), "InternalRecurrence::InternalRecurrence, recurrence with hashCode: {0} created", internalRecurrence.GetHashCode());
				}
				result = internalRecurrence;
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)((masterItemId != null) ? masterItemId.GetHashCode() : 0), ex4.Message);
				throw new RecurrenceFormatException(ServerStrings.RecurrenceBlobCorrupted, blobReader.BaseStream, ex4);
			}
			return result;
		}

		private ExDateTime GetFirstOccurrenceAfter1601()
		{
			DayOfWeek dayOfWeek = (base.Pattern is WeeklyRecurrencePattern) ? ((WeeklyRecurrencePattern)base.Pattern).FirstDayOfWeek : DayOfWeek.Sunday;
			TimeSpan t = base.Range.StartDate.Date.LocalTime - Util.Date1601.LocalTime;
			int num = (base.Pattern is IntervalRecurrencePattern) ? ((IntervalRecurrencePattern)base.Pattern).RecurrenceInterval : 1;
			if (base.Pattern is RegeneratingPattern)
			{
				return base.Range.StartDate;
			}
			ExDateTime exDateTime;
			if (base.Pattern is DailyRecurrencePattern)
			{
				t = TimeSpan.FromDays((double)(t.Days % num));
				exDateTime = (ExDateTime)(Util.Date1601.LocalTime + t);
			}
			else if (base.Pattern is WeeklyRecurrencePattern)
			{
				DayOfWeek dayOfWeek2 = base.Range.StartDate.Date.DayOfWeek;
				exDateTime = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 1, 1).IncrementDays((((dayOfWeek2 >= dayOfWeek) ? (dayOfWeek - dayOfWeek2) : (dayOfWeek - ((DayOfWeek)7 + (int)dayOfWeek2))) + t.Days) % (num * 7));
			}
			else
			{
				if (base.Pattern is YearlyRecurrencePattern || base.Pattern is YearlyThRecurrencePattern)
				{
					return InternalRecurrence.GetFirstMonthStartAfter1601((IYearlyPatternInfo)base.Pattern, base.Range.StartDate);
				}
				if (!(base.Pattern is MonthlyRecurrencePattern) && !(base.Pattern is MonthlyThRecurrencePattern))
				{
					ExTraceGlobals.StorageTracer.TraceError<RecurrencePattern>((long)this.GetHashCode(), "InternalRecurrence::GetFirstOccurrenceAfter1601. The recurrence pattern is not recognized. Pattern = {0}.", base.Pattern);
					throw new CorruptDataException(ServerStrings.ExCorruptedRecurringCalItem);
				}
				IMonthlyPatternInfo monthlyPatternInfo = base.Pattern as IMonthlyPatternInfo;
				ExCalendar calendar = Recurrence.GetCalendar(monthlyPatternInfo.CalendarType);
				ExDateTime exDateTime2 = (calendar.MinSupportedDateTime > Util.Date1601) ? calendar.MinSupportedDateTime : InternalRecurrence.firstMonthsAfter1601[monthlyPatternInfo.CalendarType][0];
				int numberOfMonthsBetween = Recurrence.GetNumberOfMonthsBetween(exDateTime2, base.Range.StartDate, calendar);
				exDateTime = calendar.AddMonths(exDateTime2, numberOfMonthsBetween % num);
			}
			ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::GetFirstOccurrenceAfter, FirstPatternOccurrenceAfter 1601 is {0}", exDateTime);
			return exDateTime;
		}

		private string ReadExceptionInfoSubstring(BinaryReader blobReader, Encoding newEncoding)
		{
			string empty = string.Empty;
			if (newEncoding == null)
			{
				return empty;
			}
			int num = (int)blobReader.ReadInt16();
			int num2 = (int)blobReader.ReadInt16();
			if (num2 < 0)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError<int, int>((long)this.GetHashCode(), "InternalRecurrence::ReadExceptionInfoSubstring(), Invalid length for the exception info substring - LengthWithNull {0}, LengthWithoutNull {1} ", num, num2);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidExceptionInfoSubstringLength(num, num2), blobReader.BaseStream);
			}
			byte[] array = blobReader.ReadBytes(num2);
			if (array.Length < num2)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ReadExceptionInfoSubstring(), Incomplete substring in recurrence blob");
				throw new RecurrenceFormatException(ServerStrings.ExIncompleteBlob, blobReader.BaseStream);
			}
			try
			{
				return newEncoding.GetString(array, 0, array.Length);
			}
			catch (DecoderFallbackException)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ReadExceptionInfoSubstring(), Decoder.GetString threw a DecoderFallbackException");
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ReadExceptionInfoSubstring(), Decoder.GetString threw an ArgumentException");
			}
			return empty;
		}

		private Encoding GetEncodingForCodePage(int usingCodePageId)
		{
			Encoding encoding = null;
			try
			{
				encoding = Charset.GetEncoding(usingCodePageId);
			}
			catch (InvalidCharsetException)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError<int>((long)this.GetHashCode(), "InternalRecurrence::GetEncodingForCodePage(), Charset.GetEncoding threw an InvalidCharsetException for {0}", usingCodePageId);
			}
			if (encoding == null)
			{
				encoding = Charset.GetEncoding(CalendarItem.DefaultCodePage);
				if (encoding == null)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::GetEncodingForCodePage(), Charset.GetEncoding failed to get an Ascii decoder");
				}
			}
			return encoding;
		}

		private void ParseExceptionInfo(BinaryReader blobReader, RecurrenceVersion recurrenceVersion, int usingCodePageId)
		{
			Encoding encodingForCodePage = this.GetEncodingForCodePage(usingCodePageId);
			List<ExceptionInfo> list = new List<ExceptionInfo>();
			int num = (int)blobReader.ReadInt16();
			int num2 = num - this.exceptions.Count;
			if (num2 < 0)
			{
				ExTraceGlobals.StorageTracer.TraceError<int, int>((long)this.GetHashCode(), "InternalRecurrence::ParseExceptionInfo, Exception count({0}) in exception info does not match exception count in exception list({1})", num, this.exceptions.Count);
				throw new RecurrenceFormatException(ServerStrings.ExInvalidExceptionCount(this.exceptions.Count, num), blobReader.BaseStream);
			}
			ExTraceGlobals.RecurrenceTracer.Information<int>((long)this.GetHashCode(), "InternalRecurrence::ParseExceptionInfo, DuplicateExceptionCount: {0}", num2);
			SortedDictionary<ExDateTime, ExceptionInfo> sortedDictionary = new SortedDictionary<ExDateTime, ExceptionInfo>(this.exceptions);
			this.exceptions.Clear();
			for (int i = 0; i < num; i++)
			{
				MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
				memoryPropertyBag.Load(RecurrenceManager.PropertiesInTheBlobO11);
				memoryPropertyBag.ExTimeZone = this.ReadExTimeZone;
				ExDateTime exDateTime = base.CreatedExTimeZone.Assign(InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream));
				ExDateTime exDateTime2 = base.CreatedExTimeZone.Assign(InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream));
				ExDateTime exDateTime3 = base.CreatedExTimeZone.Assign(InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream));
				ExDateTime date = exDateTime3.Date;
				ExDateTime date2 = exDateTime.Date;
				ExTraceGlobals.RecurrenceTracer.Information<ExDateTime, ExDateTime, ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::ParseExceptionInfo, Exception originalStartTime: {0} startTime: {1} endTime: {2}", exDateTime3, exDateTime, exDateTime2);
				exDateTime = this.ReadExTimeZone.ConvertDateTime(exDateTime);
				exDateTime2 = this.ReadExTimeZone.ConvertDateTime(exDateTime2);
				VersionedId versionedId = null;
				if (this.MasterItemId != null)
				{
					OccurrenceStoreObjectId itemId = new OccurrenceStoreObjectId(this.MasterItemId.ObjectId.ProviderLevelItemId, exDateTime3);
					versionedId = new VersionedId(itemId, this.MasterItemId.ChangeKeyAsByteArray());
				}
				memoryPropertyBag[InternalSchema.MapiStartTime] = exDateTime;
				memoryPropertyBag[InternalSchema.MapiEndTime] = exDateTime2;
				ModificationType modificationType = (ModificationType)blobReader.ReadInt16();
				ExTraceGlobals.RecurrenceTracer.Information<ExDateTime, ModificationType>((long)this.GetHashCode(), "InternalRecurrence::ParseExceptionInfo, Modification in exception dateId: {0} are {1}", date, modificationType);
				memoryPropertyBag.Load(RecurrenceManager.PropertiesInTheBlobO11);
				ExceptionInfo exceptionInfo = new ExceptionInfo(versionedId, date, this.ReadExTimeZone.ConvertDateTime(exDateTime3), exDateTime, exDateTime2, modificationType, memoryPropertyBag);
				list.Add(exceptionInfo);
				if (!this.deletions.ContainsKey(date))
				{
					ExTraceGlobals.StorageTracer.TraceError<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::ParseExceptionInfo, original date({0}) in exception info does not match original date in exception list", exDateTime3);
					this.anomalies |= AnomaliesFlags.MismatchedOriginalDateFromExceptionList;
				}
				else
				{
					this.deletions.Remove(date);
					sortedDictionary.Remove(date2);
					if (base.IsValidOccurrenceId(exceptionInfo.OccurrenceDateId))
					{
						this.exceptions[date] = exceptionInfo;
					}
					else
					{
						ExTraceGlobals.StorageTracer.TraceError<ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::ParseExceptionInfo, Exception present which does not fall on recurrence pattern {0}", exceptionInfo.OccurrenceDateId);
						this.anomalies |= AnomaliesFlags.ExtraExceptionNotInPattern;
					}
				}
				if ((modificationType & ModificationType.Subject) == ModificationType.Subject)
				{
					string value = this.ReadExceptionInfoSubstring(blobReader, encodingForCodePage);
					memoryPropertyBag[InternalSchema.Subject] = value;
				}
				if ((modificationType & ModificationType.MeetingType) != (ModificationType)0)
				{
					memoryPropertyBag[InternalSchema.AppointmentState] = blobReader.ReadInt32();
				}
				if ((modificationType & ModificationType.ReminderDelta) != (ModificationType)0)
				{
					int num3 = blobReader.ReadInt32();
					memoryPropertyBag[InternalSchema.ReminderMinutesBeforeStartInternal] = num3;
				}
				if ((modificationType & ModificationType.Reminder) != (ModificationType)0)
				{
					int num4 = blobReader.ReadInt32();
					bool flag = num4 != 0;
					memoryPropertyBag[InternalSchema.ReminderIsSetInternal] = flag;
				}
				if ((modificationType & ModificationType.Location) != (ModificationType)0)
				{
					string value2 = this.ReadExceptionInfoSubstring(blobReader, encodingForCodePage);
					memoryPropertyBag[InternalSchema.Location] = value2;
				}
				if ((modificationType & ModificationType.BusyStatus) != (ModificationType)0)
				{
					int num5 = blobReader.ReadInt32();
					memoryPropertyBag[InternalSchema.FreeBusyStatus] = num5;
				}
				if ((modificationType & ModificationType.Attachment) != (ModificationType)0)
				{
					int num6 = blobReader.ReadInt32();
					memoryPropertyBag[InternalSchema.MapiHasAttachment] = (num6 != 0);
					memoryPropertyBag[InternalSchema.AllAttachmentsHidden] = false;
				}
				if ((modificationType & ModificationType.SubType) != (ModificationType)0)
				{
					int num7 = blobReader.ReadInt32();
					memoryPropertyBag[InternalSchema.MapiIsAllDayEvent] = (num7 != 0);
				}
				if ((modificationType & ModificationType.Color) != (ModificationType)0)
				{
					int num8 = blobReader.ReadInt32();
					memoryPropertyBag[InternalSchema.AppointmentColor] = num8;
				}
			}
			if (recurrenceVersion >= RecurrenceVersion.Outlook11)
			{
				this.ParseExtendedExceptionInfo(blobReader, recurrenceVersion, list);
			}
		}

		private void ParseExtendedExceptionInfo(BinaryReader blobReader, RecurrenceVersion recurrenceBlobVersion, List<ExceptionInfo> exceptionOrder)
		{
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			InternalRecurrence.SkipBlock(blobReader);
			foreach (ExceptionInfo exceptionInfo in exceptionOrder)
			{
				int num = blobReader.ReadInt32();
				if (num == 4)
				{
					exceptionInfo.PropertyBag[InternalSchema.ChangeHighlight] = blobReader.ReadInt32();
					InternalRecurrence.SkipBlock(blobReader);
				}
				else if (num > 0)
				{
					throw new RecurrenceFormatException(ServerStrings.ExInvalidO12BytesToSkip(num), blobReader.BaseStream);
				}
				ModificationType modificationType = exceptionInfo.ModificationType;
				if ((modificationType & ModificationType.Subject) == ModificationType.Subject || (modificationType & ModificationType.Location) == ModificationType.Location)
				{
					ExDateTime exDateTime = base.CreatedExTimeZone.Assign(InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream));
					exDateTime = this.ReadExTimeZone.ConvertDateTime(exDateTime);
					if (exceptionInfo.StartTime != exDateTime)
					{
						ExTraceGlobals.StorageTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::ParseExtendedExceptionInfo, StartTime in extended exception({0}) info does not match startTime in exception info({1})", exDateTime, exceptionInfo.StartTime);
						throw new RecurrenceFormatException(ServerStrings.ExTimeInExtendedInfoNotSameAsExceptionInfo("Start time", exceptionInfo.StartTime, exDateTime), blobReader.BaseStream);
					}
					exDateTime = base.CreatedExTimeZone.Assign(InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream));
					exDateTime = this.ReadExTimeZone.ConvertDateTime(exDateTime);
					if (exceptionInfo.EndTime != exDateTime)
					{
						ExTraceGlobals.StorageTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::ParseExtendedExceptionInfo, EndTime in extended exception({0}) info does not match startTime in exception info({1})", exDateTime, exceptionInfo.EndTime);
						throw new RecurrenceFormatException(ServerStrings.ExTimeInExtendedInfoNotSameAsExceptionInfo("Endtime", exceptionInfo.EndTime, exDateTime), blobReader.BaseStream);
					}
					exDateTime = base.CreatedExTimeZone.Assign(InternalRecurrence.FileTimeToDateTime(blobReader.ReadInt32(), blobReader.BaseStream));
					exDateTime = this.ReadExTimeZone.ConvertDateTime(exDateTime);
					if (exceptionInfo.OriginalStartTime.Date != exDateTime.Date)
					{
						ExTraceGlobals.StorageTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "InternalRecurrence::ParseExtendedExceptionInfo, OriginalDate({0}) from extended exception info does not match originalDate in exceptions({1})", exDateTime, exceptionInfo.OriginalStartTime);
						throw new RecurrenceFormatException(ServerStrings.ExIncorrectOriginalTimeInExtendedExceptionInfo(exceptionInfo.OriginalStartTime.Date, exDateTime.Date), blobReader.BaseStream);
					}
					if ((modificationType & ModificationType.Subject) != (ModificationType)0)
					{
						int num2 = (int)blobReader.ReadInt16();
						InternalRecurrence.CheckFieldLength(num2 * 2, blobReader.BaseStream);
						byte[] array = blobReader.ReadBytes(num2 * 2);
						if (array.Length < num2 * 2)
						{
							ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ParseExtendedExceptionInfo, Incomplete unicode subject in recurrence blob");
							throw new RecurrenceFormatException(ServerStrings.ExIncompleteBlob, blobReader.BaseStream);
						}
						string @string = unicodeEncoding.GetString(array, 0, array.Length);
						exceptionInfo.PropertyBag[InternalSchema.Subject] = @string;
					}
					if ((modificationType & ModificationType.Location) != (ModificationType)0)
					{
						int num3 = (int)blobReader.ReadInt16();
						InternalRecurrence.CheckFieldLength(num3 * 2, blobReader.BaseStream);
						byte[] array2 = blobReader.ReadBytes(num3 * 2);
						if (array2.Length < num3 * 2)
						{
							ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "InternalRecurrence::ParseExtendedExceptionInfo, Incomplete unicode location in recurrence blob");
							throw new RecurrenceFormatException(ServerStrings.ExIncompleteBlob, blobReader.BaseStream);
						}
						string string2 = unicodeEncoding.GetString(array2, 0, array2.Length);
						exceptionInfo.PropertyBag[InternalSchema.Location] = string2;
					}
					InternalRecurrence.SkipBlock(blobReader);
				}
			}
			InternalRecurrence.SkipBlock(blobReader);
		}

		private void GeneratePrimaryRecurrenceBlob(BinaryWriter blobWriter)
		{
			blobWriter.Write(12292);
			blobWriter.Write(12292);
			int value = 0;
			int value2 = 0;
			int num = 0;
			RecurrenceTypeInBlob recurrenceTypeInBlob;
			RecurrenceGroup recurrenceGroup;
			int value3;
			if (base.Pattern is DailyRecurrencePattern)
			{
				recurrenceTypeInBlob = RecurrenceTypeInBlob.Minute;
				recurrenceGroup = RecurrenceGroup.Daily;
				value3 = ((DailyRecurrencePattern)base.Pattern).RecurrenceInterval * 24 * 60;
			}
			else if (base.Pattern is WeeklyRecurrencePattern)
			{
				WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)base.Pattern;
				recurrenceTypeInBlob = RecurrenceTypeInBlob.Week;
				recurrenceGroup = RecurrenceGroup.Weekly;
				value = (int)weeklyRecurrencePattern.DaysOfWeek;
				value3 = weeklyRecurrencePattern.RecurrenceInterval;
			}
			else if (base.Pattern is MonthlyRecurrencePattern)
			{
				MonthlyRecurrencePattern monthlyRecurrencePattern = (MonthlyRecurrencePattern)base.Pattern;
				recurrenceTypeInBlob = ((monthlyRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonth : RecurrenceTypeInBlob.Month);
				recurrenceGroup = RecurrenceGroup.Monthly;
				value2 = monthlyRecurrencePattern.DayOfMonth;
				value3 = monthlyRecurrencePattern.RecurrenceInterval;
			}
			else if (base.Pattern is YearlyRecurrencePattern)
			{
				YearlyRecurrencePattern yearlyRecurrencePattern = (YearlyRecurrencePattern)base.Pattern;
				recurrenceTypeInBlob = ((yearlyRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonth : RecurrenceTypeInBlob.Month);
				recurrenceGroup = RecurrenceGroup.Yearly;
				value2 = yearlyRecurrencePattern.DayOfMonth;
				value3 = 12 * yearlyRecurrencePattern.RecurrenceInterval;
			}
			else if (base.Pattern is MonthlyThRecurrencePattern)
			{
				MonthlyThRecurrencePattern monthlyThRecurrencePattern = (MonthlyThRecurrencePattern)base.Pattern;
				recurrenceTypeInBlob = ((monthlyThRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonthNth : RecurrenceTypeInBlob.MonthNth);
				recurrenceGroup = RecurrenceGroup.Monthly;
				value = (int)monthlyThRecurrencePattern.DaysOfWeek;
				num = (int)monthlyThRecurrencePattern.Order;
				value3 = monthlyThRecurrencePattern.RecurrenceInterval;
			}
			else if (base.Pattern is YearlyThRecurrencePattern)
			{
				YearlyThRecurrencePattern yearlyThRecurrencePattern = (YearlyThRecurrencePattern)base.Pattern;
				recurrenceTypeInBlob = ((yearlyThRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonthNth : RecurrenceTypeInBlob.MonthNth);
				recurrenceGroup = RecurrenceGroup.Yearly;
				value = (int)yearlyThRecurrencePattern.DaysOfWeek;
				num = (int)yearlyThRecurrencePattern.Order;
				value3 = 12 * yearlyThRecurrencePattern.RecurrenceInterval;
			}
			else if (base.Pattern is DailyRegeneratingPattern)
			{
				recurrenceTypeInBlob = RecurrenceTypeInBlob.Minute;
				recurrenceGroup = RecurrenceGroup.Daily;
				value3 = ((DailyRegeneratingPattern)base.Pattern).RecurrenceInterval * 24 * 60;
			}
			else if (base.Pattern is WeeklyRegeneratingPattern)
			{
				WeeklyRegeneratingPattern weeklyRegeneratingPattern = (WeeklyRegeneratingPattern)base.Pattern;
				recurrenceTypeInBlob = RecurrenceTypeInBlob.Minute;
				recurrenceGroup = RecurrenceGroup.Weekly;
				value = 2;
				value3 = weeklyRegeneratingPattern.RecurrenceInterval * 7 * 24 * 60;
			}
			else if (base.Pattern is MonthlyRegeneratingPattern)
			{
				MonthlyRegeneratingPattern monthlyRegeneratingPattern = (MonthlyRegeneratingPattern)base.Pattern;
				recurrenceTypeInBlob = RecurrenceTypeInBlob.Month;
				recurrenceGroup = RecurrenceGroup.Monthly;
				value = 2;
				value2 = 1;
				num = 0;
				value3 = monthlyRegeneratingPattern.RecurrenceInterval;
			}
			else
			{
				if (!(base.Pattern is YearlyRegeneratingPattern))
				{
					throw new ArgumentException(ServerStrings.ExUnknownPattern(base.Pattern));
				}
				YearlyRegeneratingPattern yearlyRegeneratingPattern = (YearlyRegeneratingPattern)base.Pattern;
				recurrenceTypeInBlob = RecurrenceTypeInBlob.Month;
				recurrenceGroup = RecurrenceGroup.Yearly;
				value2 = 1;
				value3 = 12 * yearlyRegeneratingPattern.RecurrenceInterval;
			}
			blobWriter.Write((short)recurrenceGroup);
			blobWriter.Write((short)recurrenceTypeInBlob);
			CalendarType calendarType = CalendarType.Default;
			if (base.Pattern is IMonthlyPatternInfo)
			{
				calendarType = ((IMonthlyPatternInfo)base.Pattern).CalendarType;
			}
			if (calendarType == CalendarType.Hijri || calendarType == CalendarType.Gregorian)
			{
				calendarType = CalendarType.Default;
			}
			blobWriter.Write((short)calendarType);
			blobWriter.Write(InternalRecurrence.DateTimeToFileTime(this.firstInstanceSince1601));
			blobWriter.Write(value3);
			if (base.Pattern is RegeneratingPattern)
			{
				blobWriter.Write(1);
			}
			else
			{
				blobWriter.Write(0);
			}
			RecurrenceTypeInBlob recurrenceTypeInBlob2 = recurrenceTypeInBlob;
			switch (recurrenceTypeInBlob2)
			{
			case RecurrenceTypeInBlob.Week:
				blobWriter.Write(value);
				goto IL_370;
			case RecurrenceTypeInBlob.Month:
				break;
			case RecurrenceTypeInBlob.MonthNth:
				goto IL_359;
			default:
				switch (recurrenceTypeInBlob2)
				{
				case RecurrenceTypeInBlob.HjMonth:
					break;
				case RecurrenceTypeInBlob.HjMonthNth:
					goto IL_359;
				default:
					goto IL_370;
				}
				break;
			}
			blobWriter.Write(value2);
			goto IL_370;
			IL_359:
			blobWriter.Write(value);
			blobWriter.Write((num == -1) ? 5 : num);
			IL_370:
			int value4 = 10;
			RecurrenceRangeType value5;
			if (base.Range is EndDateRecurrenceRange)
			{
				value5 = RecurrenceRangeType.End;
				value4 = base.NumberOfOccurrences;
			}
			else if (base.Range is NumberedRecurrenceRange)
			{
				value5 = RecurrenceRangeType.AfterNOccur;
				value4 = ((NumberedRecurrenceRange)base.Range).NumberOfOccurrences;
			}
			else
			{
				value5 = RecurrenceRangeType.NoEnd;
			}
			blobWriter.Write((int)value5);
			blobWriter.Write(value4);
			if (base.Pattern is IWeeklyPatternInfo)
			{
				blobWriter.Write((int)((IWeeklyPatternInfo)base.Pattern).FirstDayOfWeek);
			}
			else
			{
				blobWriter.Write(0);
			}
			List<ExDateTime> list = new List<ExDateTime>();
			list.AddRange(this.deletions.Keys);
			list.AddRange(this.exceptions.Keys);
			list.Sort();
			blobWriter.Write(list.Count);
			foreach (ExDateTime date in list)
			{
				blobWriter.Write(InternalRecurrence.DateTimeToFileTime(date));
			}
			blobWriter.Write(this.exceptions.Count);
			foreach (ExceptionInfo exceptionInfo in this.exceptions.Values)
			{
				blobWriter.Write(InternalRecurrence.DateTimeToFileTime(base.CreatedExTimeZone.ConvertDateTime(exceptionInfo.StartTime).Date));
			}
			blobWriter.Write(InternalRecurrence.DateTimeToFileTime(base.Range.StartDate));
			if (base.Range is NoEndRecurrenceRange)
			{
				blobWriter.Write(1525252319);
			}
			else
			{
				blobWriter.Write(InternalRecurrence.DateTimeToFileTime(base.EndDate));
			}
			blobWriter.Write(12294);
			blobWriter.Write(12297);
			blobWriter.Write(Convert.ToInt32(base.StartOffset.TotalMinutes));
			blobWriter.Write(Convert.ToInt32(base.EndOffset.TotalMinutes));
		}

		private void GenerateExceptionInfo(BinaryWriter blobWriter)
		{
			blobWriter.Write((short)this.exceptions.Count);
			foreach (KeyValuePair<ExDateTime, ExceptionInfo> keyValuePair in this.exceptions)
			{
				blobWriter.Write(InternalRecurrence.DateTimeToFileTime(base.CreatedExTimeZone.ConvertDateTime(keyValuePair.Value.StartTime)));
				blobWriter.Write(InternalRecurrence.DateTimeToFileTime(base.CreatedExTimeZone.ConvertDateTime(keyValuePair.Value.EndTime)));
				blobWriter.Write(InternalRecurrence.DateTimeToFileTime(new ExDateTime(base.CreatedExTimeZone, keyValuePair.Key.LocalTime + base.StartOffset)));
				keyValuePair.Value.ModificationType = (keyValuePair.Value.ModificationType & ModificationType.Body);
				foreach (KeyValuePair<NativeStorePropertyDefinition, ModificationType> keyValuePair2 in InternalRecurrence.propertyModification)
				{
					object propertyValue = keyValuePair.Value.PropertyBag.TryGetProperty(keyValuePair2.Key);
					if (!PropertyError.IsPropertyError(propertyValue))
					{
						keyValuePair.Value.ModificationType |= keyValuePair2.Value;
					}
				}
				blobWriter.Write((short)keyValuePair.Value.ModificationType);
				if ((keyValuePair.Value.ModificationType & ModificationType.Subject) == ModificationType.Subject)
				{
					string s = (string)keyValuePair.Value.PropertyBag[InternalSchema.Subject];
					byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(s);
					blobWriter.Write((short)(bytes.Length + 1));
					blobWriter.Write((short)bytes.Length);
					blobWriter.Write(bytes);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.MeetingType) == ModificationType.MeetingType)
				{
					blobWriter.Write((int)keyValuePair.Value.PropertyBag[InternalSchema.AppointmentState]);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.ReminderDelta) == ModificationType.ReminderDelta)
				{
					blobWriter.Write((int)keyValuePair.Value.PropertyBag[InternalSchema.ReminderMinutesBeforeStartInternal]);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.Reminder) == ModificationType.Reminder)
				{
					blobWriter.Write(((bool)keyValuePair.Value.PropertyBag[InternalSchema.ReminderIsSetInternal]) ? 1 : 0);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.Location) == ModificationType.Location)
				{
					string s2 = (string)keyValuePair.Value.PropertyBag[InternalSchema.Location];
					byte[] bytes2 = CTSGlobals.AsciiEncoding.GetBytes(s2);
					blobWriter.Write((short)(bytes2.Length + 1));
					blobWriter.Write((short)bytes2.Length);
					blobWriter.Write(bytes2);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.BusyStatus) != (ModificationType)0)
				{
					int value = (int)keyValuePair.Value.PropertyBag[InternalSchema.FreeBusyStatus];
					blobWriter.Write(value);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.Attachment) != (ModificationType)0)
				{
					blobWriter.Write(((bool)keyValuePair.Value.PropertyBag[InternalSchema.MapiHasAttachment]) ? 1 : 0);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.SubType) != (ModificationType)0)
				{
					blobWriter.Write(((bool)keyValuePair.Value.PropertyBag[InternalSchema.MapiIsAllDayEvent]) ? 1 : 0);
				}
				if ((keyValuePair.Value.ModificationType & ModificationType.Color) != (ModificationType)0)
				{
					int value2 = (int)keyValuePair.Value.PropertyBag[InternalSchema.AppointmentColor];
					blobWriter.Write(value2);
				}
			}
		}

		private void GenerateExtendedExceptionInfo(BinaryWriter blobWriter)
		{
			blobWriter.Write(0);
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			foreach (KeyValuePair<ExDateTime, ExceptionInfo> keyValuePair in this.exceptions)
			{
				blobWriter.Write(4);
				int valueOrDefault = keyValuePair.Value.PropertyBag.GetValueOrDefault<int>(InternalSchema.ChangeHighlight);
				blobWriter.Write(valueOrDefault);
				blobWriter.Write(0);
				if ((keyValuePair.Value.ModificationType & ModificationType.Subject) == ModificationType.Subject || (keyValuePair.Value.ModificationType & ModificationType.Location) == ModificationType.Location)
				{
					blobWriter.Write(InternalRecurrence.DateTimeToFileTime(base.CreatedExTimeZone.ConvertDateTime(keyValuePair.Value.StartTime)));
					blobWriter.Write(InternalRecurrence.DateTimeToFileTime(base.CreatedExTimeZone.ConvertDateTime(keyValuePair.Value.EndTime)));
					blobWriter.Write(InternalRecurrence.DateTimeToFileTime(new ExDateTime(base.CreatedExTimeZone, keyValuePair.Key.LocalTime + base.StartOffset)));
					if ((keyValuePair.Value.ModificationType & ModificationType.Subject) == ModificationType.Subject)
					{
						string text = (string)keyValuePair.Value.PropertyBag[InternalSchema.Subject];
						byte[] bytes = unicodeEncoding.GetBytes(text);
						blobWriter.Write((short)text.Length);
						blobWriter.Write(bytes);
					}
					if ((keyValuePair.Value.ModificationType & ModificationType.Location) == ModificationType.Location)
					{
						string text2 = (string)keyValuePair.Value.PropertyBag[InternalSchema.Location];
						byte[] bytes2 = unicodeEncoding.GetBytes(text2);
						blobWriter.Write((short)text2.Length);
						blobWriter.Write(bytes2);
					}
					blobWriter.Write(0);
				}
			}
			blobWriter.Write(0);
		}

		private const int ExchangeStoreCellMaxNew = 25600;

		private const int ExchangeStoreCellMaxOld = 510;

		private const int MaxRecurrenceBlobFieldLength = 32768;

		private readonly SortedDictionary<ExDateTime, ExceptionInfo> exceptions;

		private readonly SortedDictionary<ExDateTime, object> deletions;

		private readonly ExDateTime firstInstanceSince1601;

		private static Dictionary<CalendarType, ExDateTime[]> firstMonthsAfter1601 = InternalRecurrence.GenerateTableForMonthStartDatesAfter1601();

		private static Dictionary<NativeStorePropertyDefinition, ModificationType> propertyModification = InternalRecurrence.BuildPropertyModification(InternalRecurrence.propertyModification);

		private Item masterItem;

		private VersionedId masterItemId;

		private AnomaliesFlags anomalies;

		public new static readonly StorePropertyDefinition[] RequiredRecurrenceProperties = new StorePropertyDefinition[]
		{
			InternalSchema.ItemId,
			InternalSchema.AppointmentRecurrenceBlob,
			InternalSchema.TimeZone,
			InternalSchema.TimeZoneBlob,
			InternalSchema.TimeZoneDefinitionRecurring
		};

		internal interface IOccurrenceIterator
		{
			OccurrenceInfo Current { get; }

			InternalRecurrence.IOccurrenceIterator Move(bool forward);
		}

		private struct ExceptionOccurrenceIterator : InternalRecurrence.IOccurrenceIterator
		{
			internal ExceptionOccurrenceIterator(IList<OccurrenceInfo> exceptionList, int iException)
			{
				this = new InternalRecurrence.ExceptionOccurrenceIterator(exceptionList, iException, false);
			}

			private ExceptionOccurrenceIterator(IList<OccurrenceInfo> exceptionList, int iException, bool initialized)
			{
				this.exceptionList = exceptionList;
				this.iException = iException;
				this.initialized = initialized;
			}

			public OccurrenceInfo Current
			{
				get
				{
					if (!this.initialized)
					{
						throw new InvalidOperationException("Call Move before calling Current");
					}
					if (this.iException < 0 || this.iException >= this.exceptionList.Count)
					{
						return null;
					}
					return this.exceptionList[this.iException];
				}
			}

			public InternalRecurrence.IOccurrenceIterator Move(bool forward)
			{
				int num = forward ? (this.iException + 1) : (this.iException - 1);
				if (num < 0)
				{
					num = -1;
				}
				else if (num >= this.exceptionList.Count)
				{
					num = this.exceptionList.Count;
				}
				return new InternalRecurrence.ExceptionOccurrenceIterator(this.exceptionList, num, true);
			}

			private bool initialized;

			private int iException;

			private IList<OccurrenceInfo> exceptionList;
		}

		private struct OccurrenceIterator : InternalRecurrence.IOccurrenceIterator
		{
			internal OccurrenceIterator(InternalRecurrence recurrence, ExDateTime instance)
			{
				this = new InternalRecurrence.OccurrenceIterator(recurrence, instance, false);
			}

			private OccurrenceIterator(InternalRecurrence recurrence, ExDateTime instance, bool initialized)
			{
				this.recurrence = recurrence;
				this.instance = instance;
				this.initialized = initialized;
			}

			public OccurrenceInfo Current
			{
				get
				{
					if (!this.initialized)
					{
						throw new InvalidOperationException("Call Move before calling Current");
					}
					if (!this.recurrence.IsValidOccurrenceId(this.instance))
					{
						return null;
					}
					return this.recurrence.GetOccurrenceInfoByDateId(this.instance);
				}
			}

			public InternalRecurrence.IOccurrenceIterator Move(bool forward)
			{
				ExDateTime exDateTime = this.instance;
				do
				{
					exDateTime = (forward ? this.recurrence.GetNextOccurrenceDateId(exDateTime) : this.recurrence.GetPreviousOccurrenceDateId(exDateTime));
				}
				while (this.recurrence.IsValidOccurrenceId(exDateTime) && this.recurrence.IsOccurrenceDeleted(exDateTime));
				return new InternalRecurrence.OccurrenceIterator(this.recurrence, exDateTime, true);
			}

			private bool initialized;

			private InternalRecurrence recurrence;

			private ExDateTime instance;
		}
	}
}
