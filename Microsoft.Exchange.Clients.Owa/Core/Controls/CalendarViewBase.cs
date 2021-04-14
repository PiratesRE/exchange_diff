using System;
using System.Collections;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal abstract class CalendarViewBase
	{
		protected CalendarViewBase(ISessionContext sessionContext, CalendarAdapterBase calendarAdapter)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			this.sessionContext = sessionContext;
			this.CalendarAdapter = calendarAdapter;
		}

		public ISessionContext SessionContext
		{
			get
			{
				return this.sessionContext;
			}
		}

		public ICalendarDataSource DataSource
		{
			get
			{
				if (this.CalendarAdapter != null)
				{
					return this.CalendarAdapter.DataSource;
				}
				return null;
			}
		}

		public CalendarAdapterBase CalendarAdapter { get; private set; }

		public DateRange[] DateRanges
		{
			get
			{
				if (this.CalendarAdapter != null)
				{
					return this.CalendarAdapter.DateRanges;
				}
				return null;
			}
		}

		public int DayCount
		{
			get
			{
				return this.DateRanges.Length;
			}
		}

		public abstract int MaxEventAreaRows { get; }

		public abstract int MaxItemsPerView { get; }

		public void RemoveItemFromView(int dataIndex)
		{
			if (this.removedItems == null)
			{
				this.removedItems = new Hashtable();
			}
			this.removedItems[dataIndex] = true;
		}

		public bool IsItemRemoved(int dataIndex)
		{
			if (this.removedItems == null)
			{
				return false;
			}
			object obj = this.removedItems[dataIndex];
			return null != obj;
		}

		public int RemovedItemCount
		{
			get
			{
				if (this.removedItems == null)
				{
					return 0;
				}
				return this.removedItems.Count;
			}
		}

		protected Hashtable RemovedItems
		{
			get
			{
				return this.removedItems;
			}
		}

		public abstract SanitizedHtmlString FolderDateDescription { get; }

		private ISessionContext sessionContext;

		private Hashtable removedItems;
	}
}
