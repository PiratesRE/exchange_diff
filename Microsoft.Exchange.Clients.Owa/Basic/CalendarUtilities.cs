using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal static class CalendarUtilities
	{
		public static bool AddCalendarInfobarMessages(Infobar infobar, CalendarItemBase calendarItemBase, MeetingMessage meetingMessage, UserContext userContext)
		{
			bool result = false;
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (calendarItemBase == null)
			{
				throw new ArgumentNullException("calendarItemBase");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			bool flag = calendarItemBase.IsOrganizer() && calendarItemBase.IsMeeting;
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			bool flag2 = false;
			if (calendarItemBase.IsMeeting && calendarItemBase.IsCancelled)
			{
				infobar.AddMessageLocalized(-161808760, InfobarMessageType.Informational);
				result = true;
			}
			if (calendarItemBase is CalendarItemOccurrence && !calendarItemBase.IsOrganizer())
			{
				VersionedId id = calendarItemBase.Id;
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(id.ObjectId.ProviderLevelItemId);
				SanitizedHtmlString obj = SanitizedHtmlString.Format("<a href=\"#\" onClick=\"return onClkAppt('{0}')\">", new object[]
				{
					Utilities.JavascriptEncode(HttpUtility.UrlEncode(storeObjectId.ToBase64String()))
				});
				string str = "</a>";
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1179776776));
				sanitizingStringBuilder.Append(" ");
				sanitizingStringBuilder.Append<SanitizedHtmlString>(obj);
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(343581894));
				sanitizingStringBuilder.Append(str);
				infobar.AddMessageHtml(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational);
				result = true;
			}
			if (calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster)
			{
				CalendarItem calendarItem = (CalendarItem)calendarItemBase;
				if (calendarItem.Recurrence != null && !(calendarItem.Recurrence.Range is NoEndRecurrenceRange))
				{
					OccurrenceInfo lastOccurrence = calendarItem.Recurrence.GetLastOccurrence();
					if (lastOccurrence != null && lastOccurrence.EndTime < localTime)
					{
						infobar.AddMessageLocalized(-2124392108, InfobarMessageType.Informational);
						flag2 = true;
						result = true;
					}
				}
			}
			else if (calendarItemBase.EndTime < localTime)
			{
				flag2 = true;
				if (calendarItemBase.CalendarItemType != CalendarItemType.RecurringMaster)
				{
					infobar.AddMessageLocalized(-593429293, InfobarMessageType.Informational);
					result = true;
				}
			}
			if (flag)
			{
				if (calendarItemBase.MeetingRequestWasSent)
				{
					CalendarUtilities.AddAttendeeResponseCountMessage(infobar, calendarItemBase);
				}
				else
				{
					infobar.AddMessageLocalized(613373695, InfobarMessageType.Informational);
				}
				result = true;
			}
			if (!calendarItemBase.IsOrganizer() && calendarItemBase.IsMeeting)
			{
				if (calendarItemBase.ResponseType != ResponseType.NotResponded)
				{
					string text = null;
					switch (calendarItemBase.ResponseType)
					{
					case ResponseType.Tentative:
						text = LocalizedStrings.GetNonEncoded(-1859761232);
						break;
					case ResponseType.Accept:
						text = LocalizedStrings.GetNonEncoded(-700793833);
						break;
					case ResponseType.Decline:
						text = LocalizedStrings.GetNonEncoded(-278420592);
						break;
					}
					if (text != null)
					{
						ExDateTime property = ItemUtility.GetProperty<ExDateTime>(calendarItemBase, CalendarItemBaseSchema.AppointmentReplyTime, ExDateTime.MinValue);
						string arg = LocalizedStrings.GetNonEncoded(1414246128);
						string arg2 = string.Empty;
						if (property != ExDateTime.MinValue)
						{
							arg = property.ToString(userContext.UserOptions.DateFormat);
							arg2 = property.ToString(userContext.UserOptions.TimeFormat);
						}
						infobar.AddMessageText(string.Format(text, arg, arg2), InfobarMessageType.Informational);
						result = true;
					}
				}
				else if (!flag2 && !calendarItemBase.IsCancelled)
				{
					bool flag3 = false;
					MeetingRequest meetingRequest = meetingMessage as MeetingRequest;
					if (meetingRequest != null)
					{
						flag3 = (meetingRequest.MeetingRequestType == MeetingMessageType.PrincipalWantsCopy);
					}
					if (!flag3)
					{
						bool flag4 = true;
						object obj2 = calendarItemBase.TryGetProperty(ItemSchema.IsResponseRequested);
						if (obj2 is bool)
						{
							flag4 = (bool)obj2;
						}
						if (flag4)
						{
							infobar.AddMessageLocalized(919273049, InfobarMessageType.Informational);
						}
						else
						{
							infobar.AddMessageLocalized(1602295502, InfobarMessageType.Informational);
						}
						result = true;
					}
					CalendarUtilities.GetConflictingAppointments(infobar, calendarItemBase, userContext);
				}
			}
			return result;
		}

		private static void AddAttendeeResponseCountMessage(Infobar infobar, CalendarItemBase calendarItemBase)
		{
			SanitizedHtmlString attendeeResponseCountMessage = MeetingUtilities.GetAttendeeResponseCountMessage(calendarItemBase);
			infobar.AddMessageHtml(attendeeResponseCountMessage, InfobarMessageType.Informational);
		}

		private static void GetConflictingAppointments(Infobar infobar, CalendarItemBase calendarItemBase, UserContext userContext)
		{
			AdjacencyOrConflictInfo[] array = null;
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(userContext.MailboxSession, DefaultFolderType.Calendar))
			{
				array = calendarFolder.GetAdjacentOrConflictingItems(calendarItemBase);
			}
			if (array != null && array.Length != 0)
			{
				CalendarUtilities.AddConflictingAppointmentsInfobarMessage(infobar, array, userContext, calendarItemBase.CalendarItemType);
			}
		}

		private static void AddConflictingAppointmentsInfobarMessage(Infobar infobar, AdjacencyOrConflictInfo[] adjacencyOrConflictInfo, UserContext userContext, CalendarItemType calendarItemType)
		{
			List<AdjacencyOrConflictInfo> list = new List<AdjacencyOrConflictInfo>();
			List<AdjacencyOrConflictInfo> list2 = new List<AdjacencyOrConflictInfo>();
			List<AdjacencyOrConflictInfo> list3 = new List<AdjacencyOrConflictInfo>();
			for (int i = 0; i < adjacencyOrConflictInfo.Length; i++)
			{
				AdjacencyOrConflictType adjacencyOrConflictType = adjacencyOrConflictInfo[i].AdjacencyOrConflictType;
				BusyType freeBusyStatus = adjacencyOrConflictInfo[i].FreeBusyStatus;
				if (freeBusyStatus != BusyType.Free)
				{
					if ((adjacencyOrConflictType & AdjacencyOrConflictType.Conflicts) != (AdjacencyOrConflictType)0)
					{
						list.Add(adjacencyOrConflictInfo[i]);
					}
					else if ((adjacencyOrConflictType & AdjacencyOrConflictType.Precedes) != (AdjacencyOrConflictType)0)
					{
						list2.Add(adjacencyOrConflictInfo[i]);
					}
					else if ((adjacencyOrConflictType & AdjacencyOrConflictType.Follows) != (AdjacencyOrConflictType)0)
					{
						list3.Add(adjacencyOrConflictInfo[i]);
					}
				}
			}
			if (list.Count == 0 && list2.Count == 0 && list3.Count == 0)
			{
				return;
			}
			string queryStringParameter = Utilities.GetQueryStringParameter(HttpContext.Current.Request, "shc", false);
			bool flag = false;
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				flag = (queryStringParameter == "1");
			}
			if (calendarItemType != CalendarItemType.RecurringMaster)
			{
				SanitizedHtmlString sanitizedStringWithoutEncoding;
				SanitizedHtmlString sanitizedStringWithoutEncoding2;
				if (flag)
				{
					sanitizedStringWithoutEncoding = SanitizedHtmlString.GetSanitizedStringWithoutEncoding(" <a href=\"\" onclick=\"return shwConf(0);\">");
					sanitizedStringWithoutEncoding2 = SanitizedHtmlString.GetSanitizedStringWithoutEncoding("<img src=\"" + userContext.GetThemeFileUrl(ThemeFileId.Collapse) + "\">");
				}
				else
				{
					sanitizedStringWithoutEncoding = SanitizedHtmlString.GetSanitizedStringWithoutEncoding(" <a href=\"\" onclick=\"return shwConf(1);\">");
					sanitizedStringWithoutEncoding2 = SanitizedHtmlString.GetSanitizedStringWithoutEncoding("<img src=\"" + userContext.GetThemeFileUrl(ThemeFileId.Expand) + "\">");
				}
				string str = "</a>";
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				if (list.Count > 0)
				{
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-812272237));
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding);
					if (flag)
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-669919370));
					}
					else
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1786149639));
					}
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding2);
					sanitizingStringBuilder.Append(str);
				}
				else if (list2.Count > 0 && list3.Count > 0)
				{
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(2138994880));
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding);
					if (flag)
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1083835406));
					}
					else
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1877110893));
					}
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding2);
					sanitizingStringBuilder.Append(str);
				}
				else if (list2.Count > 0)
				{
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1508975609));
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding);
					if (flag)
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1796482192));
					}
					else
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(2029212075));
					}
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding2);
					sanitizingStringBuilder.Append(str);
				}
				else if (list3.Count > 0)
				{
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1710537313));
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding);
					if (flag)
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1733349590));
					}
					else
					{
						sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-608468101));
					}
					sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedStringWithoutEncoding2);
					sanitizingStringBuilder.Append(str);
				}
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder2 = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder2.Append("<table id=tblCnf cellpadding=0 cellspacing=4>");
				if (list.Count > 0)
				{
					sanitizingStringBuilder2.Append<SanitizedHtmlString>(CalendarUtilities.BuildAdjacencyOrConflictSection(list, LocalizedStrings.GetNonEncoded(-1874853770), userContext));
				}
				if (list2.Count > 0)
				{
					sanitizingStringBuilder2.Append<SanitizedHtmlString>(CalendarUtilities.BuildAdjacencyOrConflictSection(list2, LocalizedStrings.GetNonEncoded(2095567903), userContext));
				}
				if (list3.Count > 0)
				{
					sanitizingStringBuilder2.Append<SanitizedHtmlString>(CalendarUtilities.BuildAdjacencyOrConflictSection(list3, LocalizedStrings.GetNonEncoded(-51439729), userContext));
				}
				sanitizingStringBuilder2.Append("</table>");
				infobar.AddMessage(InfobarMessage.CreateExpandingHtml(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), sanitizingStringBuilder2.ToSanitizedString<SanitizedHtmlString>(), flag));
				return;
			}
			string messageText = null;
			if (list.Count > 0)
			{
				messageText = LocalizedStrings.GetNonEncoded(890561325);
			}
			else if (list2.Count > 0 || list3.Count > 0)
			{
				messageText = LocalizedStrings.GetNonEncoded(1923039961);
			}
			infobar.AddMessageText(messageText, InfobarMessageType.Informational);
		}

		private static SanitizedHtmlString BuildAdjacencyOrConflictSection(List<AdjacencyOrConflictInfo> appointments, string sectionTitle, UserContext userContext)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>(16);
			sanitizingStringBuilder.Append("<tr><td class=\"hdr bld\" colspan=5 nowrap>");
			sanitizingStringBuilder.Append(sectionTitle);
			sanitizingStringBuilder.Append("</td></tr>");
			bool flag = true;
			int count = appointments.Count;
			foreach (AdjacencyOrConflictInfo adjacencyOrConflictInfo in appointments)
			{
				BusyType freeBusyStatus = adjacencyOrConflictInfo.FreeBusyStatus;
				sanitizingStringBuilder.Append("<tr>");
				if (flag)
				{
					sanitizingStringBuilder.Append("<td");
					if (count > 1)
					{
						sanitizingStringBuilder.Append(" rowspan=");
						sanitizingStringBuilder.Append<int>(count);
					}
					sanitizingStringBuilder.Append("><img class=\"lmgn\" src=\"");
					sanitizingStringBuilder.Append(userContext.GetThemeFileUrl(ThemeFileId.Clear));
					sanitizingStringBuilder.Append("\" alt=\"\"></td>");
					flag = false;
				}
				sanitizingStringBuilder.Append("<td");
				switch (freeBusyStatus)
				{
				case BusyType.Tentative:
					sanitizingStringBuilder.Append("><img class=\"tntv\" src=\"");
					sanitizingStringBuilder.Append(userContext.GetThemeFileUrl(ThemeFileId.Tentative));
					sanitizingStringBuilder.Append("\">");
					break;
				case BusyType.Busy:
					sanitizingStringBuilder.Append("><img class=\"busy\" src=\"");
					sanitizingStringBuilder.Append(userContext.GetThemeFileUrl(ThemeFileId.Clear));
					sanitizingStringBuilder.Append("\" alt=\"\">");
					break;
				case BusyType.OOF:
					sanitizingStringBuilder.Append("><img class=\"oof\" src=\"");
					sanitizingStringBuilder.Append(userContext.GetThemeFileUrl(ThemeFileId.Clear));
					sanitizingStringBuilder.Append("\" alt=\"\">");
					break;
				}
				sanitizingStringBuilder.Append("</td><td class=\"IbL\">");
				if (!userContext.IsWebPartRequest)
				{
					sanitizingStringBuilder.Append("<a href=\"\" onclick=\"return onClkAppt('");
					sanitizingStringBuilder.Append(Utilities.JavascriptEncode(Utilities.UrlEncode(adjacencyOrConflictInfo.OccurrenceInfo.VersionedId.ObjectId.ToBase64String())));
					sanitizingStringBuilder.Append("'); return false;\">");
				}
				string text = string.Empty;
				if (!string.IsNullOrEmpty(adjacencyOrConflictInfo.Subject))
				{
					text = adjacencyOrConflictInfo.Subject.Trim();
				}
				if (text.Length == 0)
				{
					sanitizingStringBuilder.Append(userContext.DirectionMark);
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(6409762));
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-776227687));
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1023695022));
					sanitizingStringBuilder.Append(userContext.DirectionMark);
				}
				else
				{
					sanitizingStringBuilder.Append(text);
				}
				if (!userContext.IsWebPartRequest)
				{
					sanitizingStringBuilder.Append("</a>");
				}
				sanitizingStringBuilder.Append("</td><td class=\"dt\">");
				string empty = string.Empty;
				ExDateTime startTime = adjacencyOrConflictInfo.OccurrenceInfo.StartTime;
				ExDateTime endTime = adjacencyOrConflictInfo.OccurrenceInfo.EndTime;
				TimeSpan timeSpan = endTime - startTime;
				if (startTime.Day != endTime.Day || timeSpan.TotalDays >= 1.0)
				{
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(492249539), new object[]
					{
						startTime.ToString(userContext.UserOptions.DateFormat),
						startTime.ToString(userContext.UserOptions.TimeFormat),
						endTime.ToString(userContext.UserOptions.DateFormat),
						endTime.ToString(userContext.UserOptions.TimeFormat)
					});
				}
				else
				{
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(-792821726), new object[]
					{
						startTime.ToString(userContext.UserOptions.TimeFormat),
						endTime.ToString(userContext.UserOptions.TimeFormat)
					});
				}
				sanitizingStringBuilder.Append("</td><td class=\"loc\">");
				sanitizingStringBuilder.Append(adjacencyOrConflictInfo.Location);
				sanitizingStringBuilder.Append("</td></tr>");
			}
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		public static Recurrence CreateDefaultRecurrence(ExDateTime startDate)
		{
			DaysOfWeek daysOfWeek = CalendarUtilities.ConvertDateTimeToDaysOfWeek(startDate);
			RecurrencePattern pattern = new WeeklyRecurrencePattern(daysOfWeek, 1);
			RecurrenceRange range = new NoEndRecurrenceRange(startDate);
			return new Recurrence(pattern, range);
		}

		public static RecurrencePattern CreateDefaultRecurrencePattern(OwaRecurrenceType type, ExDateTime date)
		{
			DaysOfWeek daysOfWeek = CalendarUtilities.ConvertDateTimeToDaysOfWeek(date);
			RecurrenceOrderType order = (RecurrenceOrderType)CalendarUtilities.ComputeDayOfMonthOrder(date);
			if (type <= (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyEveryWeekday))
			{
				if (type <= OwaRecurrenceType.Monthly)
				{
					switch (type)
					{
					case OwaRecurrenceType.None:
					case OwaRecurrenceType.None | OwaRecurrenceType.Daily:
						break;
					case OwaRecurrenceType.Daily:
						return new DailyRecurrencePattern();
					case OwaRecurrenceType.Weekly:
						return new WeeklyRecurrencePattern(daysOfWeek, 1);
					default:
						if (type == OwaRecurrenceType.Monthly)
						{
							return new MonthlyRecurrencePattern(date.Day, 1);
						}
						break;
					}
				}
				else
				{
					if (type == OwaRecurrenceType.Yearly)
					{
						return new YearlyRecurrencePattern(date.Day, date.Month);
					}
					switch (type)
					{
					case OwaRecurrenceType.DailyEveryWeekday:
					case OwaRecurrenceType.Daily | OwaRecurrenceType.DailyEveryWeekday:
						return new WeeklyRecurrencePattern(DaysOfWeek.Weekdays);
					}
				}
			}
			else if (type <= (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh))
			{
				if (type == OwaRecurrenceType.MonthlyTh || type == (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh))
				{
					return new MonthlyThRecurrencePattern(daysOfWeek, order, 1);
				}
			}
			else if (type == OwaRecurrenceType.YearlyTh || type == (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh))
			{
				return new YearlyThRecurrencePattern(daysOfWeek, order, date.Month);
			}
			throw new ArgumentException("Can't create default RecurrencePattern of type None or unknown.");
		}

		public static int ComputeDayOfMonthOrder(ExDateTime date)
		{
			int num = (date.Day - 1) / 7 + 1;
			if (num > 4)
			{
				num = -1;
			}
			return num;
		}

		public static OwaRecurrenceType MapRecurrenceType(Recurrence recurrence)
		{
			OwaRecurrenceType result = OwaRecurrenceType.None;
			if (recurrence == null)
			{
				result = OwaRecurrenceType.None;
			}
			else if (recurrence.Pattern is DailyRecurrencePattern)
			{
				result = OwaRecurrenceType.Daily;
			}
			else if (recurrence.Pattern is WeeklyRecurrencePattern)
			{
				WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)recurrence.Pattern;
				if (weeklyRecurrencePattern.DaysOfWeek == DaysOfWeek.Weekdays)
				{
					result = (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyEveryWeekday);
				}
				else
				{
					result = OwaRecurrenceType.Weekly;
				}
			}
			else if (recurrence.Pattern is MonthlyRecurrencePattern)
			{
				result = OwaRecurrenceType.Monthly;
			}
			else if (recurrence.Pattern is MonthlyThRecurrencePattern)
			{
				result = (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh);
			}
			else if (recurrence.Pattern is YearlyRecurrencePattern)
			{
				result = OwaRecurrenceType.Yearly;
			}
			else if (recurrence.Pattern is YearlyThRecurrencePattern)
			{
				result = (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh);
			}
			return result;
		}

		public static void RenderDateTimeTable(TextWriter writer, string namePrefix, ExDateTime dateTime, int includeYear, string timeFormatString, string tdClass, string onChange, string selectClass)
		{
			string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
			string selectName = namePrefix + "M";
			string selectName2 = namePrefix + "D";
			string selectName3 = namePrefix + "Y";
			string name = namePrefix + "T";
			writer.Write("<table class=\"caltm\" cellpadding=\"0\" cellspacing=\"0\"><tr>\n");
			writer.Write("<td class=\"");
			writer.Write(tdClass);
			writer.Write("\">");
			writer.Write(CalendarUtilities.BuildSelectStartTag(selectName, selectClass, onChange));
			for (int i = 1; i < monthNames.Length; i++)
			{
				writer.Write("<option value=\"{1}\"{0}>{2}</option>", (i == dateTime.Month) ? " selected" : string.Empty, i, Utilities.SanitizeHtmlEncode(monthNames[i - 1]));
			}
			writer.Write("</select></td>");
			writer.Write("<td class=\"");
			writer.Write(tdClass);
			writer.Write("\">");
			writer.Write(CalendarUtilities.BuildSelectStartTag(selectName2, selectClass, onChange));
			for (int j = 1; j <= 31; j++)
			{
				writer.Write("<option value=\"{1}\"{0}>{2}</option>", (j == dateTime.Day) ? " selected" : string.Empty, j, j);
			}
			writer.Write("</select></td>");
			writer.Write("<td class=\"");
			writer.Write(tdClass);
			writer.Write("\">");
			writer.Write(CalendarUtilities.BuildSelectStartTag(selectName3, selectClass, onChange));
			int year = DateTimeUtilities.GetLocalTime().Year;
			if (includeYear <= 0)
			{
				includeYear = year;
			}
			int num = Math.Min(Math.Min(includeYear, year) - 2, dateTime.Year);
			int num2 = Math.Max(Math.Max(includeYear, year) + 4, dateTime.Year);
			for (int k = num; k <= num2; k++)
			{
				writer.Write("<option value=\"{1}\"{0}>{2}</option>", (k == dateTime.Year) ? " selected" : string.Empty, k, k);
			}
			writer.Write("</select></td>");
			if (!string.IsNullOrEmpty(timeFormatString))
			{
				writer.Write("<td class=\"");
				writer.Write(tdClass);
				writer.Write("\" style=\"padding-right:0px\">");
				CalendarUtilities.RenderTimeSelect(writer, name, dateTime, timeFormatString, onChange, string.Empty);
				writer.WriteLine("</td>");
			}
			writer.WriteLine("</tr></table>");
		}

		public static void RenderTimeSelect(TextWriter writer, string name, ExDateTime dateTime, string timeFormatString, string onChange, string selectClass)
		{
			List<DateTime> list = new List<DateTime>();
			DateTime item = new DateTime(1, 1, 1, dateTime.Hour, dateTime.Minute, 0);
			for (int i = 0; i < 24; i++)
			{
				list.Add(new DateTime(1, 1, 1, i, 0, 0));
				if (item.Hour == i && item.Minute > 0 && item.Minute < 30)
				{
					list.Add(item);
				}
				list.Add(new DateTime(1, 1, 1, i, 30, 0));
				if (item.Hour == i && item.Minute > 30)
				{
					list.Add(item);
				}
			}
			writer.Write(CalendarUtilities.BuildSelectStartTag(name, selectClass, onChange));
			foreach (DateTime dateTime2 in list)
			{
				string arg = (item.Hour == dateTime2.Hour && item.Minute == dateTime2.Minute) ? " selected" : string.Empty;
				writer.Write("<option value=\"{1}\"{0}>{2}</option>", arg, dateTime2.Hour * 60 + dateTime2.Minute, Utilities.SanitizeHtmlEncode(dateTime2.ToString(timeFormatString, CultureInfo.CurrentUICulture.DateTimeFormat)));
			}
			writer.WriteLine("</select>");
		}

		public static void RenderDurationSelect(TextWriter writer, string name, int selectedDurationMinutes, string onChange, string selectClass)
		{
			List<int> list = new List<int>();
			for (int i = 0; i <= 1440; i += 30)
			{
				list.Add(i);
			}
			if (!list.Contains(selectedDurationMinutes))
			{
				list.Add(selectedDurationMinutes);
				list.Sort();
			}
			writer.Write(CalendarUtilities.BuildSelectStartTag(name, selectClass, onChange));
			foreach (int num in list)
			{
				string arg = (num == selectedDurationMinutes) ? " selected" : string.Empty;
				writer.Write("<option value=\"{1}\"{0}>{2}</option>", arg, num, DateTimeUtilities.FormatDuration(num));
			}
			writer.WriteLine("</select>");
		}

		public static ExDateTime ParseDateTimeFromForm(HttpRequest request, string yearParameter, string monthParameter, string dayParameter, string timeParameter, UserContext userContext)
		{
			string formParameter = Utilities.GetFormParameter(request, yearParameter, false);
			string formParameter2 = Utilities.GetFormParameter(request, monthParameter, false);
			string formParameter3 = Utilities.GetFormParameter(request, dayParameter, false);
			ExDateTime minValue = ExDateTime.MinValue;
			if (!string.IsNullOrEmpty(formParameter2) && !string.IsNullOrEmpty(formParameter3) && !string.IsNullOrEmpty(formParameter))
			{
				int year;
				int month;
				int num;
				if (int.TryParse(formParameter, out year) && int.TryParse(formParameter2, out month) && int.TryParse(formParameter3, out num))
				{
					try
					{
						int val = ExDateTime.DaysInMonth(year, month);
						num = Math.Min(num, val);
						DateTime dateTime = new DateTime(year, month, num, 0, 0, 0);
						if (!string.IsNullOrEmpty(timeParameter))
						{
							TimeSpan timeSpan = CalendarUtilities.ParseTimeFromForm(request, timeParameter);
							if (!(timeSpan != TimeSpan.MinValue))
							{
								throw new OwaInvalidRequestException("Unable to parse time from form parameters.");
							}
							dateTime = dateTime.Add(timeSpan);
						}
						return new ExDateTime(userContext.TimeZone, dateTime);
					}
					catch (ArgumentOutOfRangeException)
					{
						throw new OwaInvalidRequestException("Bad year, month, or day value in form parameters.");
					}
				}
				throw new OwaInvalidRequestException("Unable to parse date from form parameters.");
			}
			throw new OwaInvalidRequestException("Unable to find date fields in form parameters.");
		}

		public static TimeSpan ParseTimeFromForm(HttpRequest request, string timeParameter)
		{
			TimeSpan minValue = TimeSpan.MinValue;
			string formParameter = Utilities.GetFormParameter(request, timeParameter, false);
			int num;
			if (!string.IsNullOrEmpty(formParameter) && int.TryParse(formParameter, out num))
			{
				if (num < 0 || num > 1440)
				{
					throw new OwaInvalidRequestException("The minutes-in-day value is out of range.");
				}
				int hours = num / 60;
				int minutes = num % 60;
				minValue = new TimeSpan(hours, minutes, 0);
			}
			return minValue;
		}

		public static DaysOfWeek ConvertDateTimeToDaysOfWeek(ExDateTime dateTime)
		{
			return CalendarUtilities.ConvertDayOfWeekToDaysOfWeek(dateTime.DayOfWeek);
		}

		public static DaysOfWeek ConvertDayOfWeekToDaysOfWeek(DayOfWeek dayOfWeek)
		{
			return (DaysOfWeek)(1 << (int)dayOfWeek);
		}

		public static string BuildDayIndexDropdownList(int selectedDay, string selectName, string selectClass, string onChange)
		{
			StringWriter stringWriter = new StringWriter();
			stringWriter.Write(CalendarUtilities.BuildSelectStartTag(selectName, selectClass, onChange));
			for (int i = 1; i < 32; i++)
			{
				string arg = (selectedDay == i) ? " selected" : string.Empty;
				stringWriter.Write("<option value=\"{0}\"{1}>{0}</option>", i, arg);
			}
			stringWriter.Write("</select>");
			stringWriter.Close();
			return stringWriter.ToString();
		}

		public static string BuildNthDropdownList(int order, string selectName, string selectClass, string onChange)
		{
			StringWriter stringWriter = new StringWriter();
			stringWriter.Write(CalendarUtilities.BuildSelectStartTag(selectName, selectClass, onChange));
			for (int i = 0; i < CalendarUtilities.nThList.Length; i++)
			{
				int num = i + 1;
				if (num > 4)
				{
					num = -1;
				}
				string arg = (order == num) ? " selected" : string.Empty;
				stringWriter.Write("<option value=\"{0}\"{2}>{1}</option>", num, LocalizedStrings.GetHtmlEncoded(CalendarUtilities.nThList[i]), arg);
			}
			stringWriter.Write("</select>");
			stringWriter.Close();
			return stringWriter.ToString();
		}

		public static string BuildDayDropdownList(DaysOfWeek daysOfWeek, string selectName, string selectClass, string onChange)
		{
			string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
			DictionaryEntry[] array = new DictionaryEntry[]
			{
				new DictionaryEntry(127, LocalizedStrings.GetNonEncoded(696030412)),
				new DictionaryEntry(62, LocalizedStrings.GetNonEncoded(394490012)),
				new DictionaryEntry(65, LocalizedStrings.GetNonEncoded(1137128015)),
				new DictionaryEntry(1, dayNames[0]),
				new DictionaryEntry(2, dayNames[1]),
				new DictionaryEntry(4, dayNames[2]),
				new DictionaryEntry(8, dayNames[3]),
				new DictionaryEntry(16, dayNames[4]),
				new DictionaryEntry(32, dayNames[5]),
				new DictionaryEntry(64, dayNames[6])
			};
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				stringWriter.Write(CalendarUtilities.BuildSelectStartTag(selectName, selectClass, onChange));
				foreach (DictionaryEntry dictionaryEntry in array)
				{
					string arg = ((int)dictionaryEntry.Key == (int)daysOfWeek) ? " selected" : string.Empty;
					stringWriter.Write("<option value=\"{0}\"{2}>{1}</option>", ((int)dictionaryEntry.Key).ToString(CultureInfo.InvariantCulture), Utilities.HtmlEncode((string)dictionaryEntry.Value), arg);
				}
				stringWriter.Write("</select>");
				result = stringWriter.ToString();
			}
			return result;
		}

		public static string BuildMonthDropdownList(int selectedMonth, string selectName, string selectClass, string onChange)
		{
			string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				stringWriter.Write(CalendarUtilities.BuildSelectStartTag(selectName, selectClass, onChange));
				for (int i = 0; i < monthNames.Length; i++)
				{
					if (!string.IsNullOrEmpty(monthNames[i]))
					{
						int num = i + 1;
						string arg = (num == selectedMonth) ? " selected" : string.Empty;
						stringWriter.Write("<option value=\"{0}\"{2}>{1}</option>", num.ToString(CultureInfo.InvariantCulture), Utilities.HtmlEncode(monthNames[i]), arg);
					}
				}
				stringWriter.Write("</select>");
				result = stringWriter.ToString();
			}
			return result;
		}

		private static SanitizedHtmlString BuildSelectStartTag(string selectName, string selectClass, string onChange)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<select name=\"");
			sanitizingStringBuilder.Append(selectName);
			sanitizingStringBuilder.Append("\"");
			if (!string.IsNullOrEmpty(onChange))
			{
				sanitizingStringBuilder.Append(" onchange=\"");
				sanitizingStringBuilder.Append(onChange);
				sanitizingStringBuilder.Append("\"");
			}
			if (!string.IsNullOrEmpty(selectClass))
			{
				sanitizingStringBuilder.Append(" class=\"");
				sanitizingStringBuilder.Append(selectClass);
				sanitizingStringBuilder.Append("\"");
			}
			sanitizingStringBuilder.Append(">");
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		public static bool CheckIsLocationGenerated(CalendarItemBase calendarItemBase)
		{
			return CalendarUtilities.StringsEqualNullEmpty(calendarItemBase.Location, CalendarUtilities.GenerateLocation(calendarItemBase), StringComparison.CurrentCulture);
		}

		public static bool GenerateAndSetLocation(CalendarItemBase calendarItemBase)
		{
			string text = CalendarUtilities.GenerateLocation(calendarItemBase);
			if (!CalendarUtilities.StringsEqualNullEmpty(text, calendarItemBase.Location, StringComparison.CurrentCulture))
			{
				calendarItemBase.Location = text;
				return true;
			}
			return false;
		}

		public static string GenerateLocation(CalendarItemBase calendarItemBase)
		{
			string result = string.Empty;
			if (calendarItemBase != null && calendarItemBase.AttendeeCollection != null && calendarItemBase.AttendeeCollection.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Attendee attendee in calendarItemBase.AttendeeCollection)
				{
					if (attendee.AttendeeType == AttendeeType.Resource && attendee.Participant != null && !string.IsNullOrEmpty(attendee.Participant.DisplayName))
					{
						int num = stringBuilder.Length + ((stringBuilder.Length > 0) ? 2 : 0) + attendee.Participant.DisplayName.Length;
						if (num <= 255)
						{
							if (stringBuilder.Length > 0)
							{
								stringBuilder.Append("; ");
							}
							stringBuilder.Append(attendee.Participant.DisplayName);
						}
					}
				}
				if (stringBuilder.Length > 0)
				{
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		public static bool StringsEqualNullEmpty(string s1, string s2, StringComparison comp)
		{
			return string.IsNullOrEmpty(s1) == string.IsNullOrEmpty(s2) && ((string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) || string.Equals(s1, s2, comp));
		}

		public static AttendeeType RecipientTypeToAttendeeType(RecipientItemType recipientItemType)
		{
			switch (recipientItemType)
			{
			case RecipientItemType.To:
				return AttendeeType.Required;
			case RecipientItemType.Cc:
				return AttendeeType.Optional;
			case RecipientItemType.Bcc:
				return AttendeeType.Resource;
			}
			throw new OwaInvalidOperationException("Bad attendee item type.");
		}

		public static bool RemoveAttendeeByRecipientIdString(CalendarItemBase calendarItemBase, string idString)
		{
			RecipientItemType recipientItemType;
			int num;
			ItemRecipientWell.ParseRecipientIdString(idString, out recipientItemType, out num);
			AttendeeType attendeeType = CalendarUtilities.RecipientTypeToAttendeeType(recipientItemType);
			Attendee attendee = null;
			int num2 = 0;
			foreach (Attendee attendee2 in calendarItemBase.AttendeeCollection)
			{
				if (CalendarUtilities.IsExpectedTypeAttendee(attendee2, attendeeType))
				{
					if (num2 == num)
					{
						attendee = attendee2;
						break;
					}
					num2++;
				}
			}
			return attendee != null && CalendarUtilities.RemoveAttendee(calendarItemBase, attendee);
		}

		public static bool IsExpectedTypeAttendee(Attendee attendee, AttendeeType attendeeType)
		{
			return attendee.AttendeeType.Equals(attendeeType) && !attendee.IsOrganizer;
		}

		public static bool RemoveAttendee(CalendarItemBase calendarItemBase, Attendee attendee)
		{
			if (calendarItemBase != null && attendee != null && calendarItemBase.AttendeeCollection.Contains(attendee))
			{
				bool flag = false;
				if (attendee.AttendeeType == AttendeeType.Resource)
				{
					flag = CalendarUtilities.CheckIsLocationGenerated(calendarItemBase);
				}
				calendarItemBase.AttendeeCollection.Remove(attendee);
				if (flag || string.IsNullOrEmpty(calendarItemBase.Location))
				{
					CalendarUtilities.GenerateAndSetLocation(calendarItemBase);
				}
				return true;
			}
			return false;
		}

		public static bool RemoveAttendeeAt(CalendarItemBase calendarItemBase, int index)
		{
			if (calendarItemBase != null && index >= 0 && index < calendarItemBase.AttendeeCollection.Count)
			{
				bool flag = false;
				Attendee attendee = calendarItemBase.AttendeeCollection[index];
				if (attendee.AttendeeType == AttendeeType.Resource)
				{
					flag = CalendarUtilities.CheckIsLocationGenerated(calendarItemBase);
				}
				calendarItemBase.AttendeeCollection.RemoveAt(index);
				if (flag || string.IsNullOrEmpty(calendarItemBase.Location))
				{
					CalendarUtilities.GenerateAndSetLocation(calendarItemBase);
				}
				return true;
			}
			return false;
		}

		public static void AddResolvedAttendees(CalendarItemBase calendarItemBase, AttendeeType attendeeType, ResolvedRecipientDetail[] resolvedRecipientDetails, UserContext userContext)
		{
			if (calendarItemBase == null)
			{
				throw new ArgumentNullException("calendarItemBase");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (resolvedRecipientDetails == null)
			{
				throw new ArgumentNullException("resolvedRecipientDetails");
			}
			RecipientCache recipientCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
			for (int i = 0; i < resolvedRecipientDetails.Length; i++)
			{
				Participant participant = resolvedRecipientDetails[i].ToParticipant();
				if (!(participant == null))
				{
					CalendarUtilities.AddAttendee(calendarItemBase, participant, attendeeType);
					if (userContext.UserOptions.AddRecipientsToAutoCompleteCache && recipientCache != null)
					{
						recipientCache.AddEntry(resolvedRecipientDetails[i].DisplayName, resolvedRecipientDetails[i].SmtpAddress, resolvedRecipientDetails[i].RoutingAddress, string.Empty, resolvedRecipientDetails[i].RoutingType, resolvedRecipientDetails[i].AddressOrigin, resolvedRecipientDetails[i].RecipientFlags, resolvedRecipientDetails[i].ItemId, resolvedRecipientDetails[i].EmailAddressIndex);
					}
				}
			}
			if (recipientCache != null && recipientCache.IsDirty)
			{
				recipientCache.Commit(true);
			}
		}

		public static bool AddAttendee(CalendarItemBase calendarItemBase, Participant participant, AttendeeType type)
		{
			if (calendarItemBase != null && participant != null)
			{
				bool flag = false;
				if (type == AttendeeType.Resource)
				{
					flag = CalendarUtilities.CheckIsLocationGenerated(calendarItemBase);
				}
				calendarItemBase.AttendeeCollection.Add(participant, type, null, null, false);
				if (flag || string.IsNullOrEmpty(calendarItemBase.Location))
				{
					CalendarUtilities.GenerateAndSetLocation(calendarItemBase);
				}
				return true;
			}
			return false;
		}

		internal static bool SaveCalendarItem(CalendarItemBase calendarItemBase, UserContext userContext, out InfobarMessage infobarMessage)
		{
			infobarMessage = null;
			LocalizedException ex = null;
			string text = null;
			if (!CalendarUtilities.ValidateCalendarItemBase(userContext, calendarItemBase, out infobarMessage))
			{
				return false;
			}
			try
			{
				text = CalendarUtilities.SaveItem(calendarItemBase);
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				infobarMessage = InfobarMessage.CreateErrorMessageFromException(ex, userContext);
			}
			else if (text != null)
			{
				infobarMessage = InfobarMessage.CreateText(text, InfobarMessageType.Error);
			}
			return infobarMessage == null;
		}

		private static string SaveItem(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			bool flag = item.Id != null;
			ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
			item.Load();
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				if (ExTraceGlobals.CalendarTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.CalendarTracer.TraceDebug(0L, "Saving item failed due to conflict resolution.");
				}
				return LocalizedStrings.GetNonEncoded(-482397486);
			}
			if (Globals.ArePerfCountersEnabled)
			{
				if (flag)
				{
					OwaSingleCounters.ItemsUpdated.Increment();
				}
				else
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
			}
			return null;
		}

		internal static bool SendMeetingMessages(UserContext userContext, CalendarItemBase calendarItemBase, bool sendToAll, out InfobarMessage infobarMessage)
		{
			bool result = true;
			infobarMessage = null;
			if (!CalendarUtilities.ValidateCalendarItemBase(userContext, calendarItemBase, out infobarMessage))
			{
				return false;
			}
			if (!calendarItemBase.IsOrganizer())
			{
				throw new OwaEventHandlerException("Only meeting organizer can send meeting invite");
			}
			LocalizedException ex = null;
			try
			{
				calendarItemBase.SendMeetingMessages(sendToAll, null, false, true, null, null);
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				infobarMessage = InfobarMessage.CreateErrorMessageFromException(ex, userContext);
				result = false;
			}
			return result;
		}

		public static StoreObjectId GetMasterStoreObjectId(CalendarItemBase calendarItem)
		{
			return StoreObjectId.FromProviderSpecificId(calendarItem.Id.ObjectId.ProviderLevelItemId);
		}

		public static bool IsCalendarItemDirty(CalendarItemBase calendarItemBase, UserContext userContext)
		{
			bool result = false;
			if (calendarItemBase.Id != null)
			{
				result = calendarItemBase.IsDirty;
			}
			else
			{
				CalendarItem calendarItem = calendarItemBase as CalendarItem;
				if (calendarItemBase.AttachmentCollection.Count > 0)
				{
					result = true;
				}
				else if (calendarItemBase.AttendeeCollection.Count > 0)
				{
					result = true;
				}
				else if (calendarItemBase.Body.IsBodyChanged)
				{
					result = true;
				}
				else if (calendarItemBase.FreeBusyStatus != BusyType.Busy)
				{
					result = true;
				}
				else if (calendarItemBase.Importance != Importance.Normal)
				{
					result = true;
				}
				else if (calendarItemBase.IsAllDayEvent)
				{
					result = true;
				}
				else if (calendarItemBase.IsCancelled)
				{
					result = true;
				}
				else if (!string.IsNullOrEmpty(calendarItemBase.Location))
				{
					result = true;
				}
				else if (calendarItemBase.Sensitivity != Sensitivity.Normal)
				{
					result = true;
				}
				else if (!string.IsNullOrEmpty(calendarItemBase.Subject))
				{
					result = true;
				}
				else if (!(bool)calendarItemBase[ItemSchema.IsResponseRequested])
				{
					result = true;
				}
				else if (calendarItem != null && calendarItem.Recurrence != null)
				{
					result = true;
				}
				else
				{
					ExDateTime d = DateTimeUtilities.GetLocalTime();
					CalendarModuleViewState calendarModuleViewState = userContext.LastClientViewState as CalendarModuleViewState;
					if (calendarModuleViewState != null)
					{
						d = new ExDateTime(userContext.TimeZone, calendarModuleViewState.DateTime.Year, calendarModuleViewState.DateTime.Month, calendarModuleViewState.DateTime.Day, d.Hour, d.Minute, 0);
					}
					if (d.Minute != 0 && d.Minute != 30)
					{
						d = d.AddMinutes((double)(30 - d.Minute % 30));
					}
					ExDateTime d2 = d.AddMinutes(60.0);
					if (calendarItemBase.StartTime != d || calendarItemBase.EndTime != d2)
					{
						result = true;
					}
				}
			}
			return result;
		}

		internal static bool ValidateCalendarItemBase(UserContext userContext, CalendarItemBase calendarItemBase, out InfobarMessage infobarMessage)
		{
			infobarMessage = null;
			LocalizedException ex = null;
			if (calendarItemBase == null)
			{
				throw new ArgumentNullException("calendarItemBase");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.MailboxSession == null)
			{
				throw new ArgumentNullException("userContext", "userContext.MailboxSession is null");
			}
			try
			{
				calendarItemBase.Validate();
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				infobarMessage = InfobarMessage.CreateErrorMessageFromException(ex, userContext);
			}
			return infobarMessage == null;
		}

		private const string OptionFormatString = "<option value=\"{1}\"{0}>{2}</option>";

		private static readonly Strings.IDs[] nThList = new Strings.IDs[]
		{
			-555757312,
			-1339960366,
			869183319,
			1031963858,
			49675370
		};
	}
}
