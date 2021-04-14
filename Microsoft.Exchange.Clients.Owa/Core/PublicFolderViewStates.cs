using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class PublicFolderViewStates : FolderViewStates
	{
		internal PublicFolderViewStates(UserContext userContext, Folder folder)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.cache = PublicFolderViewStatesCache.GetInstance(userContext);
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromStoreObject(folder);
			this.folderId = owaStoreObjectId.ToString();
		}

		private bool ExistsInCache
		{
			get
			{
				return this.cache.CacheEntryExists(this.folderId);
			}
		}

		private PublicFolderViewStatesEntry CacheEntryForGet
		{
			get
			{
				if (this.ExistsInCache)
				{
					PublicFolderViewStatesEntry publicFolderViewStatesEntry = this.cache[this.folderId];
					if (publicFolderViewStatesEntry != null)
					{
						publicFolderViewStatesEntry.UpdateLastAccessedDateTimeTicks();
						return publicFolderViewStatesEntry;
					}
				}
				return this.dummyEntry;
			}
		}

		private PublicFolderViewStatesEntry CacheEntryForSet
		{
			get
			{
				if (this.ExistsInCache)
				{
					PublicFolderViewStatesEntry publicFolderViewStatesEntry = this.cache[this.folderId];
					if (publicFolderViewStatesEntry != null)
					{
						publicFolderViewStatesEntry.UpdateLastAccessedDateTimeTicks();
						return publicFolderViewStatesEntry;
					}
				}
				PublicFolderViewStatesEntry publicFolderViewStatesEntry2 = new PublicFolderViewStatesEntry(this.folderId);
				this.cache.AddEntry(this.folderId, publicFolderViewStatesEntry2);
				publicFolderViewStatesEntry2.UpdateLastAccessedDateTimeTicks();
				return publicFolderViewStatesEntry2;
			}
		}

		internal override CalendarViewType CalendarViewType
		{
			get
			{
				return (CalendarViewType)this.CacheEntryForGet.CalendarViewType;
			}
			set
			{
				if (!FolderViewStates.ValidateCalendarViewType(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set CalendarViewType to invalid value.");
				}
				this.CacheEntryForSet.CalendarViewType = (int)value;
			}
		}

		internal override int DailyViewDays
		{
			get
			{
				return this.CacheEntryForGet.DailyViewDays;
			}
			set
			{
				if (!FolderViewStates.ValidateDailyViewDays(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set DailyViewDays to invalid value.");
				}
				this.CacheEntryForSet.DailyViewDays = value;
			}
		}

		internal override bool GetMultiLine(bool defaultValue)
		{
			if (this.ExistsInCache && this.CacheEntryForGet.MultiLine != null)
			{
				return this.CacheEntryForGet.MultiLine.Value;
			}
			return defaultValue;
		}

		internal override bool MultiLine
		{
			set
			{
				this.CacheEntryForSet.MultiLine = new bool?(value);
			}
		}

		internal override ReadingPanePosition GetReadingPanePosition(ReadingPanePosition defaultValue)
		{
			if (this.ExistsInCache)
			{
				int? readingPanePosition = this.CacheEntryForGet.ReadingPanePosition;
				if (readingPanePosition != null)
				{
					return (ReadingPanePosition)readingPanePosition.Value;
				}
			}
			return defaultValue;
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
					throw new ArgumentOutOfRangeException("value", "Cannot set ReadingPanePosition to invalid value.");
				}
				this.CacheEntryForSet.ReadingPanePosition = new int?((int)value);
			}
		}

		internal override ReadingPanePosition ReadingPanePositionMultiDay
		{
			get
			{
				return (ReadingPanePosition)this.CacheEntryForGet.ReadingPanePositionMultiDay;
			}
			set
			{
				if (!FolderViewStates.ValidateReadingPanePosition(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set ReadingPanePositionMultiDay to invalid value.");
				}
				this.CacheEntryForSet.ReadingPanePositionMultiDay = (int)value;
			}
		}

		internal override string GetSortColumn(string defaultValue)
		{
			if (this.ExistsInCache && this.CacheEntryForGet.SortColumn != null)
			{
				return this.CacheEntryForGet.SortColumn;
			}
			return defaultValue;
		}

		internal override string SortColumn
		{
			set
			{
				this.CacheEntryForSet.SortColumn = value;
			}
		}

		internal override SortOrder GetSortOrder(SortOrder defaultValue)
		{
			if (this.ExistsInCache && this.CacheEntryForGet.SortOrder != null)
			{
				return (SortOrder)this.CacheEntryForGet.SortOrder.Value;
			}
			return defaultValue;
		}

		internal override SortOrder SortOrder
		{
			set
			{
				if (!FolderViewStates.ValidateSortOrder(value))
				{
					throw new ArgumentOutOfRangeException("value", "Cannot set SortOrder to invalid value.");
				}
				this.CacheEntryForSet.SortOrder = new int?((int)value);
			}
		}

		internal override int GetViewHeight(int defaultViewHeight)
		{
			if (this.CacheEntryForGet.ViewHeight != null)
			{
				return this.CacheEntryForGet.ViewHeight.Value;
			}
			return defaultViewHeight;
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
				this.CacheEntryForSet.ViewHeight = new int?(value);
			}
		}

		internal override int GetViewWidth(int defaultViewWidth)
		{
			if (this.CacheEntryForGet.ViewWidth != null)
			{
				return this.CacheEntryForGet.ViewWidth.Value;
			}
			return defaultViewWidth;
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
				this.CacheEntryForSet.ViewWidth = new int?(value);
			}
		}

		internal override void Save()
		{
			this.cache.Commit();
		}

		private string folderId;

		private PublicFolderViewStatesCache cache;

		private PublicFolderViewStatesEntry dummyEntry = new PublicFolderViewStatesEntry();
	}
}
