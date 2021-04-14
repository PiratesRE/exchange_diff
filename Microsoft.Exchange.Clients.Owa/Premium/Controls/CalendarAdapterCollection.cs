using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class CalendarAdapterCollection : DisposeTrackableBase
	{
		internal CalendarAdapterCollection(UserContext userContext, OwaStoreObjectId[] folderIds, CalendarViewType viewType)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderIds == null)
			{
				throw new ArgumentNullException("folderIds");
			}
			if (folderIds.Length == 0)
			{
				throw new ArgumentException("Length of folderIds cannot be 0");
			}
			this.userContext = userContext;
			this.folderIds = folderIds;
			this.PropertyFolderId = (folderIds[0].IsPublic ? folderIds[0] : userContext.CalendarFolderOwaId);
			this.propertyFolder = Utilities.GetFolderForContent<CalendarFolder>(userContext, this.PropertyFolderId, CalendarUtilities.FolderViewProperties);
			this.folderViewStates = userContext.GetFolderViewStates(this.propertyFolder);
			int viewWidth;
			ReadingPanePosition readingPanePosition;
			CalendarUtilities.GetCalendarViewParamsFromViewStates(this.folderViewStates, out viewWidth, ref viewType, out readingPanePosition);
			this.ViewWidth = viewWidth;
			this.ViewType = viewType;
			this.ReadingPanePosition = readingPanePosition;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.propertyFolder != null)
				{
					this.propertyFolder.Dispose();
					this.propertyFolder = null;
				}
				if (this.adapters != null)
				{
					foreach (CalendarAdapter calendarAdapter in this.adapters)
					{
						calendarAdapter.Dispose();
					}
					this.adapters = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarAdapterCollection>(this);
		}

		internal void SaveViewStates(ExDateTime[] days)
		{
			this.folderViewStates.CalendarViewType = this.ViewType;
			this.folderViewStates.DailyViewDays = ((days != null && this.ViewType == CalendarViewType.Min) ? days.Length : 1);
			this.folderViewStates.Save();
		}

		internal CalendarAdapter[] GetAdapters(ExDateTime[] days, bool addOwaConditionAdvisor)
		{
			if (days == null)
			{
				throw new ArgumentNullException("days");
			}
			if (days.Length == 0)
			{
				throw new ArgumentException("Length of days cannot be 0.");
			}
			if (this.adapters == null)
			{
				List<CalendarAdapter> list = new List<CalendarAdapter>();
				List<CalendarAdapter> list2 = new List<CalendarAdapter>();
				foreach (OwaStoreObjectId owaStoreObjectId in this.folderIds)
				{
					CalendarAdapter calendarAdapter;
					if (owaStoreObjectId.Equals(this.PropertyFolderId))
					{
						calendarAdapter = new CalendarAdapter(this.userContext, this.propertyFolder);
					}
					else
					{
						calendarAdapter = new CalendarAdapter(this.userContext, owaStoreObjectId);
					}
					try
					{
						calendarAdapter.LoadData(CalendarUtilities.QueryProperties, days, addOwaConditionAdvisor, false);
					}
					catch (Exception)
					{
						calendarAdapter.Dispose();
						calendarAdapter = null;
						list.AddRange(list2);
						foreach (CalendarAdapter calendarAdapter2 in list)
						{
							if (calendarAdapter2 != null)
							{
								calendarAdapter2.Dispose();
							}
						}
						throw;
					}
					if (calendarAdapter.DataSource is AvailabilityDataSource)
					{
						list2.Add(calendarAdapter);
					}
					else
					{
						list.Add(calendarAdapter);
					}
				}
				if (list2.Count > 0)
				{
					CalendarAdapter[] array2 = list2.ToArray();
					FreeBusyQueryResult[] array3 = AvailabilityDataSource.BatchLoadData(this.userContext, array2, CalendarAdapterBase.ConvertDateTimeArrayToDateRangeArray(days));
					if (array3 != null)
					{
						for (int j = 0; j < array2.Length; j++)
						{
							AvailabilityDataSource availabilityDataSource = (AvailabilityDataSource)array2[j].DataSource;
							availabilityDataSource.LoadFromQueryResult(array3[j]);
						}
					}
					list.AddRange(list2);
				}
				this.adapters = list.ToArray();
			}
			foreach (CalendarAdapter calendarAdapter3 in this.adapters)
			{
				calendarAdapter3.SaveCalendarTypeFromOlderExchangeAsNeeded();
			}
			return this.adapters;
		}

		internal int ViewWidth { get; set; }

		internal CalendarViewType ViewType { get; set; }

		internal ReadingPanePosition ReadingPanePosition { get; set; }

		internal OwaStoreObjectId PropertyFolderId { get; private set; }

		internal FolderViewStates FolderViewStates
		{
			get
			{
				return this.folderViewStates;
			}
		}

		private UserContext userContext;

		private OwaStoreObjectId[] folderIds;

		private CalendarFolder propertyFolder;

		private CalendarAdapter[] adapters;

		private FolderViewStates folderViewStates;
	}
}
