using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class EditRecurrencePreFormAction : IPreFormAction
	{
		public static bool CheckNeedPatternChangeConfirmation(UserContext userContext, CalendarItemData calendarItemData)
		{
			Recurrence recurrence = null;
			CalendarItemBaseData userContextData = EditCalendarItemHelper.GetUserContextData(userContext);
			CalendarItemData calendarItemData2 = userContextData as CalendarItemData;
			if (calendarItemData2 != null)
			{
				recurrence = calendarItemData2.Recurrence;
			}
			RecurrencePattern p = null;
			if (recurrence != null)
			{
				p = recurrence.Pattern;
			}
			bool flag = !CalendarItemData.IsRecurrencePatternEqual(calendarItemData.Recurrence.Pattern, p);
			bool flag2 = false;
			if (flag && calendarItemData.Id != null)
			{
				using (CalendarItem item = Utilities.GetItem<CalendarItem>(userContext, calendarItemData.Id, new PropertyDefinition[0]))
				{
					if (item != null && item.Recurrence != null)
					{
						IList<OccurrenceInfo> modifiedOccurrences = item.Recurrence.GetModifiedOccurrences();
						if (modifiedOccurrences != null)
						{
							flag2 = (modifiedOccurrences.Count > 0);
						}
					}
				}
			}
			return flag && flag2;
		}

		public static Recurrence ChangeRecurrenceType(Recurrence recurrence, OwaRecurrenceType newRecurrenceType)
		{
			OwaRecurrenceType owaRecurrenceType = CalendarUtilities.MapRecurrenceType(recurrence);
			if (owaRecurrenceType == newRecurrenceType)
			{
				return recurrence;
			}
			RecurrencePattern pattern = CalendarUtilities.CreateDefaultRecurrencePattern(newRecurrenceType, recurrence.Range.StartDate);
			RecurrenceRange range = CalendarItemData.CloneRecurrenceRange(recurrence.Range);
			Recurrence result;
			if (recurrence.CreatedExTimeZone != ExTimeZone.UtcTimeZone && recurrence.ReadExTimeZone != ExTimeZone.UtcTimeZone)
			{
				result = new Recurrence(pattern, range, recurrence.CreatedExTimeZone, recurrence.ReadExTimeZone);
			}
			else
			{
				result = new Recurrence(pattern, range);
			}
			return result;
		}

		internal static bool OwaRecurrenceTypeIsValid(int owaRecurrenceType)
		{
			return owaRecurrenceType != 0 && (owaRecurrenceType & -32544) == 0;
		}

		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext", "owaContext is null");
			}
			if (owaContext.HttpContext == null)
			{
				throw new ArgumentNullException("owaContext", "owaContext.HttpContext is null");
			}
			if (owaContext.HttpContext.Request == null)
			{
				throw new ArgumentNullException("owaContext", "owaContext.HttpContext.Request is null");
			}
			applicationElement = ApplicationElement.NotSet;
			type = null;
			state = null;
			action = null;
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			preFormActionResponse.Type = "IPM.Appointment";
			preFormActionResponse.Action = string.Empty;
			preFormActionResponse.State = string.Empty;
			this.request = owaContext.HttpContext.Request;
			this.userContext = owaContext.UserContext;
			if (!Utilities.IsPostRequest(this.request))
			{
				return this.userContext.LastClientViewState.ToPreFormActionResponse();
			}
			InfobarMessage value = null;
			string queryStringParameter = Utilities.GetQueryStringParameter(this.request, "a", false);
			CalendarItemData calendarItemData = EditRecurrencePreFormAction.CreateCalendarItemDataFromRequest(this.request, this.userContext);
			bool flag = false;
			string a;
			EditRecurrencePreFormAction.RedirectTo redirectTo;
			if ((a = queryStringParameter) != null)
			{
				if (a == "ChangeRecurrenceMode")
				{
					if (!string.IsNullOrEmpty(Utilities.GetQueryStringParameter(this.request, "d", false)))
					{
						preFormActionResponse.AddParameter("d", "1");
					}
					string formParameter = Utilities.GetFormParameter(this.request, "hidcidrt", true);
					int num;
					if (!string.IsNullOrEmpty(formParameter) && int.TryParse(formParameter, out num) && num > 0)
					{
						preFormActionResponse.AddParameter("cd", "1");
					}
					redirectTo = EditRecurrencePreFormAction.RedirectTo.EditRecurrence;
					goto IL_217;
				}
				if (a == "CloseRecurrence")
				{
					redirectTo = EditRecurrencePreFormAction.RedirectTo.EditCalendarItem;
					goto IL_217;
				}
				if (a == "RemoveRecurrence")
				{
					calendarItemData.Recurrence = null;
					flag = true;
					redirectTo = EditRecurrencePreFormAction.RedirectTo.EditCalendarItem;
					goto IL_217;
				}
				if (!(a == "SaveRecurrence"))
				{
					if (a == "ConfirmedSaveRecurrence")
					{
						flag = true;
						redirectTo = EditRecurrencePreFormAction.RedirectTo.EditCalendarItem;
						goto IL_217;
					}
				}
				else
				{
					bool flag2 = EditRecurrencePreFormAction.ValidateCalendarItemData(this.userContext, calendarItemData, out value);
					if (!flag2)
					{
						redirectTo = EditRecurrencePreFormAction.RedirectTo.EditRecurrence;
						goto IL_217;
					}
					if (EditRecurrencePreFormAction.CheckNeedPatternChangeConfirmation(this.userContext, calendarItemData))
					{
						redirectTo = EditRecurrencePreFormAction.RedirectTo.EditRecurrence;
						preFormActionResponse.AddParameter("pcp", "1");
						goto IL_217;
					}
					flag = true;
					redirectTo = EditRecurrencePreFormAction.RedirectTo.EditCalendarItem;
					goto IL_217;
				}
			}
			redirectTo = EditRecurrencePreFormAction.RedirectTo.EditRecurrence;
			IL_217:
			if (flag)
			{
				CalendarItemData calendarItemData2 = EditCalendarItemHelper.GetUserContextData(this.userContext) as CalendarItemData;
				if (calendarItemData2 == null)
				{
					throw new OwaInvalidRequestException("UserContext didn't have a CalendarItemData object.");
				}
				calendarItemData2.Recurrence = calendarItemData.Recurrence;
				if (calendarItemData.Recurrence != null)
				{
					calendarItemData2.StartTime = calendarItemData.StartTime;
					calendarItemData2.EndTime = calendarItemData.EndTime;
					calendarItemData2.IsAllDayEvent = calendarItemData.IsAllDayEvent;
				}
			}
			switch (redirectTo)
			{
			case EditRecurrencePreFormAction.RedirectTo.None:
				throw new OwaInvalidRequestException("Unhandled redirection in EditRecurrencePreFormAction.");
			case EditRecurrencePreFormAction.RedirectTo.EditRecurrence:
				owaContext.PreFormActionData = calendarItemData;
				preFormActionResponse.Action = "EditRecurrence";
				break;
			case EditRecurrencePreFormAction.RedirectTo.EditCalendarItem:
				preFormActionResponse.Action = "Open";
				if (calendarItemData.Id != null)
				{
					owaContext.PreFormActionId = OwaStoreObjectId.CreateFromMailboxItemId(calendarItemData.Id);
				}
				break;
			default:
				throw new OwaInvalidRequestException("Unhandled redirection enum value in EditRecurrencePreFormAction.");
			}
			owaContext[OwaContextProperty.InfobarMessage] = value;
			return preFormActionResponse;
		}

		private static bool ValidateCalendarItemData(UserContext userContext, CalendarItemData calendarItemData, out InfobarMessage infobarMessage)
		{
			infobarMessage = null;
			if (calendarItemData == null)
			{
				throw new ArgumentNullException("calendarItemData");
			}
			if (calendarItemData.FolderId == null)
			{
				throw new ArgumentNullException("calendarItemData", "calendarItemData.FolderId is null");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.MailboxSession == null)
			{
				throw new ArgumentNullException("userContext", "userContext.MailboxSession is null");
			}
			using (CalendarItem calendarItem = CalendarItem.Create(userContext.MailboxSession, userContext.CalendarFolderId))
			{
				LocalizedException ex = null;
				try
				{
					calendarItemData.CopyTo(calendarItem);
					calendarItem.Validate();
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
					ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(ex, userContext.MailboxIdentity);
					string messageText = exceptionHandlingInformation.Message;
					if (ex is CorruptDataException && calendarItemData.EndTime < calendarItemData.StartTime)
					{
						messageText = LocalizedStrings.GetNonEncoded(2047223147);
					}
					infobarMessage = InfobarMessage.CreateText(messageText, InfobarMessageType.Error);
				}
			}
			return infobarMessage == null;
		}

		private static CalendarItemData CreateCalendarItemDataFromRequest(HttpRequest request, UserContext userContext)
		{
			CalendarItemData calendarItemData = new CalendarItemData();
			string formParameter = Utilities.GetFormParameter(request, "hidid");
			string formParameter2 = Utilities.GetFormParameter(request, "hidfid");
			try
			{
				if (!string.IsNullOrEmpty(formParameter))
				{
					calendarItemData.Id = StoreObjectId.Deserialize(formParameter);
				}
				else
				{
					calendarItemData.Id = null;
				}
				if (!string.IsNullOrEmpty(formParameter2))
				{
					calendarItemData.FolderId = StoreObjectId.Deserialize(formParameter2);
				}
				else
				{
					calendarItemData.FolderId = null;
				}
			}
			catch (ArgumentException)
			{
				throw new OwaInvalidRequestException("Invalid store object id");
			}
			catch (FormatException)
			{
				throw new OwaInvalidRequestException("Invalid store object id");
			}
			ExDateTime startDate = CalendarUtilities.ParseDateTimeFromForm(request, "selSY", "selSM", "selSD", null, userContext);
			TimeSpan value = CalendarUtilities.ParseTimeFromForm(request, "sttm");
			int intValueFromForm = RequestParser.GetIntValueFromForm(request, "drtn");
			calendarItemData.StartTime = startDate.Add(value);
			calendarItemData.EndTime = calendarItemData.StartTime.AddMinutes((double)intValueFromForm);
			if (calendarItemData.EndTime < calendarItemData.StartTime)
			{
				calendarItemData.EndTime = calendarItemData.StartTime.AddHours(1.0);
			}
			calendarItemData.Subject = Utilities.GetFormParameter(request, "hidsubj", false);
			calendarItemData.Location = Utilities.GetFormParameter(request, "hidloc", false);
			calendarItemData.Recurrence = EditRecurrencePreFormAction.CreateRecurrenceFromRequest(request, startDate, userContext);
			if (calendarItemData.Recurrence != null)
			{
				calendarItemData.IsAllDayEvent = Utilities.IsAllDayEvent(calendarItemData.StartTime, calendarItemData.EndTime);
			}
			return calendarItemData;
		}

		private static Recurrence CreateRecurrenceFromRequest(HttpRequest request, ExDateTime startDate, UserContext userContext)
		{
			OwaRecurrenceType newRecurrenceTypeFromPost = EditRecurrencePreFormAction.GetNewRecurrenceTypeFromPost(request);
			RecurrencePattern pattern = null;
			Recurrence result = null;
			DaysOfWeek defaultDaysOfWeek = CalendarUtilities.ConvertDateTimeToDaysOfWeek(startDate);
			int defaultValue = CalendarUtilities.ComputeDayOfMonthOrder(startDate);
			OwaRecurrenceType owaRecurrenceType = newRecurrenceTypeFromPost;
			if (owaRecurrenceType <= OwaRecurrenceType.Yearly)
			{
				switch (owaRecurrenceType)
				{
				case OwaRecurrenceType.Daily:
				{
					int num = EditRecurrencePreFormAction.GetIntFormParameter(request, "txtinterval", 1);
					num = Math.Max(1, num);
					pattern = new DailyRecurrencePattern(num);
					break;
				}
				case OwaRecurrenceType.None | OwaRecurrenceType.Daily:
					break;
				case OwaRecurrenceType.Weekly:
				{
					int num = EditRecurrencePreFormAction.GetIntFormParameter(request, "txtinterval", 1);
					num = Math.Max(1, num);
					pattern = new WeeklyRecurrencePattern(EditRecurrencePreFormAction.ParseDayCheckboxes(request, defaultDaysOfWeek), num);
					break;
				}
				default:
					if (owaRecurrenceType != OwaRecurrenceType.Monthly)
					{
						if (owaRecurrenceType == OwaRecurrenceType.Yearly)
						{
							int num2 = EditRecurrencePreFormAction.GetIntFormParameter(request, "selRcrYD", startDate.Day);
							int intFormParameter = EditRecurrencePreFormAction.GetIntFormParameter(request, "selRcrYM", startDate.Month);
							num2 = Math.Min(ExDateTime.DaysInMonth(startDate.Year, intFormParameter), num2);
							pattern = new YearlyRecurrencePattern(num2, intFormParameter);
						}
					}
					else
					{
						int num2 = EditRecurrencePreFormAction.GetIntFormParameter(request, "txtRcrMD", startDate.Day);
						int num = EditRecurrencePreFormAction.GetIntFormParameter(request, "txtRcrMM", 1);
						num = Math.Max(1, num);
						pattern = new MonthlyRecurrencePattern(num2, num);
					}
					break;
				}
			}
			else if (owaRecurrenceType != (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyEveryWeekday))
			{
				if (owaRecurrenceType != (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh))
				{
					if (owaRecurrenceType == (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh))
					{
						int intFormParameter2 = EditRecurrencePreFormAction.GetIntFormParameter(request, "selRcrYTI", defaultValue);
						int intFormParameter = EditRecurrencePreFormAction.GetIntFormParameter(request, "selRcrYTM", startDate.Month);
						DaysOfWeek daysOfWeek = EditRecurrencePreFormAction.ParseDaysOfWeek(request, "selRcrThD", defaultDaysOfWeek);
						pattern = new YearlyThRecurrencePattern(daysOfWeek, (RecurrenceOrderType)intFormParameter2, intFormParameter);
					}
				}
				else
				{
					int intFormParameter2 = EditRecurrencePreFormAction.GetIntFormParameter(request, "selRcrYTI", defaultValue);
					int num = EditRecurrencePreFormAction.GetIntFormParameter(request, "txtRcrMThM", 1);
					num = Math.Max(1, num);
					DaysOfWeek daysOfWeek = EditRecurrencePreFormAction.ParseDaysOfWeek(request, "selRcrThD", defaultDaysOfWeek);
					pattern = new MonthlyThRecurrencePattern(daysOfWeek, (RecurrenceOrderType)intFormParameter2, num);
				}
			}
			else
			{
				pattern = new WeeklyRecurrencePattern(DaysOfWeek.Weekdays);
			}
			if (newRecurrenceTypeFromPost != OwaRecurrenceType.None)
			{
				if (startDate == ExDateTime.MinValue)
				{
					startDate = CalendarUtilities.ParseDateTimeFromForm(request, "selSY", "selSM", "selSD", null, userContext);
				}
				RecurrenceRange range;
				switch (EditRecurrencePreFormAction.GetRecurrenceRangeTypeFromPost(request))
				{
				case RecurrenceRangeType.Numbered:
				{
					int num3 = EditRecurrencePreFormAction.GetIntFormParameter(request, "txtno", 10);
					num3 = Math.Max(1, num3);
					range = new NumberedRecurrenceRange(startDate, num3);
					goto IL_284;
				}
				case RecurrenceRangeType.EndDate:
				{
					ExDateTime exDateTime = CalendarUtilities.ParseDateTimeFromForm(request, "selEY", "selEM", "selED", null, userContext);
					if (exDateTime < startDate)
					{
						exDateTime = startDate.IncrementDays(10);
					}
					range = new EndDateRecurrenceRange(startDate, exDateTime);
					goto IL_284;
				}
				}
				range = new NoEndRecurrenceRange(startDate);
				IL_284:
				result = new Recurrence(pattern, range);
			}
			return result;
		}

		private static DaysOfWeek ParseDaysOfWeek(HttpRequest request, string selectName, DaysOfWeek defaultDaysOfWeek)
		{
			string formParameter = Utilities.GetFormParameter(request, selectName, false);
			if (!string.IsNullOrEmpty(formParameter))
			{
				return (DaysOfWeek)int.Parse(formParameter);
			}
			return defaultDaysOfWeek;
		}

		private static DaysOfWeek ParseDayCheckboxes(HttpRequest request, DaysOfWeek defaultDaysOfWeek)
		{
			DaysOfWeek daysOfWeek = DaysOfWeek.None;
			for (int i = 0; i < 7; i++)
			{
				int num = 1 << i;
				string name = "chkRcrW" + num.ToString(CultureInfo.InvariantCulture);
				if (Utilities.GetFormParameter(request, name, false) != null)
				{
					daysOfWeek |= (DaysOfWeek)num;
				}
			}
			if (daysOfWeek == DaysOfWeek.None)
			{
				daysOfWeek = defaultDaysOfWeek;
			}
			return daysOfWeek;
		}

		private static RecurrenceRangeType GetRecurrenceRangeTypeFromPost(HttpRequest request)
		{
			RecurrenceRangeType result = RecurrenceRangeType.NoEnd;
			string formParameter = Utilities.GetFormParameter(request, "rdrng", false);
			if (!string.IsNullOrEmpty(formParameter))
			{
				result = (RecurrenceRangeType)int.Parse(formParameter);
			}
			return result;
		}

		private static OwaRecurrenceType ParseRecurrenceType(string recurrenceTypeString)
		{
			if (string.IsNullOrEmpty(recurrenceTypeString))
			{
				throw new OwaInvalidRequestException("Null or empty recurrence core type in form parameter.");
			}
			int num;
			if (!int.TryParse(recurrenceTypeString, out num))
			{
				throw new OwaInvalidRequestException("Non-integer recurrence core type in form parameter.");
			}
			if (EditRecurrencePreFormAction.OwaRecurrenceTypeIsValid(num))
			{
				return (OwaRecurrenceType)num;
			}
			throw new OwaInvalidRequestException("Invalid recurrence type integer value, doesn't map to a OwaRecurrenceType.");
		}

		private static OwaRecurrenceType GetNewRecurrenceTypeFromPost(HttpRequest request)
		{
			string formParameter = Utilities.GetFormParameter(request, "rdtype", false);
			OwaRecurrenceType owaRecurrenceType = EditRecurrencePreFormAction.ParseRecurrenceType(formParameter);
			OwaRecurrenceType owaRecurrenceType2 = owaRecurrenceType;
			if (owaRecurrenceType2 != OwaRecurrenceType.Daily)
			{
				if (owaRecurrenceType2 != OwaRecurrenceType.Monthly)
				{
					if (owaRecurrenceType2 == OwaRecurrenceType.Yearly)
					{
						string formParameter2 = Utilities.GetFormParameter(request, "rdyrtype", false);
						if (string.Equals(formParameter2, "1"))
						{
							owaRecurrenceType |= OwaRecurrenceType.YearlyTh;
						}
					}
				}
				else
				{
					string formParameter2 = Utilities.GetFormParameter(request, "rdmotype", false);
					if (string.Equals(formParameter2, "1"))
					{
						owaRecurrenceType |= OwaRecurrenceType.MonthlyTh;
					}
				}
			}
			else
			{
				string formParameter2 = Utilities.GetFormParameter(request, "rddytype", false);
				if (string.Equals(formParameter2, "1"))
				{
					owaRecurrenceType |= OwaRecurrenceType.DailyEveryWeekday;
				}
			}
			return owaRecurrenceType;
		}

		private static int GetIntFormParameter(HttpRequest request, string paramName, int defaultValue)
		{
			int result = defaultValue;
			string formParameter = Utilities.GetFormParameter(request, paramName, false);
			if (!string.IsNullOrEmpty(formParameter))
			{
				int.TryParse(formParameter, out result);
			}
			return result;
		}

		private UserContext userContext;

		private HttpRequest request;

		private enum RedirectTo
		{
			None,
			EditRecurrence,
			EditCalendarItem
		}
	}
}
