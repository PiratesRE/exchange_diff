using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RecurrenceBlobMerger
	{
		private CalendarItem CalendarItem { get; set; }

		private string GlobalObjectId { get; set; }

		private StoreSession Session { get; set; }

		private InternalRecurrence OriginalRecurrence { get; set; }

		private InternalRecurrence NewRecurrence { get; set; }

		private IList<ExDateTime> OccurrencesToDelete { get; set; }

		private List<ExDateTime> OccurrencesToRevive { get; set; }

		public static bool Merge(StoreSession session, CalendarItem calendarItem, GlobalObjectId globalObjectId, InternalRecurrence originalRecurrence, InternalRecurrence newRecurrence)
		{
			return new RecurrenceBlobMerger(session, calendarItem, globalObjectId, originalRecurrence, newRecurrence).Merge();
		}

		private RecurrenceBlobMerger(StoreSession session, CalendarItem calendarItem, GlobalObjectId globalObjectId, InternalRecurrence originalRecurrence, InternalRecurrence newRecurrence)
		{
			this.Session = session;
			this.CalendarItem = calendarItem;
			this.GlobalObjectId = ((globalObjectId == null) ? string.Empty : globalObjectId.ToString());
			this.OriginalRecurrence = originalRecurrence;
			this.NewRecurrence = newRecurrence;
		}

		private bool Merge()
		{
			this.CalculateLocalDeletions();
			this.CalculateOccurrencesToRevive();
			bool flag = this.CopyLocalModifications();
			flag |= this.CopyLocalDeletions();
			return flag | this.ReviveCancelledOcurrences();
		}

		private bool CopyLocalModifications()
		{
			bool flag = false;
			foreach (OccurrenceInfo occurrenceInfo in this.OriginalRecurrence.GetModifiedOccurrences())
			{
				ExceptionInfo exceptionInfo = occurrenceInfo as ExceptionInfo;
				if (exceptionInfo != null)
				{
					if (this.NewRecurrence.IsValidOccurrenceId(exceptionInfo.OccurrenceDateId))
					{
						if (!this.NewRecurrence.IsOccurrenceDeleted(exceptionInfo.OccurrenceDateId) && !(this.NewRecurrence.GetOccurrenceInfoByDateId(exceptionInfo.OccurrenceDateId) is ExceptionInfo))
						{
							this.NewRecurrence.ModifyOccurrence(exceptionInfo);
							flag = true;
							ExTraceGlobals.RecurrenceTracer.Information<string, ExDateTime>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.CopyLocalModifications: GOID={0} has revived a local exception with id={1}", this.GlobalObjectId, exceptionInfo.OccurrenceDateId);
						}
					}
					else if (this.CalendarItem != null)
					{
						ExTraceGlobals.RecurrenceTracer.Information<string>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.CopyLocalModifications: GOID={0} will remove the attachment for an invalid occurrence", this.GlobalObjectId);
						RecurrenceManager.DeleteAttachment(this.CalendarItem, this.NewRecurrence.CreatedExTimeZone, exceptionInfo.StartTime, exceptionInfo.EndTime);
					}
				}
			}
			ExTraceGlobals.RecurrenceTracer.Information<string, bool>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.CopyLocalModifications: GOID={0}; will return {1}", this.GlobalObjectId, flag);
			return flag;
		}

		private bool CopyLocalDeletions()
		{
			bool flag = false;
			foreach (ExDateTime exDateTime in this.OccurrencesToDelete)
			{
				if (this.NewRecurrence.TryDeleteOccurrence(exDateTime.Date))
				{
					flag = true;
					ExTraceGlobals.RecurrenceTracer.Information<string, ExDateTime>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.CopyLocalDeletions has deleted an occurrence: GOID={0}; Occurrence id={1}", this.GlobalObjectId, exDateTime.Date);
				}
			}
			ExTraceGlobals.RecurrenceTracer.Information<string, bool>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.CopyLocalDeletions: GOID={0}; will return {1}", this.GlobalObjectId, flag);
			return flag;
		}

		private bool ReviveCancelledOcurrences()
		{
			bool flag = false;
			foreach (ExDateTime exDateTime in this.OccurrencesToRevive)
			{
				ExDateTime date = exDateTime.Date;
				ExceptionInfo exceptionInfo = this.OriginalRecurrence.GetOccurrenceInfoByDateId(date) as ExceptionInfo;
				if (exceptionInfo != null && this.NewRecurrence.TryUndeleteOccurrence(date))
				{
					this.NewRecurrence.ModifyOccurrence(exceptionInfo);
					flag = true;
					ExTraceGlobals.RecurrenceTracer.Information<string, ExDateTime>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.ReviveCancelledOcurrences has revived an occurrence: GOID={0}; Occurrence Id={1}", this.GlobalObjectId, date);
				}
			}
			ExTraceGlobals.RecurrenceTracer.Information<string, bool>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.ReviveCancelledOcurrences: GOID={0}; will return {1}", this.GlobalObjectId, flag);
			return flag;
		}

		private void CalculateLocalDeletions()
		{
			this.OccurrencesToDelete = Array.FindAll<ExDateTime>(this.OriginalRecurrence.GetDeletedOccurrences(false), (ExDateTime occurrence) => this.NewRecurrence.IsValidOccurrenceId(occurrence.Date) && !this.NewRecurrence.IsOccurrenceDeleted(occurrence.Date));
			ExTraceGlobals.RecurrenceTracer.Information<string>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.CalculateLocalDeletions: GOID={0}", this.GlobalObjectId);
		}

		private void CalculateOccurrencesToRevive()
		{
			this.OccurrencesToRevive = new List<ExDateTime>();
			foreach (ExDateTime exDateTime in this.NewRecurrence.GetDeletedOccurrences(false))
			{
				ExDateTime date = exDateTime.Date;
				if (this.OriginalRecurrence.IsValidOccurrenceId(date) && !this.OriginalRecurrence.IsOccurrenceDeleted(date))
				{
					OccurrenceInfo occurrenceInfoByDateId = this.OriginalRecurrence.GetOccurrenceInfoByDateId(date);
					using (CalendarItemOccurrence calendarItemOccurrence = CalendarItemOccurrence.Bind(this.Session, occurrenceInfoByDateId.VersionedId))
					{
						if (calendarItemOccurrence.IsCancelled)
						{
							this.OccurrencesToRevive.Add(occurrenceInfoByDateId.OccurrenceDateId);
						}
					}
				}
			}
			ExTraceGlobals.RecurrenceTracer.Information<string>((long)this.GetHashCode(), "Storage.RecurrenceBlobMerger.CalculateOccurrencesToRevive: GOID={0}", this.GlobalObjectId);
		}
	}
}
