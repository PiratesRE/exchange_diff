using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal static class SchedulingTabRenderingUtilities
	{
		public static int CalculateTotalWorkingHours(Microsoft.Exchange.Clients.Owa.Core.WorkingHours workingHours)
		{
			return (int)Math.Ceiling((double)workingHours.WorkDayEndTimeInWorkingHoursTimeZone / 60.0) - (int)Math.Floor((double)workingHours.WorkDayStartTimeInWorkingHoursTimeZone / 60.0);
		}

		public static int GetWorkDayStartHour(Microsoft.Exchange.Clients.Owa.Core.WorkingHours workingHours, ExDateTime date)
		{
			return (int)Math.Floor((double)workingHours.GetWorkDayStartTime(date) / 60.0);
		}

		public static int GetWorkDayEndHour(Microsoft.Exchange.Clients.Owa.Core.WorkingHours workingHours, ExDateTime date)
		{
			return (int)Math.Ceiling((double)workingHours.GetWorkDayEndTime(date) / 60.0);
		}

		public static void RenderGridDayNames(TextWriter output, ExDateTime startDate, ExDateTime endDate)
		{
			ExDateTime exDateTime = startDate;
			int num = ((DateTime)endDate - (DateTime)startDate).Days + 1;
			for (int i = 0; i < num; i++)
			{
				output.Write("\"");
				CultureInfo userCulture = OwaContext.Current.UserContext.UserCulture;
				string s = exDateTime.Date.ToString(DateTimeUtilities.GetLongDatePatternWithWeekDay(userCulture), userCulture);
				Utilities.HtmlEncode(Utilities.JavascriptEncode(s), output);
				output.Write("\"");
				if (i + 1 != num)
				{
					output.Write(",");
				}
				exDateTime = exDateTime.IncrementDays(1);
			}
		}

		public static void RenderSuggestionQualities(TextWriter output, SuggestionDayResult[] suggestionDayResults, bool renderNoDataSuggestionBuckets, ExDateTime startDate, ExDateTime endDate, Microsoft.Exchange.Clients.Owa.Core.WorkingHours workingHours)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "SchedulingTabRenderingUtilities.RenderSuggestionQualitys");
			if (suggestionDayResults == null || renderNoDataSuggestionBuckets)
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Either we did not get any suggestion buckets or we intentionally want to render buckets for no data");
				int num = ((DateTime)endDate - (DateTime)startDate).Days + 1;
				for (int i = 0; i < num; i++)
				{
					output.Write(4);
				}
				return;
			}
			UserContextManager.GetUserContext();
			int days = (suggestionDayResults[0].Date - (DateTime)startDate).Days;
			for (int j = 0; j < days; j++)
			{
				output.Write(4);
			}
			for (int k = 0; k < suggestionDayResults.Length; k++)
			{
				Suggestion[] suggestionArray = suggestionDayResults[k].SuggestionArray;
				int value = 3;
				if (0 < suggestionArray.Length)
				{
					value = (int)suggestionArray[0].SuggestionQuality;
				}
				else if (!workingHours.IsWorkDay(suggestionDayResults[k].Date.DayOfWeek))
				{
					value = 4;
				}
				output.Write(value);
			}
		}

		public static void RenderSuggestions(TextWriter output, Suggestion[] meetingSuggestions, MailboxData[] mailboxDataArray, SchedulingRecipientInfo[] schedulingRecipientInfos, UserContext userContext)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "SchedulingTabRenderingUtilities.RenderSuggestions");
			UserOptions userOptions = UserContextManager.GetUserContext().UserOptions;
			foreach (Suggestion suggestion in meetingSuggestions)
			{
				int[] array = new int[suggestion.AvailableRoomsCount];
				int num = 0;
				int num2 = suggestion.RequiredAttendeeCount - suggestion.RequiredAttendeeConflictCount;
				int num3 = suggestion.OptionalAttendeeCount - suggestion.OptionalAttendeeConflictCount;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				StringBuilder stringBuilder3 = new StringBuilder();
				StringBuilder stringBuilder4 = new StringBuilder();
				int j = 0;
				while (j < mailboxDataArray.Length)
				{
					AttendeeConflictData attendeeConflictData = suggestion.AttendeeConflictDataArray[j];
					MeetingAttendeeType meetingAttendeeType = MeetingAttendeeType.Required;
					StringBuilder stringBuilder5 = null;
					string value = string.Empty;
					if (attendeeConflictData is IndividualAttendeeConflictData)
					{
						IndividualAttendeeConflictData individualAttendeeConflictData = (IndividualAttendeeConflictData)attendeeConflictData;
						if (individualAttendeeConflictData.Attendee.AttendeeType == MeetingAttendeeType.Room && !individualAttendeeConflictData.AttendeeHasConflict)
						{
							array[num++] = j;
						}
						else
						{
							if (individualAttendeeConflictData.AttendeeHasConflict)
							{
								meetingAttendeeType = individualAttendeeConflictData.Attendee.AttendeeType;
								value = schedulingRecipientInfos[j].DisplayName;
								goto IL_1F5;
							}
							if (individualAttendeeConflictData.IsMissingFreeBusyData)
							{
								stringBuilder5 = stringBuilder4;
								value = schedulingRecipientInfos[j].DisplayName;
								goto IL_1F5;
							}
						}
					}
					else
					{
						if (attendeeConflictData is UnknownAttendeeConflictData)
						{
							stringBuilder5 = stringBuilder4;
							value = schedulingRecipientInfos[j].DisplayName;
							goto IL_1F5;
						}
						if (attendeeConflictData is TooBigGroupAttendeeConflictData)
						{
							stringBuilder5 = stringBuilder4;
							value = schedulingRecipientInfos[j].DisplayName;
							goto IL_1F5;
						}
						if (attendeeConflictData is GroupAttendeeConflictData)
						{
							GroupAttendeeConflictData groupAttendeeConflictData = (GroupAttendeeConflictData)attendeeConflictData;
							if (0 < groupAttendeeConflictData.NumberOfMembersWithNoData)
							{
								if (0 < stringBuilder4.Length)
								{
									stringBuilder4.Append(", ");
								}
								stringBuilder4.AppendFormat(LocalizedStrings.GetNonEncoded(-1170293090), schedulingRecipientInfos[j].DisplayName, groupAttendeeConflictData.NumberOfMembersWithNoData);
							}
							if (groupAttendeeConflictData.NumberOfMembersWithConflict != 0)
							{
								MailboxData mailboxData = mailboxDataArray[j];
								meetingAttendeeType = mailboxData.AttendeeType;
								value = string.Format(LocalizedStrings.GetNonEncoded(-1505134519), schedulingRecipientInfos[j].DisplayName, groupAttendeeConflictData.NumberOfMembersWithConflict, groupAttendeeConflictData.NumberOfMembers);
								goto IL_1F5;
							}
						}
					}
					IL_24B:
					j++;
					continue;
					IL_1F5:
					if (stringBuilder5 == null)
					{
						switch (meetingAttendeeType)
						{
						case MeetingAttendeeType.Organizer:
						case MeetingAttendeeType.Required:
							stringBuilder5 = stringBuilder;
							break;
						case MeetingAttendeeType.Optional:
							stringBuilder5 = stringBuilder2;
							break;
						case MeetingAttendeeType.Room:
						case MeetingAttendeeType.Resource:
							stringBuilder5 = stringBuilder3;
							break;
						}
					}
					if (0 < stringBuilder5.Length)
					{
						stringBuilder5.Append(", ");
					}
					stringBuilder5.Append(value);
					goto IL_24B;
				}
				output.Write("<div class=\"sug\" st=");
				output.Write(suggestion.MeetingTime.TimeOfDay.TotalMinutes);
				output.Write(" title=\"");
				if (0 < suggestion.RequiredAttendeeConflictCount || 0 < suggestion.OptionalAttendeeConflictCount || 0 < suggestion.ResourceAttendeeConflictCount)
				{
					output.Write(LocalizedStrings.GetHtmlEncoded(-1917590199));
					if (0 < stringBuilder.Length)
					{
						output.Write("\n");
						output.Write(LocalizedStrings.GetHtmlEncoded(-1709254790));
						output.Write(" ");
						Utilities.HtmlEncode(stringBuilder.ToString(), output);
					}
					if (0 < stringBuilder2.Length)
					{
						output.Write("\n");
						output.Write(LocalizedStrings.GetHtmlEncoded(-98673561));
						output.Write(" ");
						Utilities.HtmlEncode(stringBuilder2.ToString(), output);
					}
					if (0 < stringBuilder3.Length)
					{
						output.Write("\n");
						output.Write(LocalizedStrings.GetHtmlEncoded(-294537986));
						output.Write(" ");
						Utilities.HtmlEncode(stringBuilder3.ToString(), output);
					}
					if (0 < stringBuilder4.Length)
					{
						output.Write("\n");
						output.Write(LocalizedStrings.GetHtmlEncoded(608956012));
						output.Write(" ");
						Utilities.HtmlEncode(stringBuilder4.ToString(), output);
					}
				}
				output.Write("\">");
				output.Write("<div class=\"");
				output.Write(SchedulingTabRenderingUtilities.SuggestionQualityStyles[(int)suggestion.SuggestionQuality]);
				output.Write("\"></div><div class=\"suggestDetail\">");
				output.Write("<div class=\"when\"><span>");
				output.Write(suggestion.MeetingTime.ToString(userOptions.TimeFormat));
				if (0 < suggestion.RoomCount)
				{
					output.Write(" - ");
					output.Write((suggestion.RoomCount == 1) ? LocalizedStrings.GetHtmlEncoded(-1595764264) : LocalizedStrings.GetHtmlEncoded(-1962164027), suggestion.AvailableRoomsCount);
				}
				output.Write("</span></div>");
				output.Write("<div class=\"atndcnt\">");
				if (0 < suggestion.RequiredAttendeeCount)
				{
					output.Write("<span nowrap>");
					userContext.RenderThemeImage(output, ThemeFileId.RequiredAttendee);
					output.Write(LocalizedStrings.GetHtmlEncoded(1026164821), num2, suggestion.RequiredAttendeeCount);
					output.Write("</span>");
				}
				if (0 < suggestion.OptionalAttendeeCount)
				{
					output.Write(" <span nowrap>");
					userContext.RenderThemeImage(output, ThemeFileId.OptionalAttendee);
					output.Write(LocalizedStrings.GetHtmlEncoded(1026164821), num3, suggestion.OptionalAttendeeCount);
					output.Write("</span>");
				}
				output.Write("</div>");
				if (0 < suggestion.AvailableRoomsCount)
				{
					output.Write("<div id=rooms style=\"display:none\">");
					foreach (int num4 in array)
					{
						output.Write("<div nowrap rm=\"");
						Utilities.HtmlEncode(schedulingRecipientInfos[num4].ID, output);
						output.Write("\">");
						Utilities.HtmlEncode(schedulingRecipientInfos[num4].DisplayName, output);
						output.Write("</div>");
					}
					output.Write("</div>");
				}
				output.Write("</div></div>");
			}
		}

		public static void RenderRecipientFreeBusyData(TextWriter output, SchedulingRecipientInfo[] schedulingRecipientInfos, FreeBusyQueryResult[] freeBusyQueryResults, ExDateTime startDateFreeBusyWindow, ExDateTime endDateFreeBusyWindow, bool show24Hours, bool renderDataForAllRecipients, ExTimeZone timeZone, Microsoft.Exchange.Clients.Owa.Core.WorkingHours workingHours)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "SchedulingTabRenderingUtilities.RenderRecipientFreeBusyData");
			int days = ((DateTime)endDateFreeBusyWindow - (DateTime)startDateFreeBusyWindow).Days;
			int num = 0;
			int num2 = 48;
			if (!show24Hours)
			{
				num = SchedulingTabRenderingUtilities.GetWorkDayStartHour(workingHours, startDateFreeBusyWindow) * 60 / 30;
				num2 = SchedulingTabRenderingUtilities.GetWorkDayEndHour(workingHours, startDateFreeBusyWindow) * 60 / 30;
			}
			int num3 = 48;
			int num4 = num2 - num;
			output.Write("<div id=fbd>");
			output.Write("var rgUFB = new Array();");
			int num5 = 0;
			for (int i = 0; i < schedulingRecipientInfos.Length; i++)
			{
				FreeBusyQueryResult freeBusyQueryResult = (freeBusyQueryResults == null) ? null : freeBusyQueryResults[i];
				SchedulingRecipientInfo schedulingRecipientInfo = schedulingRecipientInfos[i];
				if (renderDataForAllRecipients || schedulingRecipientInfo.GetFreeBusyData)
				{
					output.Write("rgUFB[");
					output.Write(num5++);
					output.Write("] = new RecipientFreeBusyData(\"");
					output.Write(schedulingRecipientInfo.ID);
					output.Write("\",\"");
					if (freeBusyQueryResult == null || Utilities.IsFatalFreeBusyError(freeBusyQueryResult.ExceptionInfo) || string.IsNullOrEmpty(freeBusyQueryResult.MergedFreeBusy))
					{
						ExTraceGlobals.CalendarDataTracer.TraceDebug<string, string, object>(0L, "Unable to get free/busy data for user '{0} ({1})' Exception: {2}", schedulingRecipientInfo.DisplayName, schedulingRecipientInfo.EmailAddress, (freeBusyQueryResult != null) ? freeBusyQueryResult.ExceptionInfo : "<recipientQueryResult is null>");
						int num6 = num4 * days;
						for (int j = 0; j < num6; j++)
						{
							output.Write('4');
						}
						if (freeBusyQueryResult != null)
						{
							output.Write("\",\"{0}", freeBusyQueryResult.ExceptionInfo.ErrorCode);
						}
					}
					else
					{
						string mergedFreeBusy = freeBusyQueryResult.MergedFreeBusy;
						SchedulingTabRenderingUtilities.SetFreeBusyDayLightBasedValue(startDateFreeBusyWindow, endDateFreeBusyWindow, timeZone, ref mergedFreeBusy);
						for (int k = 0; k < days; k++)
						{
							int num7 = k * num3 + num;
							for (int l = 0; l < num4; l++)
							{
								output.Write(mergedFreeBusy[num7 + l]);
							}
						}
					}
					output.Write("\");");
				}
			}
			output.Write("</div>");
		}

		public static void SetFreeBusyDayLightBasedValue(ExDateTime startDate, ExDateTime endDate, ExTimeZone timeZone, ref string freeBusyData)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "SchedulingTabRenderingUtilities.GetFreeBusyDayLightIndex");
			if (string.IsNullOrEmpty(freeBusyData))
			{
				throw new ArgumentNullException("freeBusyData", "FreeBusyData cannot be null or Empty");
			}
			int num = 0;
			int num2 = 0;
			DayLightsTransition startToEndDayLightsTransitionValue = SchedulingTabRenderingUtilities.GetStartToEndDayLightsTransitionValue(startDate, endDate, timeZone);
			if (startToEndDayLightsTransitionValue == DayLightsTransition.NoTransition)
			{
				return;
			}
			SchedulingTabRenderingUtilities.SetDayLightsSavingIndices(startDate, endDate, timeZone, startToEndDayLightsTransitionValue, out num, out num2);
			if (startToEndDayLightsTransitionValue == DayLightsTransition.TransitionFromStandardToDayLights)
			{
				StringBuilder stringBuilder = new StringBuilder(freeBusyData.Length);
				stringBuilder.Append(freeBusyData.Substring(0, num));
				for (int i = 0; i < num2; i++)
				{
					stringBuilder.Append('0');
				}
				stringBuilder.Append(freeBusyData.Substring(num));
				freeBusyData = stringBuilder.ToString();
				return;
			}
			if (startToEndDayLightsTransitionValue == DayLightsTransition.TransitionFromDayLightsToStandard)
			{
				StringBuilder stringBuilder2 = new StringBuilder(freeBusyData.Length);
				stringBuilder2.Append(freeBusyData.Substring(0, num));
				stringBuilder2.Append(freeBusyData.Substring(num + num2));
				freeBusyData = stringBuilder2.ToString();
			}
		}

		private static void SetDayLightsSavingIndices(ExDateTime startDateFreeBusyWindow, ExDateTime endDateFreeBusyWindow, ExTimeZone timeZone, DayLightsTransition transition, out int transitionIndex, out int dayLightIndexSpan)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "SchedulingTabRenderingUtilities.SetDayLightsSavingIndices");
			transitionIndex = 0;
			dayLightIndexSpan = 0;
			if (transition != DayLightsTransition.NoTransition)
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
				transitionIndex = (timeSpan.Days * 60 * 24 + timeSpan.Hours * 60 + timeSpan.Minutes) / 30;
				dayLightIndexSpan = (daylightChanges.Delta.Hours * 60 + daylightChanges.Delta.Minutes) / 30;
			}
		}

		private static DayLightsTransition GetStartToEndDayLightsTransitionValue(ExDateTime startDateFreeBusyWindow, ExDateTime endDateFreeBusyWindow, ExTimeZone timeZone)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "SchedulingTabRenderingUtilities.GetTransitionValue");
			bool flag = timeZone.IsDaylightSavingTime(startDateFreeBusyWindow);
			bool flag2 = timeZone.IsDaylightSavingTime(endDateFreeBusyWindow);
			if ((flag && flag2) || (!flag && !flag2))
			{
				return DayLightsTransition.NoTransition;
			}
			if (flag && !flag2)
			{
				return DayLightsTransition.TransitionFromDayLightsToStandard;
			}
			if (!flag && flag2)
			{
				return DayLightsTransition.TransitionFromStandardToDayLights;
			}
			return DayLightsTransition.NoTransition;
		}

		private const int HourWidth = 50;

		public const int FreeBusyInterval = 30;

		private static readonly string[] SuggestionQualityStyles = new string[]
		{
			"grt",
			"gd",
			"fr",
			"por"
		};
	}
}
