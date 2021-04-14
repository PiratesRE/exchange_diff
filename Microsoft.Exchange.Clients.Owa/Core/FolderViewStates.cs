using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class FolderViewStates
	{
		internal static bool ValidateCalendarViewType(CalendarViewType value)
		{
			return value >= CalendarViewType.Min && value <= CalendarViewType.Max && value != CalendarViewType.WeeklyAgenda && value != CalendarViewType.WorkWeeklyAgenda;
		}

		internal static bool ValidateDailyViewDays(int value)
		{
			return value >= 1 && value <= 7;
		}

		internal static bool ValidateReadingPanePosition(ReadingPanePosition value)
		{
			return value >= ReadingPanePosition.Min && value <= ReadingPanePosition.Bottom;
		}

		internal static bool ValidateSortOrder(SortOrder value)
		{
			return value == SortOrder.Ascending || value == SortOrder.Descending;
		}

		internal static bool ValidateWidthOrHeight(int value)
		{
			return value >= 0;
		}

		internal abstract CalendarViewType CalendarViewType { get; set; }

		internal abstract int DailyViewDays { get; set; }

		internal abstract bool GetMultiLine(bool defaultValue);

		internal abstract bool MultiLine { set; }

		internal abstract ReadingPanePosition GetReadingPanePosition(ReadingPanePosition defaultValue);

		internal abstract ReadingPanePosition ReadingPanePosition { get; set; }

		internal abstract ReadingPanePosition ReadingPanePositionMultiDay { get; set; }

		internal abstract string GetSortColumn(string defaultValue);

		internal abstract string SortColumn { set; }

		internal abstract SortOrder GetSortOrder(SortOrder defaultValue);

		internal abstract SortOrder SortOrder { set; }

		internal abstract int GetViewHeight(int defaultViewHeight);

		internal abstract int ViewHeight { get; set; }

		internal abstract int GetViewWidth(int defaultViewWidth);

		internal abstract int ViewWidth { get; set; }

		internal abstract void Save();

		public const CalendarViewType DefaultCalendarViewType = CalendarViewType.Min;

		public const int DefaultDailyViewDays = 1;

		public const ReadingPanePosition DefaultReadingPanePosition = ReadingPanePosition.Right;

		public const ReadingPanePosition DefaultReadingPanePositionMultiDay = ReadingPanePosition.Off;

		public const int DefaultViewHeight = 250;

		public const int DefaultViewWidth = 450;

		public const int DefaultTaskViewWidth = 381;

		public const int DefaultPublicFolderViewWidth = 450;

		public const int MaxDailyViewDays = 7;

		public const int MinDailyViewDays = 1;
	}
}
