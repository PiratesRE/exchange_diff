using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.SyncCalendar
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CalendarSyncState
	{
		public CalendarSyncState(string base64IcsSyncState, CalendarViewQueryResumptionPoint queryResumptionPoint, ExDateTime? oldWindowEndTime)
		{
			this.IcsSyncState = base64IcsSyncState;
			this.QueryResumptionPoint = queryResumptionPoint;
			this.OldWindowEnd = oldWindowEndTime;
		}

		public string IcsSyncState { get; protected set; }

		public CalendarViewQueryResumptionPoint QueryResumptionPoint { get; protected set; }

		public ExDateTime? OldWindowEnd { get; protected set; }

		public static bool IsEmpty(CalendarSyncState calendarSyncState)
		{
			return calendarSyncState.IcsSyncState == null && calendarSyncState.QueryResumptionPoint == null && calendarSyncState.OldWindowEnd == null;
		}

		public static ExDateTime? SafeGetDateTimeValue(string value)
		{
			long ticks;
			if (!string.IsNullOrEmpty(value) && long.TryParse(value, out ticks))
			{
				return new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, ticks));
			}
			return null;
		}

		public static CalendarViewQueryResumptionPoint SafeGetResumptionPoint(string version, string recurringPhase, string instanceKey, string sortKeyValue)
		{
			CalendarViewQueryResumptionPoint result = null;
			bool resumeToRecurringMeetings;
			if (version == CalendarViewQueryResumptionPoint.CurrentVersion && bool.TryParse(recurringPhase, out resumeToRecurringMeetings))
			{
				result = CalendarViewQueryResumptionPoint.CreateInstance(resumeToRecurringMeetings, CalendarSyncState.SafeGetInstanceKey(instanceKey), CalendarSyncState.SafeGetDateTimeValue(sortKeyValue));
			}
			return result;
		}

		public abstract IFolderSyncState CreateFolderSyncState(StoreObjectId folderObjectId, ISyncProvider syncProvider);

		public override string ToString()
		{
			string text;
			string text2;
			string text3;
			string text4;
			if (this.QueryResumptionPoint == null)
			{
				text = null;
				text2 = null;
				text3 = null;
				text4 = null;
			}
			else
			{
				text = this.QueryResumptionPoint.Version;
				text2 = this.QueryResumptionPoint.ResumeToRecurringMeetings.ToString();
				text3 = this.InstanceKeyToString(this.QueryResumptionPoint.InstanceKey);
				text4 = this.DateTimeToString(new ExDateTime?(this.QueryResumptionPoint.SortKeyValue));
			}
			string[] value = new string[]
			{
				"v2",
				this.IcsSyncState,
				text,
				text2,
				text3,
				text4,
				this.DateTimeToString(this.OldWindowEnd)
			};
			return string.Join(','.ToString(), value);
		}

		private static byte[] SafeGetInstanceKey(string value)
		{
			byte[] result = null;
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					result = Convert.FromBase64String(value);
				}
				catch (FormatException ex)
				{
					ExTraceGlobals.SyncCalendarCallTracer.TraceDebug<string, string>(0L, "CalendarSyncState.SafeGetInstanceKey trying to parse invalid instance key ({0}). Error: {1}", value, ex.Message);
				}
			}
			return result;
		}

		private string InstanceKeyToString(byte[] instanceKey)
		{
			return Convert.ToBase64String(instanceKey ?? Array<byte>.Empty);
		}

		private string DateTimeToString(ExDateTime? dateTime)
		{
			if (dateTime == null)
			{
				return string.Empty;
			}
			return dateTime.Value.UtcTicks.ToString(CultureInfo.InvariantCulture);
		}

		public const string CurrentVersion = "v2";

		public const char TokenSeparator = ',';
	}
}
