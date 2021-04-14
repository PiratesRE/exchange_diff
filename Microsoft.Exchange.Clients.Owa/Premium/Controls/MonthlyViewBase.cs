using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class MonthlyViewBase : CalendarViewBase
	{
		public MonthlyViewBase(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter) : base(sessionContext, calendarAdapter)
		{
			if (calendarAdapter != null && calendarAdapter.DateRanges != null && calendarAdapter.DataSource != null)
			{
				this.CreateVisuals();
				this.MapVisuals();
				this.monthName = base.DateRanges[7].Start.ToString("y", sessionContext.UserCulture);
			}
		}

		private void CreateVisuals()
		{
			this.visualContainer = new MonthlyViewVisualContainer(this);
			for (int i = 0; i < base.DataSource.Count; i++)
			{
				if (i > this.MaxItemsPerView)
				{
					base.RemoveItemFromView(i);
				}
				else
				{
					this.visualContainer.AddVisual(new EventAreaVisual(i));
				}
			}
		}

		private void MapVisuals()
		{
			this.visualContainer.MapVisuals();
		}

		public override int MaxEventAreaRows
		{
			get
			{
				return 100;
			}
		}

		public override int MaxItemsPerView
		{
			get
			{
				return 1000;
			}
		}

		public override SanitizedHtmlString FolderDateDescription
		{
			get
			{
				return SanitizedHtmlString.GetSanitizedStringWithoutEncoding(this.monthName);
			}
		}

		public MonthlyViewVisualContainer VisualContainer
		{
			get
			{
				return this.visualContainer;
			}
		}

		public string PreviousMonthName
		{
			get
			{
				ExDateTime start = base.DateRanges[0].Start;
				if (start.Day == 1)
				{
					return string.Empty;
				}
				int num = start.Month - 1;
				return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[num];
			}
		}

		public string CurrentMonthName
		{
			get
			{
				int num = base.DateRanges[7].Start.Month - 1;
				return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[num];
			}
		}

		public string NextMonthName
		{
			get
			{
				ExDateTime start = base.DateRanges[base.DateRanges.Length - 1].Start;
				if (start.Day < 7)
				{
					int num = start.Month - 1;
					return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[num];
				}
				return string.Empty;
			}
		}

		public const int RowHeight = 20;

		public const int DaysOfAWeek = 7;

		private MonthlyViewVisualContainer visualContainer;

		protected string monthName;
	}
}
