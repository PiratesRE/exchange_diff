using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public static class RemindersRenderingUtilities
	{
		internal static bool GetReminderItems(ExDateTime actualizationTime, TextWriter output)
		{
			UserContext userContext = UserContextManager.GetUserContext();
			return RemindersRenderingUtilities.GetReminderItemsInternal(userContext, actualizationTime, output);
		}

		internal static bool GetReminderItems(UserContext userContext, ExDateTime actualizationTime, TextWriter output)
		{
			return RemindersRenderingUtilities.GetReminderItemsInternal(userContext, actualizationTime, output);
		}

		internal static bool GetReminderItemsInternal(UserContext userContext, ExDateTime actualizationTime, TextWriter output)
		{
			bool result = false;
			string name = userContext.UserCulture.Name;
			List<object[]> list = RemindersRenderingUtilities.QueryReminders(actualizationTime, userContext);
			if (list.Count == 0)
			{
				return false;
			}
			foreach (object[] reminder in list)
			{
				ExDateTime exDateTime = RemindersRenderingUtilities.GetItemProperty<ExDateTime>(reminder, ItemSchema.ReminderDueBy, ExDateTime.MinValue);
				ExDateTime exDateTime2 = RemindersRenderingUtilities.GetItemProperty<ExDateTime>(reminder, ItemSchema.ReminderNextTime, ExDateTime.MinValue);
				if (!(exDateTime == ExDateTime.MinValue) && !(exDateTime2 == ExDateTime.MinValue) && !(exDateTime2 == RemindersRenderingUtilities.MaxOutlookDate))
				{
					string itemProperty = RemindersRenderingUtilities.GetItemProperty<string>(reminder, StoreObjectSchema.ItemClass, string.Empty);
					bool itemProperty2 = RemindersRenderingUtilities.GetItemProperty<bool>(reminder, MessageItemSchema.IsDraft, false);
					int itemProperty3 = RemindersRenderingUtilities.GetItemProperty<int>(reminder, MessageItemSchema.SwappedToDoStore, -1);
					if (itemProperty.IndexOf("IPM.Schedule.Meeting", StringComparison.OrdinalIgnoreCase) == -1 && (itemProperty.IndexOf("IPM.Note", StringComparison.OrdinalIgnoreCase) == -1 || !itemProperty2) && itemProperty3 == -1)
					{
						ExDateTime itemProperty4 = RemindersRenderingUtilities.GetItemProperty<ExDateTime>(reminder, CalendarItemInstanceSchema.StartTime, ExDateTime.MinValue);
						ExDateTime itemProperty5 = RemindersRenderingUtilities.GetItemProperty<ExDateTime>(reminder, CalendarItemInstanceSchema.EndTime, ExDateTime.MinValue);
						int num = (int)(itemProperty5 - itemProperty4).TotalMinutes;
						if (num < 0)
						{
							num = 0;
						}
						bool flag = ObjectClass.IsCalendarItem(itemProperty);
						VersionedId itemProperty6 = RemindersRenderingUtilities.GetItemProperty<VersionedId>(reminder, ItemSchema.Id);
						bool flag2 = false;
						if (flag)
						{
							CalendarItemType itemProperty7 = RemindersRenderingUtilities.GetItemProperty<CalendarItemType>(reminder, CalendarItemBaseSchema.CalendarItemType, CalendarItemType.Single);
							flag2 = (itemProperty7 == CalendarItemType.RecurringMaster);
						}
						VersionedId versionedId = null;
						string text;
						string text2;
						if (flag2)
						{
							CalendarItemOccurrence calendarItemOccurrence = null;
							Item item = null;
							try
							{
								try
								{
									item = Utilities.GetItem<Item>(userContext, itemProperty6.ObjectId, new PropertyDefinition[0]);
									if (item.Reminder != null)
									{
										calendarItemOccurrence = (item.Reminder.GetPertinentItem(actualizationTime) as CalendarItemOccurrence);
									}
									if (calendarItemOccurrence == null)
									{
										continue;
									}
									object obj = calendarItemOccurrence.Reminder.DueBy;
									object obj2 = calendarItemOccurrence.Reminder.ReminderNextTime;
									if (obj == null || obj2 == null)
									{
										continue;
									}
									exDateTime2 = (ExDateTime)obj2;
									exDateTime = (ExDateTime)obj;
									versionedId = calendarItemOccurrence.Id;
									text = calendarItemOccurrence.Subject;
									text2 = calendarItemOccurrence.Location;
								}
								catch (StoragePermanentException ex)
								{
									ExTraceGlobals.CalendarDataTracer.TraceDebug<string>(0L, "Unable to retrieve calendar item occurence for this reminder.  Exception: {0}", ex.Message);
									result = true;
									continue;
								}
								catch (StorageTransientException ex2)
								{
									ExTraceGlobals.CalendarDataTracer.TraceDebug<string>(0L, "Unable to retrieve calendar item occurence for this reminder.  Exception: {0}", ex2.Message);
									result = true;
									continue;
								}
								goto IL_289;
							}
							finally
							{
								if (item != null)
								{
									item.Dispose();
									item = null;
								}
								if (calendarItemOccurrence != null)
								{
									calendarItemOccurrence.Dispose();
									calendarItemOccurrence = null;
								}
							}
							goto IL_26D;
						}
						goto IL_26D;
						IL_289:
						using (StringWriter stringWriter = new StringWriter())
						{
							stringWriter.Write("<div _lnk=1 class=\"divNotificationsItem\" id=\"b");
							Utilities.HtmlEncode(itemProperty6.ObjectId.ToBase64String(), stringWriter);
							stringWriter.Write("\" ck=\"");
							Utilities.HtmlEncode(itemProperty6.ChangeKeyAsBase64String(), stringWriter);
							if (flag2)
							{
								stringWriter.Write("\" oid=\"");
								Utilities.HtmlEncode(versionedId.ObjectId.ToBase64String(), stringWriter);
							}
							stringWriter.Write("\" t=\"");
							Utilities.HtmlEncode(DateTimeUtilities.GetJavascriptDate(exDateTime), stringWriter);
							stringWriter.Write("\" c=\"");
							Utilities.HtmlEncode(itemProperty, stringWriter);
							stringWriter.Write("\" r=\"");
							Utilities.HtmlEncode(DateTimeUtilities.GetJavascriptDate(exDateTime2), stringWriter);
							stringWriter.Write("\" d=\"");
							stringWriter.Write(num);
							SanitizedHtmlString sanitizedHtmlString = null;
							if (!string.IsNullOrEmpty(text))
							{
								sanitizedHtmlString = Utilities.SanitizeHtmlEncode(text);
							}
							SanitizedHtmlString sanitizedHtmlString2 = null;
							if (!string.IsNullOrEmpty(text2))
							{
								sanitizedHtmlString2 = Utilities.SanitizeHtmlEncode(text2);
							}
							stringWriter.Write("\" subj=\"");
							stringWriter.Write(sanitizedHtmlString);
							stringWriter.Write("\" loc=\"");
							stringWriter.Write(sanitizedHtmlString2);
							string text3 = string.Format(userContext.UserCulture, LocalizedStrings.GetHtmlEncodedFromKey(name, 580192567), new object[]
							{
								Utilities.HtmlEncode(exDateTime.ToString(DateTimeFormatInfo.CurrentInfo.LongDatePattern)),
								Utilities.HtmlEncode(exDateTime.ToString(userContext.UserOptions.TimeFormat))
							});
							stringWriter.Write("\" due=\"");
							stringWriter.Write(text3);
							stringWriter.Write("\" tabindex=0>");
							SmallIconManager.RenderItemIcon(stringWriter, userContext, itemProperty, false, -1, string.Empty, new string[]
							{
								"id=\"imgNtfyEnt\""
							});
							stringWriter.Write("<DIV id=\"c3\" class=\"divNotificationsColumn3\"><div id=\"c3Time\" class=\"divRemindersDueInTime\"></div><div id=\"c3Text\" class=\"divRemindersDueInText\"></div></div>");
							stringWriter.Write("<DIV id=\"c2\" class=\"divNotificationsColumn2\"><div id=\"c2Time\" class=\"divRemindersDueInTime\"></div><div id=\"c2Text\" class=\"divRemindersDueInText\"></div></div>");
							stringWriter.Write("<DIV id=\"c1\" class=\"divNotificationsColumn1\"><div id=\"c1Row1\" class=\"divNotificationsColumn1Row1\">{0}</div><div id=\"c1Row2\" class=\"divNotificationsColumn1Row2\">{1}</div>", (sanitizedHtmlString != null) ? sanitizedHtmlString.ToString() : string.Empty, text3);
							if (sanitizedHtmlString2 != null)
							{
								stringWriter.Write("<div id=\"c1Row3\" class=\"divNotificationsColumn1Row3\">");
								stringWriter.Write(sanitizedHtmlString2);
								stringWriter.Write("</div>");
							}
							stringWriter.Write("</div></div>");
							Utilities.JavascriptEncode(stringWriter.ToString(), output);
						}
						continue;
						IL_26D:
						text = RemindersRenderingUtilities.GetItemProperty<string>(reminder, ItemSchema.Subject, null);
						text2 = RemindersRenderingUtilities.GetItemProperty<string>(reminder, CalendarItemBaseSchema.Location, null);
						goto IL_289;
					}
				}
			}
			return result;
		}

		public static void RenderRemindersMenu(UserContext userContext, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Culture.SingularPluralRegularExpression singularPluralRegularExpressions = Culture.GetSingularPluralRegularExpressions(Thread.CurrentThread.CurrentUICulture.LCID);
			StringBuilder stringBuilder = new StringBuilder("re1=\"");
			stringBuilder.Append(singularPluralRegularExpressions.SingularExpression);
			stringBuilder.Append("\" re2=\"");
			stringBuilder.Append(singularPluralRegularExpressions.PluralExpression);
			stringBuilder.Append("\" fSoundEnabled=\"");
			stringBuilder.Append(userContext.UserOptions.EnableReminderSound ? "1" : "0");
			int value = (int)DateTimeUtilities.GetLocalTime().Bias.TotalMinutes;
			stringBuilder.Append("\" iTimeOffset=\"");
			stringBuilder.Append(value);
			stringBuilder.Append("\" rgTU=\"");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-632713499));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(1401635374));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-2005924231));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-993448055));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(2108503220));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-713621139));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(527281145));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-1099158712));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(845856285));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(1605214999));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-242484566));
			stringBuilder.Append(",");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(2014322819));
			stringBuilder.Append("\" sNow=\"");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(367075512));
			stringBuilder.Append("\"");
			stringBuilder.Append("\" sOverdue=\"");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(570999474));
			stringBuilder.Append("\"");
			StringBuilder stringBuilder2 = new StringBuilder();
			userContext.RenderThemeImage(stringBuilder2, ThemeFileId.ShadowDivLeft, "shadow-div-left", new object[0]);
			stringBuilder2.Append("<div class=\"shadow-div-tile\"></div>");
			userContext.RenderThemeImage(stringBuilder2, ThemeFileId.ShadowDivRight, "shadow-div-right", new object[0]);
			stringBuilder2.Append("<DIV class=\"divRemindersSnoozeGradient\"><div class=\"divRemindersSnoozeRegion\"><a id=\"divBtnSnooze\" class=\"aRemindersSnoozeButton\">");
			stringBuilder2.Append(LocalizedStrings.GetHtmlEncoded(-1327887096));
			stringBuilder2.Append("</a>");
			StringBuilder stringBuilder3 = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder3))
			{
				RemindersRenderingUtilities.RenderSnoozeDropDownList(stringWriter);
			}
			stringBuilder2.Append(stringBuilder3.ToString());
			stringBuilder2.Append("</div></div>");
			NotificationRenderingUtilities.RenderNotificationMenu(userContext, output, "divRmd", stringBuilder.ToString(), 1299349449, -1018465893, "divBtnDismissall", -1788659265, "divBtnDismiss", 1178588418, "divBtnOpenReminder", -669845619, stringBuilder2.ToString());
		}

		public static List<object[]> QueryReminders(ExDateTime actualizationTime, UserContext userContext)
		{
			List<object[]> list = new List<object[]>();
			object[][] array = null;
			ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.ReminderNextTime, actualizationTime.AddHours(24.0));
			if (userContext.IsPushNotificationsEnabled)
			{
				array = userContext.MapiNotificationManager.GetReminderRows(comparisonFilter, 100);
			}
			else
			{
				using (SearchFolder searchFolder = SearchFolder.Bind(userContext.MailboxSession, DefaultFolderType.Reminders))
				{
					SortBy[] sortColumns = new SortBy[]
					{
						new SortBy(ItemSchema.ReminderIsSet, SortOrder.Descending),
						new SortBy(ItemSchema.ReminderNextTime, SortOrder.Descending)
					};
					using (QueryResult queryResult = searchFolder.ItemQuery(ItemQueryType.None, null, sortColumns, RemindersRenderingUtilities.QueryProperties))
					{
						queryResult.SeekToCondition(SeekReference.OriginBeginning, comparisonFilter);
						array = queryResult.GetRows(100);
					}
				}
			}
			if (array == null)
			{
				return list;
			}
			for (int i = 0; i < array.Length; i++)
			{
				bool itemProperty = RemindersRenderingUtilities.GetItemProperty<bool>(array[i], ItemSchema.ReminderIsSet, false);
				if (!itemProperty)
				{
					break;
				}
				list.Add(array[i]);
				if (list.Count == 100)
				{
					break;
				}
			}
			return list;
		}

		private static void RenderSnoozeDropDownList(TextWriter output)
		{
			double reminderTime = 5.0;
			new ReminderDropDownList("divRmdSnzDD", reminderTime, true)
			{
				Enabled = true
			}.Render(output);
		}

		private static Dictionary<PropertyDefinition, int> LoadPropertyMap()
		{
			Dictionary<PropertyDefinition, int> dictionary = new Dictionary<PropertyDefinition, int>();
			for (int i = 0; i < RemindersRenderingUtilities.QueryProperties.Length; i++)
			{
				dictionary[RemindersRenderingUtilities.QueryProperties[i]] = i;
			}
			return dictionary;
		}

		private static T GetItemProperty<T>(object[] reminder, PropertyDefinition propertyDefinition) where T : class
		{
			int num = RemindersRenderingUtilities.propertyMap[propertyDefinition];
			return reminder[num] as T;
		}

		private static T GetItemProperty<T>(object[] reminder, PropertyDefinition propertyDefinition, T defaultValue)
		{
			int num = RemindersRenderingUtilities.propertyMap[propertyDefinition];
			object obj = reminder[num];
			if (!(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private static ExDateTime MaxOutlookDate
		{
			get
			{
				return RemindersRenderingUtilities.maxOutlookDateUtc;
			}
		}

		private const int Max100 = 100;

		internal static readonly PropertyDefinition[] QueryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			MessageItemSchema.SwappedToDoStore,
			MessageItemSchema.IsDraft,
			ItemSchema.ReminderIsSet,
			ItemSchema.ReminderNextTime,
			ItemSchema.ReminderDueBy,
			ItemSchema.Subject,
			CalendarItemBaseSchema.Location,
			StoreObjectSchema.ItemClass,
			CalendarItemBaseSchema.CalendarItemType,
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime
		};

		private static readonly Dictionary<PropertyDefinition, int> propertyMap = RemindersRenderingUtilities.LoadPropertyMap();

		private static ExDateTime maxOutlookDateUtc = new ExDateTime(ExTimeZone.UtcTimeZone, 4501, 1, 1);
	}
}
