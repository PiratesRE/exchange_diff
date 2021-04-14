using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetUserAvailabilityInternal
	{
		public GetUserAvailabilityInternal(MailboxSession mailboxSession, GetUserAvailabilityRequest request, GetUserAvailabilityResponse response)
		{
			this.request = request;
			this.response = response;
			this.mailboxSession = mailboxSession;
			OwsLogRegistry.Register(GetUserAvailabilityInternal.GetUserAvailabilityActionName, typeof(AvailabilityServiceMetadata), new Type[0]);
		}

		public GetUserAvailabilityInternalJsonResponse Execute()
		{
			GetUserAvailabilityInternalResponse getUserAvailabilityInternalResponse = new GetUserAvailabilityInternalResponse();
			getUserAvailabilityInternalResponse.Responses = new UserAvailabilityInternalResponse[this.response.FreeBusyResponseArray.Length];
			for (int i = 0; i < this.response.FreeBusyResponseArray.Length; i++)
			{
				FreeBusyResponse freeBusyResponse = this.response.FreeBusyResponseArray[i];
				UserAvailabilityInternalResponse userAvailabilityInternalResponse = new UserAvailabilityInternalResponse();
				userAvailabilityInternalResponse.ResponseMessage = freeBusyResponse.ResponseMessage;
				getUserAvailabilityInternalResponse.Responses[i] = userAvailabilityInternalResponse;
				string smtpAddressFromMailboxData = GetUserAvailabilityInternal.GetSmtpAddressFromMailboxData(this.request.MailboxDataArray[i]);
				if (userAvailabilityInternalResponse.ResponseMessage.ResponseClass != ResponseClass.Success)
				{
					getUserAvailabilityInternalResponse.Responses[i].CalendarView = new UserAvailabilityCalendarView();
					getUserAvailabilityInternalResponse.Responses[i].CalendarView.FreeBusyViewType = FreeBusyViewType.None.ToString();
					ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug<MailboxData, string>((long)this.GetHashCode(), "Failed to get freebusy response for user:{0}. Message:{1}", this.request.MailboxDataArray[i], userAvailabilityInternalResponse.ResponseMessage.MessageText);
				}
				else
				{
					FolderId parentFolderId = new FolderId(smtpAddressFromMailboxData, string.Empty);
					if (freeBusyResponse.FreeBusyView.MergedFreeBusy != null)
					{
						freeBusyResponse.FreeBusyView.MergedFreeBusy = this.UpdateMergedFreeBusy(freeBusyResponse.FreeBusyView.MergedFreeBusy);
					}
					userAvailabilityInternalResponse.CalendarView = this.ConvertFreeBusyView(freeBusyResponse.FreeBusyView, parentFolderId);
				}
			}
			return new GetUserAvailabilityInternalJsonResponse
			{
				Body = getUserAvailabilityInternalResponse
			};
		}

		private static string GetSmtpAddressFromMailboxData(MailboxData mailboxData)
		{
			if (mailboxData == null || mailboxData.Email == null || mailboxData.Email.Address == null || !SmtpAddress.IsValidSmtpAddress(mailboxData.Email.Address))
			{
				return null;
			}
			return mailboxData.Email.Address;
		}

		private string UpdateMergedFreeBusy(string mergedFreeBusy)
		{
			string result = mergedFreeBusy;
			if (mergedFreeBusy == null)
			{
				ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug((long)this.GetHashCode(), "No MergeFreeBusy data was returned.");
				return null;
			}
			this.SetFreeBusyDayLightBasedValue(new ExDateTime(EWSSettings.RequestTimeZone, this.request.FreeBusyViewOptions.TimeWindow.StartTime), new ExDateTime(EWSSettings.RequestTimeZone, this.request.FreeBusyViewOptions.TimeWindow.EndTime), EWSSettings.RequestTimeZone, this.request.FreeBusyViewOptions.MergedFreeBusyIntervalInMinutes, ref result);
			return result;
		}

		private UserAvailabilityCalendarView ConvertFreeBusyView(FreeBusyView freeBusyView, FolderId parentFolderId)
		{
			UserAvailabilityCalendarView userAvailabilityCalendarView = new UserAvailabilityCalendarView();
			userAvailabilityCalendarView.WorkingHours = this.ConvertWorkingHours(freeBusyView.WorkingHours);
			userAvailabilityCalendarView.MergedFreeBusy = freeBusyView.MergedFreeBusy;
			userAvailabilityCalendarView.FreeBusyViewType = freeBusyView.FreeBusyViewTypeString;
			if (freeBusyView.CalendarEventArray == null)
			{
				userAvailabilityCalendarView.Items = new EwsCalendarItemType[0];
			}
			else
			{
				userAvailabilityCalendarView.Items = this.ConvertCalendarEvents(freeBusyView.CalendarEventArray, parentFolderId);
			}
			return userAvailabilityCalendarView;
		}

		private WorkingHoursType ConvertWorkingHours(WorkingHours freeBusyWorkingHours)
		{
			if (freeBusyWorkingHours == null)
			{
				return new WorkingHoursType(0, 0, 0, this.mailboxSession.ExTimeZone, this.mailboxSession.ExTimeZone);
			}
			return new WorkingHoursType(freeBusyWorkingHours.StartTimeInMinutes, freeBusyWorkingHours.EndTimeInMinutes, (int)freeBusyWorkingHours.DaysOfWeek, this.mailboxSession.ExTimeZone, freeBusyWorkingHours.ExTimeZone);
		}

		private EwsCalendarItemType[] ConvertCalendarEvents(CalendarEvent[] calendarEvents, FolderId parentFolderId)
		{
			EwsCalendarItemType[] array = new EwsCalendarItemType[calendarEvents.Length];
			for (int i = 0; i < calendarEvents.Length; i++)
			{
				CalendarEvent calendarEvent = calendarEvents[i];
				EwsCalendarItemType ewsCalendarItemType = array[i] = new EwsCalendarItemType();
				ewsCalendarItemType.LegacyFreeBusyStatus = calendarEvent.BusyType;
				ewsCalendarItemType.Start = calendarEvent.StartTimeString;
				ewsCalendarItemType.End = calendarEvent.EndTimeString;
				ewsCalendarItemType.ParentFolderId = parentFolderId;
				ewsCalendarItemType.EffectiveRights = new EffectiveRightsType();
				if (calendarEvent.GlobalObjectId != null)
				{
					ewsCalendarItemType.UID = new GlobalObjectId(calendarEvent.GlobalObjectId).Uid;
				}
				ewsCalendarItemType.ItemId = new Microsoft.Exchange.Services.Core.Types.ItemId(Guid.NewGuid().ToString(), string.Empty);
				if (calendarEvent.CalendarEventDetails == null)
				{
					ewsCalendarItemType.Subject = this.ConvertFreeBusyStatusToSubject(calendarEvent.BusyType);
				}
				else
				{
					CalendarEventDetails calendarEventDetails = calendarEvent.CalendarEventDetails;
					if (calendarEventDetails.IsPrivate)
					{
						ewsCalendarItemType.Sensitivity = SensitivityType.Private;
						if (!ewsCalendarItemType.EffectiveRights.Read)
						{
							ewsCalendarItemType.Subject = ClientStrings.PrivateAppointmentSubject.ToString(this.mailboxSession.Culture);
							goto IL_1E1;
						}
					}
					else
					{
						ewsCalendarItemType.Sensitivity = SensitivityType.Normal;
					}
					ewsCalendarItemType.Subject = calendarEventDetails.Subject;
					ewsCalendarItemType.EnhancedLocation = new EnhancedLocationType
					{
						DisplayName = calendarEventDetails.Location
					};
					ewsCalendarItemType.IsMeeting = new bool?(calendarEventDetails.IsMeeting);
					ewsCalendarItemType.ReminderIsSet = new bool?(calendarEventDetails.IsReminderSet);
					ewsCalendarItemType.IsAllDayEvent = new bool?(calendarEvent.StartTime.TimeOfDay.TotalSeconds == 0.0 && calendarEvent.EndTime.TimeOfDay.TotalSeconds == 0.0 && calendarEvent.StartTime < calendarEvent.EndTime);
					if (calendarEventDetails.IsRecurring)
					{
						ewsCalendarItemType.CalendarItemType = (calendarEventDetails.IsException ? CalendarItemTypeType.Exception : CalendarItemTypeType.Occurrence);
					}
					else
					{
						ewsCalendarItemType.CalendarItemType = CalendarItemTypeType.Single;
					}
				}
				IL_1E1:;
			}
			return array;
		}

		private string ConvertFreeBusyStatusToSubject(Microsoft.Exchange.InfoWorker.Common.Availability.BusyType busyType)
		{
			switch (busyType)
			{
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Free:
				return ClientStrings.Free.ToString(this.mailboxSession.Culture);
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Tentative:
				return ClientStrings.Tentative.ToString(this.mailboxSession.Culture);
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Busy:
				return ClientStrings.Busy.ToString(this.mailboxSession.Culture);
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.OOF:
				return ClientStrings.OOF.ToString(this.mailboxSession.Culture);
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.WorkingElsewhere:
				return ClientStrings.WorkingElsewhere.ToString(this.mailboxSession.Culture);
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.NoData:
				return ClientStrings.NoDataAvailable.ToString(this.mailboxSession.Culture);
			default:
				ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug((long)this.GetHashCode(), "Unable to convert FreeBusy status to String. Returning empty string.");
				return string.Empty;
			}
		}

		private void SetFreeBusyDayLightBasedValue(ExDateTime startDate, ExDateTime endDate, ExTimeZone timeZone, int intervalInMinutes, ref string freeBusyData)
		{
			if (string.IsNullOrEmpty(freeBusyData))
			{
				throw new ArgumentNullException("freeBusyData", "FreeBusyData cannot be null or Empty");
			}
			ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug<string>((long)this.GetHashCode(), "GetUserAvailabilityInternal.SetFreeBusyDayLightBasedValue - Original MergedFreeBusy: {0}", freeBusyData);
			GetUserAvailabilityInternal.DayLightsTransition startToEndDayLightsTransitionValue = this.GetStartToEndDayLightsTransitionValue(startDate, endDate, timeZone);
			if (startToEndDayLightsTransitionValue == GetUserAvailabilityInternal.DayLightsTransition.NoTransition)
			{
				return;
			}
			ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug<GetUserAvailabilityInternal.DayLightsTransition>((long)this.GetHashCode(), "SetFreeBusyDayLightBasedValue - TransitionType: {0}.", startToEndDayLightsTransitionValue);
			int num;
			int num2;
			this.SetDayLightsSavingIndices(startDate, timeZone, startToEndDayLightsTransitionValue, intervalInMinutes, out num, out num2);
			if (startToEndDayLightsTransitionValue == GetUserAvailabilityInternal.DayLightsTransition.TransitionFromStandardToDayLights)
			{
				StringBuilder stringBuilder = new StringBuilder(freeBusyData.Length);
				stringBuilder.Append(freeBusyData.Substring(0, num));
				for (int i = 0; i < num2; i++)
				{
					stringBuilder.Append('0');
				}
				stringBuilder.Append(freeBusyData.Substring(num));
				freeBusyData = stringBuilder.ToString();
			}
			else if (startToEndDayLightsTransitionValue == GetUserAvailabilityInternal.DayLightsTransition.TransitionFromDayLightsToStandard)
			{
				StringBuilder stringBuilder2 = new StringBuilder(freeBusyData.Length);
				stringBuilder2.Append(freeBusyData.Substring(0, num));
				stringBuilder2.Append(freeBusyData.Substring(num + num2));
				freeBusyData = stringBuilder2.ToString();
			}
			ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SetFreeBusyDayLightBasedValue - New MergedFreeBusy: {0}.", freeBusyData);
		}

		private void SetDayLightsSavingIndices(ExDateTime startDateFreeBusyWindow, ExTimeZone timeZone, GetUserAvailabilityInternal.DayLightsTransition transition, int intervalInMinutes, out int transitionIndex, out int dayLightIndexSpan)
		{
			ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug((long)this.GetHashCode(), "GetUserAvailabilityInternal.SetDayLightsSavingIndices");
			transitionIndex = 0;
			dayLightIndexSpan = 0;
			if (transition != GetUserAvailabilityInternal.DayLightsTransition.NoTransition)
			{
				DaylightTime daylightChanges = timeZone.GetDaylightChanges(startDateFreeBusyWindow.Year);
				TimeSpan timeSpan;
				if ((daylightChanges.Start < daylightChanges.End && startDateFreeBusyWindow > (ExDateTime)daylightChanges.Start) || (daylightChanges.Start > daylightChanges.End && startDateFreeBusyWindow < (ExDateTime)daylightChanges.End))
				{
					timeSpan = (ExDateTime)daylightChanges.End - startDateFreeBusyWindow;
				}
				else
				{
					timeSpan = (ExDateTime)daylightChanges.Start - startDateFreeBusyWindow;
				}
				transitionIndex = (timeSpan.Days * 60 * 24 + timeSpan.Hours * 60 + timeSpan.Minutes) / intervalInMinutes;
				dayLightIndexSpan = (daylightChanges.Delta.Hours * 60 + daylightChanges.Delta.Minutes) / intervalInMinutes;
			}
		}

		private GetUserAvailabilityInternal.DayLightsTransition GetStartToEndDayLightsTransitionValue(ExDateTime startDateFreeBusyWindow, ExDateTime endDateFreeBusyWindow, ExTimeZone timeZone)
		{
			ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug((long)this.GetHashCode(), "GetUserAvailabilityInternal.GetTransitionValue");
			bool flag = timeZone.IsDaylightSavingTime(startDateFreeBusyWindow);
			bool flag2 = timeZone.IsDaylightSavingTime(endDateFreeBusyWindow);
			if ((flag && flag2) || (!flag && !flag2))
			{
				return GetUserAvailabilityInternal.DayLightsTransition.NoTransition;
			}
			if (flag && !flag2)
			{
				return GetUserAvailabilityInternal.DayLightsTransition.TransitionFromDayLightsToStandard;
			}
			if (!flag && flag2)
			{
				return GetUserAvailabilityInternal.DayLightsTransition.TransitionFromStandardToDayLights;
			}
			return GetUserAvailabilityInternal.DayLightsTransition.NoTransition;
		}

		private static readonly string GetUserAvailabilityActionName = typeof(GetUserAvailabilityInternal).Name;

		private GetUserAvailabilityRequest request;

		private GetUserAvailabilityResponse response;

		private MailboxSession mailboxSession;

		private enum DayLightsTransition
		{
			NoTransition,
			TransitionFromStandardToDayLights,
			TransitionFromDayLightsToStandard
		}
	}
}
