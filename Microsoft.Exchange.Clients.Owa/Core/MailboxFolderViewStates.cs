using System;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class MailboxFolderViewStates : FolderViewStates
	{
		internal MailboxFolderViewStates(Folder folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.folder = folder;
		}

		internal override CalendarViewType CalendarViewType
		{
			get
			{
				return Utilities.GetFolderProperty<CalendarViewType>(this.folder, ViewStateProperties.CalendarViewType, CalendarViewType.Min);
			}
			set
			{
				if (!FolderViewStates.ValidateCalendarViewType(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set CalendarViewType to invalid value.");
				}
				this.folder[ViewStateProperties.CalendarViewType] = value;
			}
		}

		internal override int DailyViewDays
		{
			get
			{
				return Utilities.GetFolderProperty<int>(this.folder, ViewStateProperties.DailyViewDays, 1);
			}
			set
			{
				if (!FolderViewStates.ValidateDailyViewDays(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set DailyViewDays to invalid value.");
				}
				this.folder[ViewStateProperties.DailyViewDays] = value;
			}
		}

		internal override bool GetMultiLine(bool defaultValue)
		{
			return Utilities.GetFolderProperty<bool>(this.folder, ViewStateProperties.MultiLine, defaultValue);
		}

		internal override bool MultiLine
		{
			set
			{
				this.folder[ViewStateProperties.MultiLine] = value;
			}
		}

		internal override ReadingPanePosition GetReadingPanePosition(ReadingPanePosition defaultValue)
		{
			ReadingPanePosition folderProperty = Utilities.GetFolderProperty<ReadingPanePosition>(this.folder, ViewStateProperties.ReadingPanePosition, defaultValue);
			if (!FolderViewStates.ValidateReadingPanePosition(folderProperty))
			{
				return defaultValue;
			}
			return folderProperty;
		}

		internal override ReadingPanePosition ReadingPanePosition
		{
			get
			{
				return this.GetReadingPanePosition(ReadingPanePosition.Right);
			}
			set
			{
				if (!FolderViewStates.ValidateReadingPanePosition(value))
				{
					this.folder[ViewStateProperties.ReadingPanePosition] = ReadingPanePosition.Right;
					throw new ArgumentOutOfRangeException("value = " + value, "Cannot set ReadingPanePosition to invalid value.");
				}
				this.folder[ViewStateProperties.ReadingPanePosition] = value;
			}
		}

		internal override ReadingPanePosition ReadingPanePositionMultiDay
		{
			get
			{
				return Utilities.GetFolderProperty<ReadingPanePosition>(this.folder, ViewStateProperties.ReadingPanePositionMultiDay, ReadingPanePosition.Off);
			}
			set
			{
				if (!FolderViewStates.ValidateReadingPanePosition(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set ReadingPanePositionMultiDay to invalid value.");
				}
				this.folder[ViewStateProperties.ReadingPanePositionMultiDay] = value;
			}
		}

		internal override string GetSortColumn(string defaultValue)
		{
			return Utilities.GetFolderProperty<string>(this.folder, ViewStateProperties.SortColumn, defaultValue);
		}

		internal override string SortColumn
		{
			set
			{
				this.folder[ViewStateProperties.SortColumn] = value;
			}
		}

		internal override SortOrder GetSortOrder(SortOrder defaultValue)
		{
			return Utilities.GetFolderProperty<SortOrder>(this.folder, ViewStateProperties.SortOrder, defaultValue);
		}

		internal override SortOrder SortOrder
		{
			set
			{
				if (!FolderViewStates.ValidateSortOrder(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set SortOrder to invalid value.");
				}
				this.folder[ViewStateProperties.SortOrder] = value;
			}
		}

		internal override int GetViewHeight(int defaultViewHeight)
		{
			return Utilities.GetFolderProperty<int>(this.folder, ViewStateProperties.ViewHeight, defaultViewHeight);
		}

		internal override int ViewHeight
		{
			get
			{
				return this.GetViewHeight(250);
			}
			set
			{
				if (!FolderViewStates.ValidateWidthOrHeight(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set ViewHeight to invalid value: " + value);
				}
				this.folder[ViewStateProperties.ViewHeight] = value;
			}
		}

		internal override int GetViewWidth(int defaultViewWidth)
		{
			return Utilities.GetFolderProperty<int>(this.folder, ViewStateProperties.ViewWidth, defaultViewWidth);
		}

		internal override int ViewWidth
		{
			get
			{
				return this.GetViewWidth(450);
			}
			set
			{
				if (!FolderViewStates.ValidateWidthOrHeight(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set ViewWidth to invalid value: " + value);
				}
				this.folder[ViewStateProperties.ViewWidth] = value;
			}
		}

		internal override void Save()
		{
			this.folder.Save();
		}

		private Folder folder;
	}
}
