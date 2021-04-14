using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TaskRecurrence : Recurrence
	{
		internal TaskRecurrence(RecurrencePattern pattern, RecurrenceRange range, TimeSpan startOffset, TimeSpan endOffset, ExTimeZone timezone, ExTimeZone readTimeZone, ExDateTime? endDateOverride) : base(pattern, range, startOffset, endOffset, timezone, readTimeZone, endDateOverride)
		{
			base.Pattern.RecurrenceObjectType = RecurrenceObjectType.TaskRecurrence;
		}

		protected override bool GenerateTimeInDay
		{
			get
			{
				return false;
			}
		}

		internal ExDateTime GetNextOccurrence(ExDateTime start)
		{
			if (start == ExDateTime.MaxValue)
			{
				return ExDateTime.MaxValue;
			}
			OccurrenceInfo occurrenceInfo = new OccurrenceInfo(null, start.Date, start, ExDateTime.MaxValue, ExDateTime.MaxValue);
			if (base.Pattern is RegeneratingPattern)
			{
				return base.GetNthOccurrence(start, 2);
			}
			if (base.Range is NoEndRecurrenceRange || this.GetLastOccurrence().StartTime > start)
			{
				return this.GetNextOccurrence(occurrenceInfo).StartTime;
			}
			return ExDateTime.MaxValue;
		}

		internal override LocalizedString GenerateWhen(bool addTimeZoneInfo)
		{
			if (!(base.Pattern is RegeneratingPattern))
			{
				return base.GenerateWhen(addTimeZoneInfo);
			}
			ExDateTime exDateTime = this.ReadExTimeZone.ConvertDateTime(base.Range.StartDate);
			LocalizedString localizedString = ClientStrings.WhenRecurringNoEndDateNoTimeInDay(base.Pattern.When(), exDateTime);
			if (!(base.Range is NoEndRecurrenceRange))
			{
				if (!(base.Range is NumberedRecurrenceRange))
				{
					ExTraceGlobals.TaskTracer.TraceError<RecurrencePattern, RecurrenceRange>((long)this.GetHashCode(), "TaskRecurrence::GenerateWhen. The recurrence range is not allowed in this pattern. Pattern = {0}, Range = {1}.", base.Pattern, base.Range);
					throw new CorruptDataException(ServerStrings.TaskRecurrenceNotSupported(base.Pattern.ToString(), base.Range.ToString()));
				}
				localizedString = ClientStrings.JointStrings(localizedString, ClientStrings.WhenNMoreOccurrences(((NumberedRecurrenceRange)base.Range).NumberOfOccurrences));
			}
			return localizedString;
		}
	}
}
