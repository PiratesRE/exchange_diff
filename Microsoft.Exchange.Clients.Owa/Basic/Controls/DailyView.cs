using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class DailyView : DailyViewBase
	{
		public DailyView(UserContext userContext, CalendarAdapter calendarAdapter) : base(userContext, calendarAdapter)
		{
		}

		private UserContext UserContext
		{
			get
			{
				return (UserContext)base.SessionContext;
			}
		}

		public override int MaxEventAreaRows
		{
			get
			{
				return 22;
			}
		}

		public override int MaxItemsPerView
		{
			get
			{
				return 300;
			}
		}

		public override int MaxConflictingItems
		{
			get
			{
				return 7;
			}
		}

		protected override TimeStripMode GetTimeStripMode()
		{
			return TimeStripMode.ThirtyMinutes;
		}

		private static string GetFreeBusyStylePrefix(BusyTypeWrapper busyType)
		{
			string result;
			switch (busyType)
			{
			case BusyTypeWrapper.Tentative:
				result = "tn";
				break;
			case BusyTypeWrapper.Busy:
				result = "bs";
				break;
			case BusyTypeWrapper.OOF:
				result = "of";
				break;
			default:
				result = "fr";
				break;
			}
			return result;
		}

		public void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			SchedulingAreaVisualContainer schedulingAreaVisualContainer = base.ViewDays[0];
			this.totalNumberOfColumns = this.GetTotalNumberOfColumns(schedulingAreaVisualContainer, this.conflictsPerTimeSlot);
			this.CalculateStartAndEndTimes();
			this.ComputeDayFreeBusy();
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"cdayvw\"><caption>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(573876176));
			writer.Write("</caption>");
			string text = DateTimeUtilities.GetDaysFormat(this.UserContext.UserOptions.DateFormat);
			if (text == null)
			{
				text = "%d";
			}
			ExDateTime exDateTime = schedulingAreaVisualContainer.DateRange.Start.IncrementDays(-1);
			ExDateTime exDateTime2 = schedulingAreaVisualContainer.DateRange.Start.IncrementDays(1);
			writer.Write("<tr><td rowspan=2 class=\"nv\" align=\"center\"><a href=\"#\" title=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(790373277));
			writer.Write("\" onClick=\"return onClkD({0},{1},{2});\"><img src=\"", exDateTime.Year, exDateTime.Month, exDateTime.Day);
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.PreviousPage);
			writer.Write("\" alt=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(790373277));
			writer.Write("\"></a><a href=\"#\" title=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-943287977));
			writer.Write("\" onClick=\"return onClkD({0},{1},{2});\"><img src=\"", exDateTime2.Year, exDateTime2.Month, exDateTime2.Day);
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.NextPage);
			writer.Write("\" alt=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-943287977));
			writer.Write("\"></a></td><td colspan=");
			writer.Write(this.totalNumberOfColumns);
			writer.Write(" class=\"cvhd");
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			if (schedulingAreaVisualContainer.DateRange.Start.Year == localTime.Year && schedulingAreaVisualContainer.DateRange.Start.Month == localTime.Month && schedulingAreaVisualContainer.DateRange.Start.Day == localTime.Day)
			{
				writer.Write(" cd");
			}
			writer.Write("\" align=\"center\"><span>");
			writer.Write(Utilities.HtmlEncode(schedulingAreaVisualContainer.DateRange.Start.ToString(text)));
			writer.Write("</span>");
			writer.Write(Utilities.HtmlEncode(schedulingAreaVisualContainer.DateRange.Start.ToString("dddd")));
			writer.Write("</td></tr>");
			string freeBusyStylePrefix = DailyView.GetFreeBusyStylePrefix(this.dayBusyType);
			writer.Write("<tr><td colspan={0} class=\"{1}e\">", this.totalNumberOfColumns, freeBusyStylePrefix);
			if (base.EventArea.Count > 0)
			{
				this.RenderEventArea(writer);
			}
			else
			{
				writer.Write("&nbsp;");
			}
			writer.Write("</td></tr>");
			this.RenderSchedulingArea(writer, schedulingAreaVisualContainer);
			writer.Write("</table>");
		}

		private void RenderEventArea(TextWriter writer)
		{
			DateTimeUtilities.GetDaysFormat(this.UserContext.UserOptions.DateFormat);
			writer.Write("<table cellspacing=0 cellpadding=0 id=\"evt\">");
			int num = 0;
			while (num < base.EventArea.Count && num < this.MaxEventAreaRows)
			{
				EventAreaVisual eventAreaVisual = (EventAreaVisual)base.EventArea[num];
				int dataIndex = eventAreaVisual.DataIndex;
				if (!base.IsItemRemoved(dataIndex))
				{
					if (num > 0)
					{
						writer.Write("<tr><td colspan=3 class=\"hspc\"></td></tr>");
					}
					ICalendarDataSource dataSource = base.CalendarAdapter.DataSource;
					string subject = dataSource.GetSubject(dataIndex);
					ExDateTime startTime = dataSource.GetStartTime(dataIndex);
					ExDateTime endTime = dataSource.GetEndTime(dataIndex);
					string location = dataSource.GetLocation(dataIndex);
					bool isMeeting = dataSource.IsMeeting(dataIndex);
					bool flag = dataSource.HasAttachment(dataIndex);
					string format = "{0} {1:" + CultureInfo.CurrentCulture.DateTimeFormat.MonthDayPattern + "}";
					string altAndTitleText = string.Format(format, Strings.EventFrom, startTime);
					string altAndTitleText2 = string.Format(format, Strings.EventTo, endTime);
					OwaStoreObjectId itemId = dataSource.GetItemId(dataIndex);
					bool flag2 = dataSource.IsPrivate(dataIndex);
					CalendarItemTypeWrapper wrappedItemType = dataSource.GetWrappedItemType(dataIndex);
					writer.Write("<tr>");
					int num2 = 0;
					if (eventAreaVisual.LeftBreak)
					{
						num2 |= 1;
					}
					if (eventAreaVisual.RightBreak)
					{
						num2 |= 2;
					}
					switch (num2)
					{
					case 0:
						writer.Write("<td class=\"spc\"></td><td class=\"lt rt\" align=\"center\">");
						this.RenderSubject(writer, subject, 64, startTime, endTime, location, true, isMeeting, itemId);
						if (flag2 || wrappedItemType != CalendarItemTypeWrapper.Single || flag)
						{
							this.RenderIcons(writer, 3, flag2, wrappedItemType, flag);
						}
						writer.Write("</td><td class=\"spc\"></td>");
						break;
					case 1:
						this.RenderPreviousDayImage(writer, altAndTitleText);
						writer.Write("<td class=\"rt\" align=\"center\">");
						this.RenderSubject(writer, subject, 64, startTime, endTime, location, true, isMeeting, itemId);
						if (flag2 || wrappedItemType != CalendarItemTypeWrapper.Single || flag)
						{
							this.RenderIcons(writer, 3, flag2, wrappedItemType, flag);
						}
						writer.Write("</td><td class=\"spc\"></td>");
						break;
					case 2:
						writer.Write("<td class=\"spc\"></td><td class=\"lt\" align=\"center\">");
						this.RenderSubject(writer, subject, 64, startTime, endTime, location, true, isMeeting, itemId);
						if (flag2 || wrappedItemType != CalendarItemTypeWrapper.Single || flag)
						{
							this.RenderIcons(writer, 3, flag2, wrappedItemType, flag);
						}
						this.RenderNextDayImage(writer, altAndTitleText2);
						break;
					case 3:
						this.RenderPreviousDayImage(writer, altAndTitleText);
						writer.Write("<td align=\"center\">");
						this.RenderSubject(writer, subject, 64, startTime, endTime, location, true, isMeeting, itemId);
						if (flag2 || wrappedItemType != CalendarItemTypeWrapper.Single || flag)
						{
							this.RenderIcons(writer, 3, flag2, wrappedItemType, flag);
						}
						this.RenderNextDayImage(writer, altAndTitleText2);
						break;
					}
					writer.Write("</tr>");
				}
				num++;
			}
			writer.Write("</table>");
		}

		private void RenderPreviousDayImage(TextWriter writer, string altAndTitleText)
		{
			writer.Write("<td class=\"img\">");
			writer.Write("<img src=\"");
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.EventFrom);
			writer.Write("\" alt=\"");
			writer.Write(altAndTitleText);
			writer.Write("\" title=\"");
			writer.Write(altAndTitleText);
			writer.Write("\"></td>");
		}

		private void RenderNextDayImage(TextWriter writer, string altAndTitleText)
		{
			writer.Write("<td class=\"img\">");
			writer.Write("<img src=\"");
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.EventTo);
			writer.Write("\" alt=\"");
			writer.Write(altAndTitleText);
			writer.Write("\" title=\"");
			writer.Write(altAndTitleText);
			writer.Write("\"></td>");
		}

		private void RenderSchedulingArea(TextWriter writer, SchedulingAreaVisualContainer day)
		{
			int[,] array = this.MapSchedulingAreaData(day, this.totalNumberOfColumns);
			string text = DateTimeUtilities.GetHoursFormat(this.UserContext.UserOptions.TimeFormat);
			string arg = "00";
			if (text == null)
			{
				text = "%h";
			}
			int num = this.renderDayStartTime / 30;
			int num2 = this.renderDayEndTime / 30;
			int num3 = this.userDayStartTime / 30;
			int num4 = this.userDayEndTime / 30;
			ICalendarDataSource dataSource = base.CalendarAdapter.DataSource;
			ExDateTime exDateTime = new ExDateTime(this.UserContext.TimeZone, 2000, 1, 1, num / 2, 0, 0);
			for (int i = num; i < num2; i++)
			{
				writer.Write("<tr><td class=\"");
				if (i == num)
				{
					writer.Write("frst ");
				}
				if (i % 2 == 0)
				{
					if (text[1] == 'h')
					{
						if (exDateTime.Hour >= 0 && exDateTime.Hour < 12)
						{
							arg = Utilities.HtmlEncode(Culture.AMDesignator);
						}
						else if (exDateTime.Hour >= 12)
						{
							arg = Utilities.HtmlEncode(Culture.PMDesignator);
						}
					}
					writer.Write("tm\" align=\"right\">{0}&nbsp;<span>{1}</span>", exDateTime.ToString(text), arg);
					exDateTime = exDateTime.AddHours(1.0);
				}
				else
				{
					writer.Write("tme\">&nbsp;");
				}
				writer.Write("</td>");
				for (int j = 0; j < this.totalNumberOfColumns; j++)
				{
					if (array[i, j] >= 0 && array[i, j] >= 0)
					{
						int num5 = j + 1;
						int num6 = 1;
						while (num5 < this.totalNumberOfColumns && array[i, num5] == array[i, j])
						{
							array[i, num5] = -1;
							num5++;
						}
						int num7 = num5 - j;
						if (array[i, j] != 0)
						{
							num6 = 1;
							int num8 = i + 1;
							while (num8 < num2 && array[num8, j] == array[i, j])
							{
								num6++;
								for (int k = 0; k < num7; k++)
								{
									array[num8, j + k] = -1;
								}
								num8++;
							}
						}
						string freeBusyStylePrefix = DailyView.GetFreeBusyStylePrefix(base.RowFreeBusy[0][i]);
						writer.Write("<td class=\"");
						if (array[i, j] == 0)
						{
							writer.Write(freeBusyStylePrefix);
							if (i % 2 == 0)
							{
								writer.Write("d ");
							}
							else
							{
								writer.Write("l ");
							}
							writer.Write(freeBusyStylePrefix);
							if (num3 <= num4 && i >= num3 && i < num4)
							{
								writer.Write("w");
							}
							else if (num3 > num4 && (i >= num3 || i < num4))
							{
								writer.Write("w");
							}
							else
							{
								writer.Write("n");
							}
						}
						else if (array[i, j] > 0)
						{
							writer.Write("v");
							int index = array[i, j] - 1;
							ExDateTime startTime = dataSource.GetStartTime(index);
							ExDateTime endTime = dataSource.GetEndTime(index);
							if (startTime.Date < day.DateRange.Start.Date)
							{
								writer.Write(" ntp");
							}
							else if (endTime.Date > day.DateRange.Start.Date)
							{
								writer.Write(" nbtm");
							}
						}
						writer.Write("\"");
						if (num6 > 1)
						{
							writer.Write(" rowspan={0}", num6);
						}
						if (num7 > 1)
						{
							writer.Write(" colspan={0}", num7);
						}
						if (num7 < this.totalNumberOfColumns)
						{
							writer.Write(" width=\"{0:f}%\"", num7 * 100 / this.totalNumberOfColumns);
						}
						if (array[i, j] > 0)
						{
							writer.Write(" nowrap");
						}
						writer.Write(">");
						if (array[i, j] == 0)
						{
							writer.Write("&nbsp;</td>");
						}
						else if (array[i, j] > 0)
						{
							int index2 = array[i, j] - 1;
							ExDateTime startTime2 = dataSource.GetStartTime(index2);
							ExDateTime endTime2 = dataSource.GetEndTime(index2);
							string subject = dataSource.GetSubject(index2);
							string location = dataSource.GetLocation(index2);
							OwaStoreObjectId itemId = dataSource.GetItemId(index2);
							bool isPrivate = dataSource.IsPrivate(index2);
							CalendarItemTypeWrapper wrappedItemType = dataSource.GetWrappedItemType(index2);
							bool flag = dataSource.IsMeeting(index2);
							string text2 = string.Empty;
							if (flag)
							{
								text2 = dataSource.GetOrganizerDisplayName(index2);
							}
							bool hasAttachment = dataSource.HasAttachment(index2);
							int num9 = 64;
							int num10 = 3;
							this.ComputeMaxCharactersAndIcons(num6, num7, out num9, out num10);
							writer.Write("<table cellspacing=0 cellpadding=0 class=\"vis\"><tr>");
							BusyTypeWrapper wrappedBusyType = dataSource.GetWrappedBusyType(index2);
							if (wrappedBusyType != BusyTypeWrapper.Busy)
							{
								writer.Write("<td class=\"");
								switch (wrappedBusyType)
								{
								case BusyTypeWrapper.Free:
									writer.Write("f\"><img src=\"");
									this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Clear);
									writer.Write("\" alt=\"\"></td>");
									break;
								case BusyTypeWrapper.Tentative:
									writer.Write("t\"></td>");
									break;
								case BusyTypeWrapper.OOF:
									writer.Write("o\"></td>");
									break;
								case BusyTypeWrapper.Unknown:
									writer.Write("u\"></td>");
									break;
								}
							}
							writer.Write("<td class=\"txt\">");
							switch (num6)
							{
							case 1:
								this.RenderSubject(writer, subject, num9, startTime2, endTime2, location, false, flag, itemId);
								if (!string.IsNullOrEmpty(subject))
								{
									num9 -= subject.Length;
								}
								else if (flag)
								{
									num9 -= LocalizedStrings.GetNonEncoded(-1500721828).Length;
								}
								else
								{
									num9 -= LocalizedStrings.GetNonEncoded(-1178892512).Length;
								}
								if (!string.IsNullOrEmpty(location) && (location.Length + 2 <= num9 || num9 >= 5))
								{
									writer.Write("; ");
									Utilities.CropAndRenderText(writer, location, num9 - 2);
								}
								break;
							case 2:
								this.RenderSubject(writer, subject, num9, startTime2, endTime2, location, false, flag, itemId);
								if (!string.IsNullOrEmpty(location))
								{
									writer.Write("<br>");
									Utilities.CropAndRenderText(writer, location, num9);
									num9 -= location.Length;
								}
								if (!string.IsNullOrEmpty(text2) && flag && num9 > 3)
								{
									writer.Write("; ");
									Utilities.CropAndRenderText(writer, text2, num9);
								}
								break;
							default:
								this.RenderSubject(writer, subject, num9, startTime2, endTime2, location, false, flag, itemId);
								if (!string.IsNullOrEmpty(location))
								{
									writer.Write("<br>");
									Utilities.CropAndRenderText(writer, location, num9);
								}
								if (!string.IsNullOrEmpty(text2) && flag)
								{
									writer.Write("<br>");
									Utilities.CropAndRenderText(writer, text2, num9);
								}
								break;
							}
							writer.Write("</td>");
							if (num10 > 0)
							{
								writer.Write("<td class=\"icn\" align=\"right\">");
								this.RenderIcons(writer, num10, isPrivate, wrappedItemType, hasAttachment);
								writer.Write("</td>");
							}
							writer.Write("</tr></table></td>");
						}
					}
				}
				writer.Write("</tr>");
			}
			int num11 = base.RowFreeBusy[0].Length - 1;
			string freeBusyStylePrefix2 = DailyView.GetFreeBusyStylePrefix(base.RowFreeBusy[0][num11]);
			writer.Write("<tr><td class=\"tm\"><img src=\"");
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Clear);
			writer.Write("\" alt=\"\"></td><td colspan=\"{0}\" class=\"{1}d {1}n\"><img src=\"", this.totalNumberOfColumns, freeBusyStylePrefix2);
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Clear);
			writer.Write("\" alt=\"\"></td></tr>");
			writer.Write("<tr><td class=\"tme h100\"><img src=\"");
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Clear);
			writer.Write("\" alt=\"\"></td><td colspan=\"{0}\" class=\"{1}n\"></td></tr>", this.totalNumberOfColumns, freeBusyStylePrefix2);
		}

		private int[,] MapSchedulingAreaData(SchedulingAreaVisualContainer day, int totalNumberOfColumns)
		{
			int count = day.Count;
			int[,] visualPositionData = this.GetVisualPositionData(day, totalNumberOfColumns);
			int[,] array = new int[48, totalNumberOfColumns];
			for (int i = 0; i < count; i++)
			{
				int num = (visualPositionData[i, 1] >= 0) ? visualPositionData[i, 1] : 0;
				while (num < 48 && num < visualPositionData[i, 1] + visualPositionData[i, 3])
				{
					for (int j = visualPositionData[i, 0]; j < visualPositionData[i, 0] + visualPositionData[i, 2]; j++)
					{
						array[num, j] = visualPositionData[i, 4];
					}
					num++;
				}
			}
			return array;
		}

		private int[,] GetVisualPositionData(SchedulingAreaVisualContainer day, int totalNumberOfColumns)
		{
			int count = day.Count;
			int[,] array = new int[count, 5];
			for (int i = 0; i < count; i++)
			{
				SchedulingAreaVisual schedulingAreaVisual = (SchedulingAreaVisual)day[i];
				if (!base.IsItemRemoved(schedulingAreaVisual.DataIndex))
				{
					array[i, 0] = (int)Math.Round(schedulingAreaVisual.Rect.X * (double)totalNumberOfColumns);
					array[i, 1] = (int)Math.Floor(schedulingAreaVisual.Rect.Y);
					array[i, 2] = (int)Math.Round(schedulingAreaVisual.Rect.Width * (double)totalNumberOfColumns);
					array[i, 3] = ((schedulingAreaVisual.Rect.Height < 1.0) ? 1 : ((int)Math.Round(schedulingAreaVisual.Rect.Y + schedulingAreaVisual.Rect.Height - (double)array[i, 1])));
					if (array[i, 1] + array[i, 3] <= 0)
					{
						array[i, 1] = 0;
					}
					array[i, 4] = schedulingAreaVisual.DataIndex + 1;
				}
			}
			return array;
		}

		private int GetTotalNumberOfColumns(SchedulingAreaVisualContainer day, int[] conflictsPerTimeSlot)
		{
			int count = day.Count;
			for (int i = 0; i < count; i++)
			{
				SchedulingAreaVisual schedulingAreaVisual = (SchedulingAreaVisual)day[i];
				int num = (int)(1.0 / schedulingAreaVisual.Rect.Width);
				int num2 = 0;
				while ((double)num2 < schedulingAreaVisual.AdjustedRect.Height)
				{
					int num3 = (int)schedulingAreaVisual.AdjustedRect.Y + num2;
					if (num3 >= 0 && num3 < 48 && conflictsPerTimeSlot[num3] < this.MaxConflictingItems && num > conflictsPerTimeSlot[num3])
					{
						conflictsPerTimeSlot[num3] = num;
					}
					num2++;
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			for (int j = 0; j < 48; j++)
			{
				if (!flag && conflictsPerTimeSlot[j] == 2)
				{
					flag = true;
				}
				if (!flag2 && conflictsPerTimeSlot[j] == 3)
				{
					flag2 = true;
				}
				if (!flag3 && conflictsPerTimeSlot[j] == 4)
				{
					flag3 = true;
				}
				if (!flag4 && conflictsPerTimeSlot[j] == 5)
				{
					flag4 = true;
				}
				if (!flag5 && conflictsPerTimeSlot[j] == 6)
				{
					flag5 = true;
				}
				if (!flag6 && conflictsPerTimeSlot[j] >= 7)
				{
					flag6 = true;
				}
			}
			if (flag5)
			{
				if (flag3)
				{
					flag3 = false;
					flag = true;
				}
				else
				{
					flag = false;
				}
				flag2 = false;
			}
			if (flag3)
			{
				flag = false;
			}
			int num4 = 1;
			if (flag)
			{
				num4 *= 2;
			}
			if (flag2)
			{
				num4 *= 3;
			}
			if (flag3)
			{
				num4 *= 4;
			}
			if (flag4)
			{
				num4 *= 5;
			}
			if (flag5)
			{
				num4 *= 6;
			}
			if (flag6)
			{
				num4 *= 7;
			}
			return num4;
		}

		private void RenderSubject(TextWriter writer, string subject, int maxCharacters, ExDateTime startDateTime, ExDateTime endDateTime, string location, bool isEvent, bool isMeeting, OwaStoreObjectId itemId)
		{
			if (Utilities.WhiteSpaceOnlyOrNullEmpty(subject))
			{
				if (isMeeting)
				{
					subject = LocalizedStrings.GetNonEncoded(-1500721828);
				}
				else
				{
					subject = LocalizedStrings.GetNonEncoded(-1178892512);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (isEvent)
			{
				stringBuilder.Append(startDateTime.ToString(this.UserContext.UserOptions.DateFormat));
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(startDateTime.ToString(this.UserContext.UserOptions.TimeFormat));
			stringBuilder.Append(" - ");
			if (isEvent)
			{
				stringBuilder.Append(endDateTime.ToString(this.UserContext.UserOptions.DateFormat));
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(endDateTime.ToString(this.UserContext.UserOptions.TimeFormat));
			stringBuilder.Append(" , ");
			stringBuilder.Append(Utilities.HtmlEncode(subject));
			if (!string.IsNullOrEmpty(location))
			{
				stringBuilder.Append("; ");
				stringBuilder.Append(Utilities.HtmlEncode(location));
			}
			writer.Write("<h1 class=\"bld\"><a href=\"#\" onClick=\"return onClkM('{0}')\" title=\"{1}\">", HttpUtility.UrlEncode(itemId.StoreObjectId.ToBase64String()), stringBuilder.ToString());
			Utilities.CropAndRenderText(writer, subject, maxCharacters);
			writer.Write("</a></h1>");
		}

		private void RenderIcons(TextWriter writer, int maxIcons, bool isPrivate, CalendarItemTypeWrapper calendarItemType, bool hasAttachment)
		{
			if (isPrivate)
			{
				maxIcons--;
				writer.Write("<img src=\"");
				this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Private);
				writer.Write("\" alt=\"");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-1268489823));
				writer.Write("\">");
			}
			if (maxIcons > 0)
			{
				if (calendarItemType == CalendarItemTypeWrapper.Occurrence)
				{
					maxIcons--;
					writer.Write("<img src=\"");
					this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.RecurringAppointment);
					writer.Write("\" alt=\"");
					writer.Write(LocalizedStrings.GetHtmlEncoded(-1522602052));
					writer.Write("\">");
				}
				else if (calendarItemType == CalendarItemTypeWrapper.Exception)
				{
					maxIcons--;
					writer.Write("<img src=\"");
					this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Exception);
					writer.Write("\" alt=\"");
					writer.Write(LocalizedStrings.GetHtmlEncoded(1824213891));
					writer.Write("\">");
				}
			}
			if (maxIcons > 0 && hasAttachment)
			{
				maxIcons--;
				writer.Write("<img src=\"");
				this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Attachment3);
				writer.Write("\" alt=\"");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-1498653219));
				writer.Write("\">");
			}
		}

		private void ComputeDayFreeBusy()
		{
			for (int i = 0; i < base.RowFreeBusy[0].Length; i++)
			{
				BusyTypeWrapper busyTypeWrapper = base.RowFreeBusy[0][i];
				if (busyTypeWrapper > this.dayBusyType)
				{
					this.dayBusyType = busyTypeWrapper;
				}
			}
		}

		private void CalculateStartAndEndTimes()
		{
			this.userDayStartTime = this.UserContext.WorkingHours.GetWorkDayStartTime(base.ViewDays[0].DateRange.Start);
			this.userDayEndTime = this.UserContext.WorkingHours.GetWorkDayEndTime(base.ViewDays[0].DateRange.Start);
			this.renderDayEndTime = 1440;
			if (this.userDayStartTime > 1440 || this.userDayStartTime < 0)
			{
				this.userDayStartTime %= 1440;
			}
			if (this.userDayStartTime < 0)
			{
				this.userDayStartTime += 1440;
			}
			if (this.userDayEndTime > 1440 || this.userDayEndTime < 0)
			{
				this.userDayEndTime %= 1440;
			}
			if (this.userDayEndTime < 0)
			{
				this.userDayEndTime += 1440;
			}
			if (this.userDayStartTime > this.userDayEndTime)
			{
				this.renderDayStartTime = 0;
				return;
			}
			if (this.userDayStartTime > 60)
			{
				this.renderDayStartTime = this.userDayStartTime - 60;
				if (this.renderDayStartTime % 60 != 0)
				{
					this.renderDayStartTime -= 30;
				}
			}
			else
			{
				this.renderDayStartTime = 0;
			}
			int num = this.renderDayEndTime - this.renderDayStartTime;
			if (num < 720)
			{
				int num2 = 720 - num;
				this.renderDayStartTime -= num2;
			}
			if (this.renderDayStartTime > 0)
			{
				int i = 0;
				while (i < this.renderDayStartTime / 30)
				{
					if (this.conflictsPerTimeSlot[i] > 0)
					{
						this.renderDayStartTime = i * 30;
						if (this.renderDayStartTime % 60 != 0)
						{
							this.renderDayStartTime -= 30;
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		private void ComputeMaxCharactersAndIcons(int rowspan, int colspan, out int maxCharacters, out int maxIcons)
		{
			maxCharacters = 64;
			maxIcons = 3;
			switch (this.totalNumberOfColumns / colspan)
			{
			case 1:
				if (rowspan > 3)
				{
					maxIcons = 4;
					return;
				}
				break;
			case 2:
				maxCharacters = 32;
				if (rowspan < 4)
				{
					maxIcons = 1;
					return;
				}
				maxIcons = 4;
				return;
			case 3:
				maxCharacters = 20;
				if (rowspan < 4)
				{
					maxIcons = 0;
					return;
				}
				maxIcons = 4;
				return;
			case 4:
				maxCharacters = 16;
				if (rowspan < 4)
				{
					maxIcons = 0;
					return;
				}
				maxIcons = 4;
				return;
			case 5:
				maxCharacters = 12;
				if (rowspan < 4)
				{
					maxIcons = 0;
					return;
				}
				maxIcons = 4;
				return;
			case 6:
				maxCharacters = 10;
				if (rowspan < 4)
				{
					maxIcons = 0;
					return;
				}
				maxIcons = 3;
				return;
			default:
				maxCharacters = 8;
				if (rowspan < 4)
				{
					maxIcons = 0;
					return;
				}
				maxIcons = 2;
				break;
			}
		}

		private const int NumberOfTimeSlots = 48;

		private const int MaxIconsForEvents = 3;

		public static int RowHeight = 24;

		private int[] conflictsPerTimeSlot = new int[48];

		private int totalNumberOfColumns = 1;

		private BusyTypeWrapper dayBusyType;

		private int userDayStartTime;

		private int userDayEndTime;

		private int renderDayStartTime;

		private int renderDayEndTime;

		internal static readonly PropertyDefinition[] QueryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime,
			ItemSchema.Subject,
			CalendarItemBaseSchema.Location,
			CalendarItemBaseSchema.OrganizerDisplayName,
			CalendarItemBaseSchema.CalendarItemType,
			ItemSchema.HasAttachment,
			CalendarItemBaseSchema.FreeBusyStatus,
			ItemSchema.Sensitivity,
			CalendarItemBaseSchema.AppointmentState
		};
	}
}
