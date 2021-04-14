using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Sharing;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Common;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal sealed class AppointmentTranslator
	{
		public AppointmentTranslator(CultureInfo clientCulture)
		{
			this.ClientCulture = clientCulture;
		}

		private CultureInfo ClientCulture { get; set; }

		public void CreateAppointment(ExchangeService exchangeService, MailboxSession session, CalendarItemType remoteItem, LocalFolder localFolder)
		{
			AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string>((long)this.GetHashCode(), "{0}: Creating appointment {1}", this, remoteItem.ItemId.Id);
			CalendarItem calendarItem = CalendarItem.Create(session, localFolder.Id);
			bool flag = false;
			try
			{
				this.UpdateAppointment(exchangeService, session, remoteItem, calendarItem);
				flag = true;
			}
			catch (LastOccurrenceDeletionException arg)
			{
				AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string, LastOccurrenceDeletionException>((long)this.GetHashCode(), "{0}: All occurrences in the series were deleted - {1}, Error - {2}, We will delete the local item because there are no instances.", this, remoteItem.ItemId.Id, arg);
				localFolder.SelectItemToDelete(calendarItem.Id.ObjectId);
				flag = true;
			}
			finally
			{
				if (!flag && calendarItem.Id != null)
				{
					localFolder.SelectItemToDelete(calendarItem.Id.ObjectId);
				}
				calendarItem.Dispose();
			}
		}

		public void UpdateAppointment(ExchangeService exchangeService, MailboxSession session, CalendarItemType remoteItem, CalendarItem localItem)
		{
			AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string>((long)this.GetHashCode(), "{0}: Updating appointment {1}", this, remoteItem.ItemId.Id);
			this.CopyCalendarProperties(localItem, remoteItem);
			localItem.Recurrence = null;
			if (remoteItem.Recurrence != null)
			{
				localItem.ExTimeZone = localItem.StartTimeZone;
				localItem.Recurrence = this.ConvertRecurrence(localItem.StartTimeZone, localItem.EndTimeZone, remoteItem.Recurrence);
			}
			AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string>((long)this.GetHashCode(), "{0}: Saving local item {1}", this, remoteItem.ItemId.Id);
			localItem.Save(SaveMode.NoConflictResolution);
			localItem.Load();
			if (remoteItem.Recurrence != null)
			{
				this.CopyExceptions(exchangeService, localItem, remoteItem);
			}
		}

		public string ConvertFreeBusyToTitle(BusyType busyType)
		{
			switch (busyType)
			{
			case BusyType.Free:
				return ClientStrings.Free.ToString(this.ClientCulture);
			case BusyType.Tentative:
				return ClientStrings.Tentative.ToString(this.ClientCulture);
			case BusyType.Busy:
				return ClientStrings.Busy.ToString(this.ClientCulture);
			case BusyType.OOF:
				return ClientStrings.OOF.ToString(this.ClientCulture);
			default:
				return string.Empty;
			}
		}

		private void CopyCalendarProperties(CalendarItemBase localItem, CalendarItemType remoteItem)
		{
			ExTimeZone desiredTimeZone = ExTimeZone.UtcTimeZone;
			ExTimeZone desiredTimeZone2 = ExTimeZone.UtcTimeZone;
			if (remoteItem.StartTimeZone != null)
			{
				AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string, string>((long)this.GetHashCode(), "{0}: Converting StartTimeZone - Id: {1}, Name: {2}", this, remoteItem.StartTimeZone.Id, remoteItem.StartTimeZone.Name);
				desiredTimeZone = new TimeZoneDefinitionAdaptor(remoteItem.StartTimeZone).ExTimeZone;
			}
			if (remoteItem.EndTimeZone != null)
			{
				AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string, string>((long)this.GetHashCode(), "{0}: Converting EndTimeZone - Id: {1}, Name: {2}", this, remoteItem.EndTimeZone.Id, remoteItem.EndTimeZone.Name);
				desiredTimeZone2 = new TimeZoneDefinitionAdaptor(remoteItem.EndTimeZone).ExTimeZone;
			}
			AppointmentTranslator.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: Copying Subject: {1}, Location: {2}, StartTime: {3}, EndTime: {4}", new object[]
			{
				this,
				remoteItem.Subject,
				remoteItem.Location,
				remoteItem.Start,
				remoteItem.End
			});
			localItem.StartTime = new ExDateTime(desiredTimeZone, this.NormalizeDateToUtc(remoteItem.Start));
			localItem.EndTime = new ExDateTime(desiredTimeZone2, this.NormalizeDateToUtc(remoteItem.End));
			localItem.Sensitivity = this.Convert(remoteItem.Sensitivity);
			localItem.FreeBusyStatus = this.Convert(remoteItem.LegacyFreeBusyStatus);
			if (remoteItem.Location != null)
			{
				localItem.Location = remoteItem.Location;
			}
			else
			{
				localItem.Location = string.Empty;
			}
			if (localItem.Sensitivity != Sensitivity.Normal)
			{
				localItem.Subject = ClientStrings.PrivateAppointmentSubject.ToString(this.ClientCulture);
			}
			else if (remoteItem.Subject != null)
			{
				localItem.Subject = remoteItem.Subject;
			}
			else
			{
				localItem.Subject = this.ConvertFreeBusyToTitle(localItem.FreeBusyStatus);
			}
			localItem.Reminder.Disable();
			string value = string.Empty;
			BodyFormat bodyFormat = BodyFormat.TextPlain;
			if (remoteItem.Body != null)
			{
				value = remoteItem.Body.Value;
				switch (remoteItem.Body.BodyType1)
				{
				case BodyTypeType.HTML:
					bodyFormat = BodyFormat.TextHtml;
					break;
				case BodyTypeType.Text:
					bodyFormat = BodyFormat.TextPlain;
					break;
				default:
					throw new InvalidAppointmentException();
				}
				AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string>((long)this.GetHashCode(), "{0}: Copying body: {1}", this, remoteItem.Body.Value);
			}
			using (TextWriter textWriter = localItem.Body.OpenTextWriter(bodyFormat))
			{
				textWriter.Write(value);
			}
			AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string>((long)this.GetHashCode(), "{0}: Copying Id: {1}", this, remoteItem.ItemId.Id);
			localItem[SharingSchema.ExternalSharingMasterId] = remoteItem.ItemId.Id;
		}

		private void CopyExceptions(ExchangeService exchangeService, CalendarItem localItem, CalendarItemType remoteItem)
		{
			bool flag = false;
			if (remoteItem.DeletedOccurrences != null)
			{
				foreach (DeletedOccurrenceInfoType deletedOccurrenceInfoType in remoteItem.DeletedOccurrences)
				{
					ExDateTime exDateTime = (ExDateTime)this.NormalizeDateToUtc(deletedOccurrenceInfoType.Start);
					ExDateTime date = localItem.Recurrence.CreatedExTimeZone.ConvertDateTime(exDateTime).Date;
					if (!localItem.Recurrence.IsOccurrenceDeleted(date) && localItem.Recurrence.IsValidOccurrenceId(date))
					{
						AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, DateTime>((long)this.GetHashCode(), "{0}: Copying deleted exception: {1}", this, deletedOccurrenceInfoType.Start);
						localItem.DeleteOccurrenceByOriginalStartTime(exDateTime);
						flag = true;
					}
					else
					{
						AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, DateTime>((long)this.GetHashCode(), "{0}: Exception {1} is already deleted on the local copy or is invalid. Skipping delete.", this, deletedOccurrenceInfoType.Start);
					}
				}
			}
			new Dictionary<string, OccurrenceInfoType>();
			if (remoteItem.ModifiedOccurrences != null)
			{
				List<ItemIdType> list = new List<ItemIdType>(128);
				Dictionary<string, OccurrenceInfoType> dictionary = new Dictionary<string, OccurrenceInfoType>();
				foreach (OccurrenceInfoType occurrenceInfoType in remoteItem.ModifiedOccurrences)
				{
					AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string>((long)this.GetHashCode(), "{0}: Fetching modified exception: {1}", this, occurrenceInfoType.ItemId.Id);
					dictionary.Add(occurrenceInfoType.ItemId.Id, occurrenceInfoType);
					list.Add(occurrenceInfoType.ItemId);
					if (list.Count >= 128)
					{
						flag |= this.HandleModifiedExceptions(exchangeService, list.ToArray(), dictionary, localItem);
						list.Clear();
						dictionary.Clear();
					}
				}
				if (list.Count > 0)
				{
					flag |= this.HandleModifiedExceptions(exchangeService, list.ToArray(), dictionary, localItem);
				}
			}
			if (flag)
			{
				AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator>((long)this.GetHashCode(), "{0}: Saving exceptions to master", this);
				localItem.Save(SaveMode.NoConflictResolution);
			}
		}

		private bool HandleModifiedExceptions(ExchangeService exchangeService, ItemIdType[] itemsToGet, Dictionary<string, OccurrenceInfoType> modifiedOccurrencesTable, CalendarItem localItem)
		{
			IEnumerable<ItemType> item;
			try
			{
				item = exchangeService.GetItem(itemsToGet, StoreObjectType.CalendarFolder);
			}
			catch (UnexpectedRemoteDataException ex)
			{
				AppointmentTranslator.Tracer.TraceError<AppointmentTranslator, UnexpectedRemoteDataException>((long)this.GetHashCode(), "{0}: Failed to get any exception item. Exception = {1}.", this, ex);
				if (ex.Items == null)
				{
					throw;
				}
				bool flag = false;
				int num = 0;
				foreach (ResponseMessageType responseMessageType in ex.Items)
				{
					if (responseMessageType == null)
					{
						throw;
					}
					if (this.IsReponseErrorRetryable(responseMessageType.ResponseCode))
					{
						flag = true;
						break;
					}
					if (responseMessageType.ResponseCode == ResponseCodeType.ErrorInvalidOperation)
					{
						num++;
					}
					else if (responseMessageType.ResponseCode != ResponseCodeType.ErrorCorruptData && responseMessageType.ResponseCode != ResponseCodeType.ErrorItemNotFound)
					{
						throw;
					}
				}
				if (flag)
				{
					throw new FailedCommunicationException(ex);
				}
				if (num > 0 && num != ex.Items.Length)
				{
					throw;
				}
				return false;
			}
			bool result = false;
			foreach (ItemType itemType in item)
			{
				CalendarItemType calendarItemType = itemType as CalendarItemType;
				OccurrenceInfoType occurrenceInfoType;
				if (calendarItemType == null)
				{
					AppointmentTranslator.Tracer.TraceError<AppointmentTranslator>((long)this.GetHashCode(), "{0}: Non-appointment exception item", this);
				}
				else if (!modifiedOccurrencesTable.TryGetValue(calendarItemType.ItemId.Id, out occurrenceInfoType))
				{
					AppointmentTranslator.Tracer.TraceError<AppointmentTranslator, string>((long)this.GetHashCode(), "{0}: Unexpected exception item: {1}", this, calendarItemType.ItemId.Id);
				}
				else
				{
					ExDateTime exDateTime = (ExDateTime)this.NormalizeDateToUtc(occurrenceInfoType.OriginalStart);
					AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string, ExDateTime>((long)this.GetHashCode(), "{0}: Processing exception {1} originalStartTime {2}", this, calendarItemType.ItemId.Id, exDateTime);
					try
					{
						using (CalendarItemOccurrence calendarItemOccurrence = localItem.OpenOccurrenceByOriginalStartTime(exDateTime, new PropertyDefinition[0]))
						{
							AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string, ExDateTime>((long)this.GetHashCode(), "{0}: Copying exception properties {1} originalStartTime {2}", this, calendarItemType.ItemId.Id, exDateTime);
							this.CopyCalendarProperties(calendarItemOccurrence, calendarItemType);
							AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, string, ExDateTime>((long)this.GetHashCode(), "{0}: Saving exception item {1} originalStartTime {2}", this, calendarItemType.ItemId.Id, exDateTime);
							calendarItemOccurrence.Save(SaveMode.NoConflictResolution);
							result = true;
						}
					}
					catch (CorruptDataException arg)
					{
						AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, ExDateTime, CorruptDataException>((long)this.GetHashCode(), "{0}: Got a corrupt data exception during Save for item with originalStartTime {1}, Exception = {2}", this, exDateTime, arg);
					}
					catch (VirusException arg2)
					{
						AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, ExDateTime, VirusException>((long)this.GetHashCode(), "{0}:  Got a virus detected exception during Save for item with originalStartTime {1}, Exception = {2}", this, exDateTime, arg2);
					}
					catch (RecurrenceException arg3)
					{
						AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, ExDateTime, RecurrenceException>((long)this.GetHashCode(), "{0}: Got a recurrence exception during Save for item with originalStartTime {1}, Exception = {2}", this, exDateTime, arg3);
					}
					catch (OccurrenceNotFoundException arg4)
					{
						AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, ExDateTime, OccurrenceNotFoundException>((long)this.GetHashCode(), "{0}: Got an OccurrenceNotFoundException exception during Save for item with originalStartTime {1}, Exception = {2}", this, exDateTime, arg4);
					}
				}
			}
			return result;
		}

		private Recurrence ConvertRecurrence(ExTimeZone startTimeZone, ExTimeZone endTimeZone, RecurrenceType recurrence)
		{
			AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, RecurrenceType>((long)this.GetHashCode(), "{0}: Converting recurrence {1}", this, recurrence);
			RecurrencePattern recurrencePattern = this.ConvertRecurrencePattern(recurrence.Item);
			RecurrenceRange recurrenceRange = this.ConvertRecurrenceRange(startTimeZone, endTimeZone, recurrence.Item1);
			AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, RecurrencePattern, RecurrenceRange>((long)this.GetHashCode(), "{0}: Recurrence pattern/range: {1}/{2}", this, recurrencePattern, recurrenceRange);
			return new Recurrence(recurrencePattern, recurrenceRange);
		}

		private RecurrenceRange ConvertRecurrenceRange(ExTimeZone startTimeZone, ExTimeZone endTimeZone, RecurrenceRangeBaseType range)
		{
			ExDateTime startDate = new ExDateTime(startTimeZone, this.NormalizeDateToUtc(range.StartDate));
			NumberedRecurrenceRangeType numberedRecurrenceRangeType = range as NumberedRecurrenceRangeType;
			if (numberedRecurrenceRangeType != null)
			{
				return new NumberedRecurrenceRange(startDate, numberedRecurrenceRangeType.NumberOfOccurrences);
			}
			EndDateRecurrenceRangeType endDateRecurrenceRangeType = range as EndDateRecurrenceRangeType;
			if (endDateRecurrenceRangeType != null)
			{
				ExDateTime exDateTime = new ExDateTime(startTimeZone, this.NormalizeDateToUtc(endDateRecurrenceRangeType.EndDate));
				if (exDateTime > Recurrence.MaximumDateForRecurrenceEnd)
				{
					return new NoEndRecurrenceRange(startDate);
				}
				return new EndDateRecurrenceRange(startDate, exDateTime);
			}
			else
			{
				NoEndRecurrenceRangeType noEndRecurrenceRangeType = range as NoEndRecurrenceRangeType;
				if (noEndRecurrenceRangeType != null)
				{
					return new NoEndRecurrenceRange(startDate);
				}
				AppointmentTranslator.Tracer.TraceError<AppointmentTranslator, RecurrenceRangeBaseType>((long)this.GetHashCode(), "{0}: Unknown recurrence range: {1}", this, range);
				throw new InvalidAppointmentException();
			}
		}

		private DateTime NormalizeDateToUtc(DateTime dateTime)
		{
			DateTime dateTime2 = (dateTime.Kind == DateTimeKind.Local) ? dateTime.ToUniversalTime() : dateTime;
			AppointmentTranslator.Tracer.TraceDebug<AppointmentTranslator, DateTime, DateTime>((long)this.GetHashCode(), "{0}: Normalizing DateTime value. Before: {1}, After: {2}", this, dateTime, dateTime2);
			return dateTime2;
		}

		private RecurrencePattern ConvertRecurrencePattern(RecurrencePatternBaseType pattern)
		{
			DailyRecurrencePatternType dailyRecurrencePatternType = pattern as DailyRecurrencePatternType;
			if (dailyRecurrencePatternType != null)
			{
				return new DailyRecurrencePattern(dailyRecurrencePatternType.Interval);
			}
			AbsoluteMonthlyRecurrencePatternType absoluteMonthlyRecurrencePatternType = pattern as AbsoluteMonthlyRecurrencePatternType;
			if (absoluteMonthlyRecurrencePatternType != null)
			{
				return new MonthlyRecurrencePattern(absoluteMonthlyRecurrencePatternType.DayOfMonth, absoluteMonthlyRecurrencePatternType.Interval);
			}
			RelativeMonthlyRecurrencePatternType relativeMonthlyRecurrencePatternType = pattern as RelativeMonthlyRecurrencePatternType;
			if (relativeMonthlyRecurrencePatternType != null)
			{
				return new MonthlyThRecurrencePattern(this.ConvertDaysOfWeek(relativeMonthlyRecurrencePatternType.DaysOfWeek), this.Convert(relativeMonthlyRecurrencePatternType.DayOfWeekIndex), relativeMonthlyRecurrencePatternType.Interval);
			}
			RelativeYearlyRecurrencePatternType relativeYearlyRecurrencePatternType = pattern as RelativeYearlyRecurrencePatternType;
			if (relativeYearlyRecurrencePatternType != null)
			{
				return new YearlyThRecurrencePattern(this.ConvertDaysOfWeek(relativeYearlyRecurrencePatternType.DaysOfWeek), this.Convert(relativeYearlyRecurrencePatternType.DayOfWeekIndex), this.ConvertMonth(relativeYearlyRecurrencePatternType.Month));
			}
			WeeklyRecurrencePatternType weeklyRecurrencePatternType = pattern as WeeklyRecurrencePatternType;
			if (weeklyRecurrencePatternType != null)
			{
				return new WeeklyRecurrencePattern(this.ConvertDaysOfWeek(weeklyRecurrencePatternType.DaysOfWeek), weeklyRecurrencePatternType.Interval);
			}
			AbsoluteYearlyRecurrencePatternType absoluteYearlyRecurrencePatternType = pattern as AbsoluteYearlyRecurrencePatternType;
			if (absoluteYearlyRecurrencePatternType != null)
			{
				return new YearlyRecurrencePattern(absoluteYearlyRecurrencePatternType.DayOfMonth, this.ConvertMonth(absoluteYearlyRecurrencePatternType.Month));
			}
			AppointmentTranslator.Tracer.TraceError<AppointmentTranslator, RecurrencePatternBaseType>((long)this.GetHashCode(), "{0}: Unknown recurrence pattern: {1}", this, pattern);
			throw new InvalidAppointmentException();
		}

		private DaysOfWeek ConvertDaysOfWeek(DayOfWeekType dayOfWeek)
		{
			switch (dayOfWeek)
			{
			case DayOfWeekType.Sunday:
				return DaysOfWeek.Sunday;
			case DayOfWeekType.Monday:
				return DaysOfWeek.Monday;
			case DayOfWeekType.Tuesday:
				return DaysOfWeek.Tuesday;
			case DayOfWeekType.Wednesday:
				return DaysOfWeek.Wednesday;
			case DayOfWeekType.Thursday:
				return DaysOfWeek.Thursday;
			case DayOfWeekType.Friday:
				return DaysOfWeek.Friday;
			case DayOfWeekType.Saturday:
				return DaysOfWeek.Saturday;
			case DayOfWeekType.Day:
				return DaysOfWeek.AllDays;
			case DayOfWeekType.Weekday:
				return DaysOfWeek.Weekdays;
			case DayOfWeekType.WeekendDay:
				return DaysOfWeek.WeekendDays;
			default:
				AppointmentTranslator.Tracer.TraceError((long)this.GetHashCode(), "{0}: Unknown DayOfWeekType. Name: {1}, Value: {2}. Acceptable Range {3}-{4}.", new object[]
				{
					this,
					dayOfWeek,
					(int)dayOfWeek,
					0,
					9
				});
				throw new InvalidAppointmentException();
			}
		}

		private DaysOfWeek ConvertDaysOfWeek(string daysOfWeekString)
		{
			string[] array = daysOfWeekString.ToLowerInvariant().Split(new char[]
			{
				' '
			});
			var array2 = new <>f__AnonymousType6<string, DaysOfWeek>[]
			{
				new
				{
					Value = "sunday",
					MapsTo = DaysOfWeek.Sunday
				},
				new
				{
					Value = "monday",
					MapsTo = DaysOfWeek.Monday
				},
				new
				{
					Value = "tuesday",
					MapsTo = DaysOfWeek.Tuesday
				},
				new
				{
					Value = "wednesday",
					MapsTo = DaysOfWeek.Wednesday
				},
				new
				{
					Value = "thursday",
					MapsTo = DaysOfWeek.Thursday
				},
				new
				{
					Value = "friday",
					MapsTo = DaysOfWeek.Friday
				},
				new
				{
					Value = "saturday",
					MapsTo = DaysOfWeek.Saturday
				}
			};
			DaysOfWeek daysOfWeek = DaysOfWeek.None;
			var array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				var <>f__AnonymousType = array3[i];
				if (Array.IndexOf<string>(array, <>f__AnonymousType.Value) != -1)
				{
					daysOfWeek |= <>f__AnonymousType.MapsTo;
				}
			}
			return daysOfWeek;
		}

		private RecurrenceOrderType Convert(DayOfWeekIndexType index)
		{
			switch (index)
			{
			case DayOfWeekIndexType.First:
				return RecurrenceOrderType.First;
			case DayOfWeekIndexType.Second:
				return RecurrenceOrderType.Second;
			case DayOfWeekIndexType.Third:
				return RecurrenceOrderType.Third;
			case DayOfWeekIndexType.Fourth:
				return RecurrenceOrderType.Fourth;
			case DayOfWeekIndexType.Last:
				return RecurrenceOrderType.Last;
			default:
				AppointmentTranslator.Tracer.TraceError((long)this.GetHashCode(), "{0}: Unknown DayOfWeekIndexType. Name: {1}, Value: {2}. Acceptable Range {3}-{4}.", new object[]
				{
					this,
					index,
					(int)index,
					0,
					4
				});
				throw new InvalidAppointmentException();
			}
		}

		private int ConvertMonth(MonthNamesType month)
		{
			switch (month)
			{
			case MonthNamesType.January:
				return 1;
			case MonthNamesType.February:
				return 2;
			case MonthNamesType.March:
				return 3;
			case MonthNamesType.April:
				return 4;
			case MonthNamesType.May:
				return 5;
			case MonthNamesType.June:
				return 6;
			case MonthNamesType.July:
				return 7;
			case MonthNamesType.August:
				return 8;
			case MonthNamesType.September:
				return 9;
			case MonthNamesType.October:
				return 10;
			case MonthNamesType.November:
				return 11;
			case MonthNamesType.December:
				return 12;
			default:
				AppointmentTranslator.Tracer.TraceError((long)this.GetHashCode(), "{0}: Unknown MonthNamesType. Name: {1}, Value: {2}. Acceptable Range {3}-{4}.", new object[]
				{
					this,
					month,
					(int)month,
					0,
					11
				});
				throw new InvalidAppointmentException();
			}
		}

		private BusyType Convert(LegacyFreeBusyType freeBusyType)
		{
			switch (freeBusyType)
			{
			case LegacyFreeBusyType.Free:
				return BusyType.Free;
			case LegacyFreeBusyType.Tentative:
				return BusyType.Tentative;
			case LegacyFreeBusyType.Busy:
				return BusyType.Busy;
			case LegacyFreeBusyType.OOF:
				return BusyType.OOF;
			default:
				return BusyType.Unknown;
			}
		}

		private Sensitivity Convert(SensitivityChoicesType sensitivity)
		{
			switch (sensitivity)
			{
			case SensitivityChoicesType.Normal:
				return Sensitivity.Normal;
			case SensitivityChoicesType.Personal:
				return Sensitivity.Personal;
			case SensitivityChoicesType.Private:
				return Sensitivity.Private;
			case SensitivityChoicesType.Confidential:
				return Sensitivity.CompanyConfidential;
			default:
				AppointmentTranslator.Tracer.TraceError((long)this.GetHashCode(), "{0}: Unknown SensitivityChoicesType. Name: {1}, Value: {2}. Acceptable Range {3}-{4}.", new object[]
				{
					this,
					sensitivity,
					(int)sensitivity,
					0,
					3
				});
				throw new InvalidAppointmentException();
			}
		}

		private bool IsReponseErrorRetryable(ResponseCodeType errorCode)
		{
			if (errorCode != ResponseCodeType.ErrorConnectionFailed)
			{
				switch (errorCode)
				{
				case ResponseCodeType.ErrorMailboxMoveInProgress:
				case ResponseCodeType.ErrorMailboxStoreUnavailable:
					break;
				default:
					if (errorCode != ResponseCodeType.ErrorTimeoutExpired)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}

		private static readonly Trace Tracer = ExTraceGlobals.AppointmentTranslatorTracer;
	}
}
