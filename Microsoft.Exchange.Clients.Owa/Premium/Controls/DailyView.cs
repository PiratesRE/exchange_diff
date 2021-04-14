using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class DailyView : DailyViewBase, ICalendarViewControl
	{
		public DailyView(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter) : base(sessionContext, calendarAdapter)
		{
		}

		public override int MaxEventAreaRows
		{
			get
			{
				return this.MaxItemsPerView;
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
				return 25;
			}
		}

		public OwaStoreObjectId SelectedItemId
		{
			get
			{
				return this.selectedItemId;
			}
			set
			{
				if (value == null)
				{
					throw new InvalidOperationException("SelectedItemId cannot be null");
				}
				this.selectedItemId = value;
			}
		}

		public void RenderSchedulingArea(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=\"divDVContainer");
			output.Write(Utilities.SanitizeHtmlEncode(base.CalendarAdapter.IdentityString));
			output.Write("\" class=\"calContainer\">");
			output.Write("<div id=\"divDVBody\">");
			for (int i = 0; i < base.DateRanges.Length; i++)
			{
				output.Write("<div class=\"abs\" id=\"divDay");
				output.Write(i.ToString(CultureInfo.InvariantCulture));
				output.Write("\">");
				output.Write("<div id=\"divSchedulingAreaBack\"></div>");
				output.Write("<div id=\"divSchedulingAreaVisualContainer\"><div id=\"divToolTip\"></div></div>");
				output.Write("</div>");
			}
			output.Write("</div></div>");
		}

		public void RenderHeadersAndEventArea(TextWriter output, bool visible)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=\"divTop\"");
			if (!visible)
			{
				output.Write(" style=\"display:none;\"");
			}
			output.Write(">");
			output.Write("<div id=\"divGAP\">");
			CalendarUtilities.RenderPreviousNextButtons(base.SessionContext, output);
			output.Write("</div>");
			output.Write("<div id=\"divEAContainer");
			output.Write(Utilities.SanitizeHtmlEncode(base.CalendarAdapter.IdentityString));
			output.Write("\" class=\"calContainer\">");
			output.Write("<div id=\"divDVTab\"></div>");
			output.Write("<div id=\"divDVEventBack\">");
			for (int i = 0; i < base.DateRanges.Length; i++)
			{
				output.Write("<div class=\"abs\" id=\"divDay");
				output.Write(i.ToString(CultureInfo.InvariantCulture));
				output.Write("\"><div id=\"divDVEventAreaGradient\"></div></div>");
			}
			output.Write("</div>");
			output.Write("<div id=\"divDVHeader\">");
			if (DateTimeUtilities.GetDaysFormat(base.SessionContext.DateFormat) == null)
			{
			}
			for (int j = 0; j < base.DateRanges.Length; j++)
			{
				DateTimeUtilities.IsToday(base.DateRanges[j].Start);
				output.Write("<div id=\"divDay");
				output.Write(j.ToString(CultureInfo.InvariantCulture));
				output.Write("\" class=\"calHD\">");
				output.Write("<span id=\"spanDayName\"></span><span id=\"spanWeekDayName\"></span>");
				output.Write("</div>");
			}
			output.Write("</div>");
			output.Write("</div>");
			base.SessionContext.RenderThemeImage(output, ThemeFileId.Clear1x1, "calScr", new object[]
			{
				"id=\"imgSACT\", style=\"display:none\""
			});
			base.SessionContext.RenderThemeImage(output, ThemeFileId.Clear1x1, "calScr", new object[]
			{
				"id=\"imgSACB\", style=\"display:none\""
			});
			output.Write("</div>");
			output.Write("<div id=\"divDVEventContainer\"><div id=\"divDVEventBody");
			output.Write(Utilities.SanitizeHtmlEncode(base.CalendarAdapter.IdentityString));
			output.Write("\" class=\"calContainer\"></div></div>");
		}

		public void RenderTimeStrip(TextWriter output)
		{
			output.Write("<div id=\"divTSW\"></div><div id=\"divTSWB\"></div>");
			int value;
			if (base.TimeStripMode == TimeStripMode.FifteenMinutes)
			{
				value = 96;
			}
			else
			{
				value = 48;
			}
			DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
			string text = DateTimeUtilities.GetHoursFormat(base.SessionContext.TimeFormat);
			if (text == null)
			{
				text = "%h";
			}
			for (int i = 0; i < 24; i++)
			{
				string s = "00";
				if (text[1] == 'h')
				{
					if (i < 12)
					{
						s = Culture.AMDesignator;
					}
					else
					{
						s = Culture.PMDesignator;
					}
				}
				output.Write("<div class=\"timeStripLine\" style=\"height:");
				output.Write(value);
				output.Write("px\"><div class=\"timeStripLeft\">");
				output.Write(dateTime.ToString(text, CultureInfo.InvariantCulture));
				output.Write("</div><div class=\"timeStripRight\">");
				output.Write(Utilities.SanitizeHtmlEncode(s));
				output.Write("</div></div>");
				dateTime = dateTime.AddHours(1.0);
			}
		}

		internal static void RenderSecondaryNavigation(TextWriter output, CalendarFolder folder, UserContext userContext)
		{
			output.Write("<div id=\"divCalPicker\">");
			RenderingUtilities.RenderSecondaryNavigationDatePicker(folder, output, "divErrDP", "dp", userContext);
			new MonthPicker(userContext, "divMp").Render(output);
			output.Write("</div>");
			NavigationHost.RenderNavigationTreeControl(output, userContext, NavigationModule.Calendar);
		}

		public int EventAreaPixelHeight
		{
			get
			{
				return 2 + 26 * base.EventAreaRowCount + 12 + 2;
			}
		}

		public int Count
		{
			get
			{
				return base.VisualCount;
			}
		}

		public const int RowHeight = 24;

		private OwaStoreObjectId selectedItemId;
	}
}
