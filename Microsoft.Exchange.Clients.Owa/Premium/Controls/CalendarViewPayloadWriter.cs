using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class CalendarViewPayloadWriter
	{
		protected CalendarViewPayloadWriter(ISessionContext sessionContext, TextWriter output)
		{
			this.sessionContext = sessionContext;
			this.output = output;
		}

		protected ISessionContext SessionContext
		{
			get
			{
				return this.sessionContext;
			}
		}

		protected TextWriter Output
		{
			get
			{
				return this.output;
			}
		}

		protected Hashtable ItemIndex
		{
			get
			{
				return this.itemIndex;
			}
		}

		protected int SelectedItemIndex
		{
			get
			{
				return this.selectedItemIndex;
			}
		}

		public abstract void Render(int viewWidth, CalendarViewType viewType, ReadingPanePosition readingPanePosition, ReadingPanePosition requestReadingPanePosition);

		private void RenderValue(TextWriter writer, string name, object value, bool addQuote)
		{
			writer.Write("\"");
			writer.Write(name);
			writer.Write("\":");
			if (addQuote)
			{
				writer.Write("\"");
			}
			writer.Write(value);
			if (addQuote)
			{
				writer.Write("\"");
			}
			writer.Write(",");
		}

		private void RenderValue(TextWriter writer, string name, string value, bool addQuote)
		{
			this.RenderValue(writer, name, value, addQuote);
		}

		private void RenderValue(TextWriter writer, string name, SanitizedHtmlString value, bool addQuote)
		{
			this.RenderValue(writer, name, value, addQuote);
		}

		protected void RenderValue(TextWriter writer, string name, string value)
		{
			this.RenderValue(writer, name, Utilities.JavascriptEncode(value), true);
		}

		protected void RenderValue(TextWriter writer, string name, SanitizedHtmlString value)
		{
			this.RenderValue(writer, name, Utilities.JavascriptEncode(value), true);
		}

		protected void RenderValue(TextWriter writer, string name, int value)
		{
			this.RenderValue(writer, name, Convert.ToString(value, CultureInfo.InvariantCulture), false);
		}

		protected void RenderValue(TextWriter writer, string name, bool value)
		{
			this.RenderValue(writer, name, value ? 1 : 0);
		}

		protected void RenderValue(TextWriter writer, string name, double value, string format)
		{
			this.RenderValue(writer, name, value.ToString(format, CultureInfo.InvariantCulture), false);
		}

		protected void RenderCalendarProperties(CalendarViewBase view)
		{
			CalendarAdapterBase calendarAdapter = view.CalendarAdapter;
			this.RenderValue(this.Output, "sFId", calendarAdapter.IdentityString);
			this.RenderValue(this.Output, "fHRtL", calendarAdapter.UserCanReadItem);
			this.RenderValue(this.Output, "Title", SanitizedHtmlString.GetSanitizedStringWithoutEncoding(calendarAdapter.CalendarTitle));
			this.RenderValue(this.Output, "sDD", view.FolderDateDescription);
			this.RenderValue(this.Output, "fCC", calendarAdapter.DataSource.UserCanCreateItem);
			CalendarAdapter calendarAdapter2 = calendarAdapter as CalendarAdapter;
			this.RenderValue(this.Output, "iSharedType", (int)calendarAdapter.DataSource.SharedType);
			if (calendarAdapter2 != null)
			{
				if (calendarAdapter2.PromotedFolderId != null)
				{
					this.RenderValue(this.Output, "sPromotedFolderId", calendarAdapter2.PromotedFolderId.ToBase64String());
				}
				this.RenderValue(this.Output, "sLegacyDN", calendarAdapter2.LegacyDN);
				if (calendarAdapter2.DataSource.SharedType == SharedType.InternalFreeBusy)
				{
					this.RenderValue(this.Output, "sCalendarOwnerDisplayName", calendarAdapter2.CalendarOwnerDisplayName);
				}
				this.RenderValue(this.Output, "iOlderExchangeCalendarType", (int)calendarAdapter2.OlderExchangeSharedCalendarType);
				this.RenderColor(calendarAdapter2);
				this.RenderValue(this.Output, "fPublishedOut", calendarAdapter2.IsPublishedOut);
				if (calendarAdapter2.IsExternalSharedInFolder)
				{
					if (calendarAdapter2.LastAttemptTime != ExDateTime.MinValue)
					{
						this.RenderValue(this.Output, "dtSyncTime", calendarAdapter2.LastAttemptTime.ToString("g", this.SessionContext.UserCulture));
					}
					if (calendarAdapter2.LastSuccessSyncTime != ExDateTime.MinValue)
					{
						this.RenderValue(this.Output, "dtSuccessSyncTime", calendarAdapter2.LastSuccessSyncTime.ToString("g", this.SessionContext.UserCulture));
					}
				}
				this.RenderValue(this.Output, "fArchive", calendarAdapter2.IsInArchiveMailbox);
				if (calendarAdapter2.DataSource.SharedType == SharedType.WebCalendar)
				{
					this.RenderValue(this.Output, "sWebCalUrl", calendarAdapter2.WebCalendarUrl);
					return;
				}
			}
			else if (calendarAdapter is PublishedCalendarAdapter)
			{
				PublishedCalendarAdapter publishedCalendarAdapter = (PublishedCalendarAdapter)calendarAdapter;
				this.RenderValue(this.Output, "sPublishRange", SanitizedHtmlString.Format(LocalizedStrings.GetNonEncoded(-1428371010), new object[]
				{
					publishedCalendarAdapter.PublishedFromDateTime.ToShortDateString(),
					publishedCalendarAdapter.PublishedToDateTime.ToShortDateString()
				}));
			}
		}

		protected void RenderEmptyCalendar(CalendarAdapterBase calendarAdapterBase, int index)
		{
			if (index > 0)
			{
				this.Output.Write(",");
			}
			this.Output.Write("{");
			this.RenderValue(this.Output, "sFId", calendarAdapterBase.IdentityString);
			if (calendarAdapterBase is CalendarAdapter)
			{
				CalendarAdapter calendarAdapter = (CalendarAdapter)calendarAdapterBase;
				this.RenderValue(this.Output, "iOlderExchangeCalendarType", (int)calendarAdapter.OlderExchangeSharedCalendarType);
			}
			this.Output.Write("\"fHRtL\": 0}");
		}

		protected void RenderCalendars(CalendarAdapterBase[] adapters, ExDateTime[] days, CalendarViewPayloadWriter.RenderCalendarDelegate render)
		{
			for (int i = 0; i < adapters.Length; i++)
			{
				if (adapters[i].UserCanReadItem)
				{
					render(adapters[i], i);
				}
				else
				{
					this.RenderEmptyCalendar(adapters[i], i);
				}
			}
		}

		protected void RenderDay(TextWriter writer, DateRange[] ranges, CalendarViewType viewType)
		{
			string format = DateTimeUtilities.GetDaysFormat(this.SessionContext.DateFormat) ?? "%d";
			string format2;
			if (CalendarUtilities.FullMonthNameRequired(this.SessionContext.UserCulture))
			{
				format2 = "MMMM";
			}
			else
			{
				format2 = "MMM";
			}
			for (int i = 0; i < ranges.Length; i++)
			{
				if (i > 0)
				{
					writer.Write(",");
				}
				ExDateTime start = ranges[i].Start;
				writer.Write("new Day(\"");
				if (start.Day == 1 && viewType == CalendarViewType.Monthly)
				{
					writer.Write("<span class='divMonthlyViewMonthName'>");
					writer.Write(Utilities.JavascriptEncode(start.ToString(format2, this.SessionContext.UserCulture)));
					writer.Write("</span>&nbsp;");
				}
				writer.Write(Utilities.JavascriptEncode(start.ToString(format, this.SessionContext.UserCulture)));
				writer.Write("\",\"");
				if (viewType != CalendarViewType.Monthly)
				{
					writer.Write(Utilities.JavascriptEncode(start.ToString("dddd", this.SessionContext.UserCulture)));
				}
				writer.Write("\",");
				writer.Write(start.Day);
				writer.Write(",");
				writer.Write(start.Month);
				writer.Write(",");
				writer.Write(start.Year);
				writer.Write(DateTimeUtilities.IsToday(start) ? ",1" : ",0");
				writer.Write(")");
			}
		}

		private void RenderPrivateAppointmentData(ExDateTime itemStart, ExDateTime itemEnd)
		{
			this.output.Write("new Item(");
			this.output.Write("-1,");
			this.output.Write(" 0,");
			this.output.Write(" 0,\"");
			this.output.Write(DateTimeUtilities.GetJavascriptDate(itemStart));
			this.output.Write("\",\"");
			this.output.Write(DateTimeUtilities.GetJavascriptDate(itemEnd));
			this.output.Write("\",\"");
			this.output.Write(LocalizedStrings.GetJavascriptEncoded(840767634));
			this.output.Write("\",");
			this.output.Write(" \"\",");
			this.output.Write(" 2,");
			this.output.Write(" \"\",");
			this.output.Write(" 0,");
			this.output.Write(" 0,");
			this.output.Write(" 0,");
			this.output.Write(" \"\",");
			this.output.Write(" 1,");
			this.output.Write(" 1,");
			this.output.Write(" 0,");
			this.output.Write(" \"noClrCal\")");
		}

		private void RenderAppointmentData(CalendarViewBase view, int i, ExDateTime itemStart, ExDateTime itemEnd)
		{
			ICalendarDataSource dataSource = view.DataSource;
			CalendarItemTypeWrapper wrappedItemType = dataSource.GetWrappedItemType(i);
			this.output.Write("new Item(\"");
			OwaStoreObjectId itemId = dataSource.GetItemId(i);
			string changeKey = dataSource.GetChangeKey(i);
			PublishedCalendarDataSource publishedCalendarDataSource = dataSource as PublishedCalendarDataSource;
			if (publishedCalendarDataSource != null && publishedCalendarDataSource.DetailLevel != DetailLevelEnumType.AvailabilityOnly)
			{
				StoreObjectId itemStoreObjectId = publishedCalendarDataSource.GetItemStoreObjectId(i);
				Utilities.JavascriptEncode(itemStoreObjectId.ToString(), this.output);
				this.output.Write("\",\"");
				if (this.IsOneOfRecurrence(wrappedItemType))
				{
					StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(itemStoreObjectId.ProviderLevelItemId);
					Utilities.JavascriptEncode(storeObjectId.ToString(), this.output);
				}
				else
				{
					this.output.Write("0");
				}
			}
			else
			{
				if (itemId != null)
				{
					Utilities.JavascriptEncode(itemId.ToString(), this.output);
				}
				this.output.Write("\",\"");
				if (itemId != null && this.IsOneOfRecurrence(wrappedItemType))
				{
					OwaStoreObjectId providerLevelItemId = itemId.ProviderLevelItemId;
					Utilities.JavascriptEncode(providerLevelItemId.ToString(), this.output);
				}
				else
				{
					this.output.Write("0");
				}
			}
			this.output.Write("\",\"");
			if (changeKey != null)
			{
				Utilities.JavascriptEncode(changeKey, this.output);
			}
			this.output.Write("\",\"");
			this.output.Write(DateTimeUtilities.GetJavascriptDate(itemStart));
			this.output.Write("\",\"");
			this.output.Write(DateTimeUtilities.GetJavascriptDate(itemEnd));
			this.output.Write("\",\"");
			Utilities.JavascriptEncode(dataSource.GetSubject(i), this.output);
			this.output.Write("\",\"");
			Utilities.JavascriptEncode(dataSource.GetLocation(i), this.output);
			BusyTypeWrapper wrappedBusyType = dataSource.GetWrappedBusyType(i);
			this.output.Write("\",");
			this.output.Write((int)wrappedBusyType);
			this.output.Write(",\"");
			if (itemId != null)
			{
				Utilities.JavascriptEncode(ObjectClass.GetContainerMessageClass(itemId.StoreObjectType), this.output);
			}
			this.output.Write("\"");
			bool flag = dataSource.IsMeeting(i);
			this.output.Write(flag ? ",1" : ",0");
			this.output.Write(dataSource.IsCancelled(i) ? ",1" : ",0");
			bool flag2 = dataSource.IsOrganizer(i);
			this.output.Write(flag2 ? ",1" : ",0");
			this.output.Write(",\"");
			if (flag)
			{
				Utilities.JavascriptEncode(dataSource.GetOrganizerDisplayName(i), this.output);
			}
			this.output.Write("\"");
			bool flag3 = dataSource.IsPrivate(i);
			this.output.Write(flag3 ? ",1," : ",0,");
			this.output.Write((int)wrappedItemType);
			this.output.Write(dataSource.HasAttachment(i) ? ",1" : ",0");
			this.output.Write(",\"");
			this.output.Write(dataSource.GetCssClassName(i));
			this.output.Write("\"");
			this.output.Write(")");
		}

		private bool IsOneOfRecurrence(CalendarItemTypeWrapper calendarItemType)
		{
			return calendarItemType == CalendarItemTypeWrapper.Occurrence || calendarItemType == CalendarItemTypeWrapper.Exception;
		}

		protected void RenderData(CalendarViewBase view, OwaStoreObjectId selectedItemId)
		{
			bool flag = true;
			int num = 0;
			if (view.RemovedItemCount > 0)
			{
				this.itemIndex = new Hashtable();
			}
			else
			{
				this.itemIndex = null;
			}
			this.selectedItemIndex = -1;
			TimeSpan t = TimeSpan.MinValue;
			PositionInTime positionInTime = PositionInTime.None;
			int num2 = 0;
			for (int i = 0; i < view.DataSource.Count; i++)
			{
				if (!view.IsItemRemoved(i))
				{
					bool flag2 = false;
					OwaStoreObjectId itemId = view.DataSource.GetItemId(i);
					if (view.DataSource.SharedType != SharedType.None)
					{
						flag2 = view.DataSource.IsPrivate(i);
					}
					if (!flag)
					{
						this.output.Write(",");
					}
					flag = false;
					num2++;
					if (this.itemIndex != null)
					{
						this.itemIndex[i] = num;
						num++;
					}
					ExDateTime startTime = view.DataSource.GetStartTime(i);
					ExDateTime endTime = view.DataSource.GetEndTime(i);
					if (flag2)
					{
						this.RenderPrivateAppointmentData(startTime, endTime);
					}
					else
					{
						this.RenderAppointmentData(view, i, startTime, endTime);
					}
					if (!flag2)
					{
						if (selectedItemId != null)
						{
							if (selectedItemId.Equals(itemId))
							{
								this.selectedItemIndex = ((this.itemIndex != null) ? ((int)this.itemIndex[i]) : i);
							}
						}
						else
						{
							bool flag3 = false;
							TimeSpan timeSpan = TimeSpan.MinValue;
							ExDateTime localTime = DateTimeUtilities.GetLocalTime();
							PositionInTime positionInTime2;
							if (endTime < localTime)
							{
								positionInTime2 = PositionInTime.Past;
							}
							else if (startTime > localTime)
							{
								positionInTime2 = PositionInTime.Future;
							}
							else
							{
								positionInTime2 = PositionInTime.Present;
							}
							if (positionInTime2 == PositionInTime.Past)
							{
								timeSpan = localTime - endTime;
								if (positionInTime == PositionInTime.Past)
								{
									if (timeSpan < t)
									{
										flag3 = true;
									}
								}
								else if (positionInTime == PositionInTime.None)
								{
									flag3 = true;
								}
							}
							else if (positionInTime2 == PositionInTime.Present)
							{
								timeSpan = endTime - localTime;
								if (positionInTime == PositionInTime.Present)
								{
									if (timeSpan < t)
									{
										flag3 = true;
									}
								}
								else
								{
									flag3 = true;
								}
							}
							else if (positionInTime2 == PositionInTime.Future)
							{
								timeSpan = startTime - localTime;
								if (positionInTime == PositionInTime.Future)
								{
									timeSpan = startTime - localTime;
									if (timeSpan < t)
									{
										flag3 = true;
									}
								}
								else if (positionInTime == PositionInTime.Past || positionInTime == PositionInTime.None)
								{
									flag3 = true;
								}
							}
							if (flag3)
							{
								this.selectedItemIndex = ((this.itemIndex != null) ? ((int)this.itemIndex[i]) : i);
								t = timeSpan;
								positionInTime = positionInTime2;
							}
						}
					}
				}
			}
		}

		protected void RenderEventAreaVisual(int idx, CalendarViewBase view, CalendarVisualContainer eventArea)
		{
			bool flag = true;
			for (int i = 0; i < eventArea.Count; i++)
			{
				EventAreaVisual eventAreaVisual = (EventAreaVisual)eventArea[i];
				if (!view.IsItemRemoved(eventAreaVisual.DataIndex))
				{
					if (!flag)
					{
						this.output.Write(",");
					}
					flag = false;
					int num = 0;
					if (eventAreaVisual.LeftBreak)
					{
						num |= 1;
					}
					if (eventAreaVisual.RightBreak)
					{
						num |= 2;
					}
					this.output.Write("new VisData(");
					this.Output.Write(idx);
					this.Output.Write(",");
					int num2 = (this.itemIndex != null) ? ((int)this.itemIndex[eventAreaVisual.DataIndex]) : eventAreaVisual.DataIndex;
					this.output.Write(num2);
					this.output.Write(",");
					this.output.Write((int)eventAreaVisual.Rect.X);
					this.output.Write(",");
					this.output.Write((int)eventAreaVisual.Rect.Y);
					this.output.Write(",");
					this.output.Write((int)eventAreaVisual.Rect.Width);
					this.output.Write(",");
					this.output.Write("0");
					this.output.Write(",");
					this.output.Write(num);
					this.output.Write(",");
					this.output.Write(eventAreaVisual.InnerBreaks);
					this.output.Write(",");
					if (num2 == this.selectedItemIndex)
					{
						this.output.Write("1");
					}
					else
					{
						this.output.Write("0");
					}
					this.output.Write(",-1");
					this.output.Write(")");
				}
			}
		}

		protected void RenderColor(CalendarAdapter calendarAdapter)
		{
			int num = -2;
			if (this.sessionContext.CanActAsOwner)
			{
				num = calendarAdapter.CalendarColor;
				if (!CalendarColorManager.IsColorIndexValid(num))
				{
					num = -2;
				}
			}
			this.RenderValue(this.Output, "iClr", CalendarColorManager.GetClientColorIndex(num));
		}

		private readonly ISessionContext sessionContext;

		private readonly TextWriter output;

		private int selectedItemIndex;

		private Hashtable itemIndex;

		protected delegate void RenderCalendarDelegate(CalendarAdapterBase CalendarAdapter, int index);
	}
}
