using System;
using System.Net;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class PublishedCalendarAdapter : CalendarAdapterBase
	{
		public PublishedCalendarAdapter(AnonymousSessionContext sessionContext)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			this.SessionContext = sessionContext;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.publishedFolder != null)
			{
				this.publishedFolder.Dispose();
				this.publishedFolder = null;
			}
		}

		public void LoadData(PropertyDefinition[] queryProperties, ExDateTime[] days, CalendarViewType viewType)
		{
			this.LoadData(queryProperties, days, viewType, null);
		}

		public void LoadData(PropertyDefinition[] queryProperties, ExDateTime[] days, CalendarViewType viewType, ExTimeZone preferredTimeZone)
		{
			this.LoadData(queryProperties, days, 0, 24, viewType, preferredTimeZone);
		}

		public void LoadData(PropertyDefinition[] queryProperties, ExDateTime[] days, int startHour, int endHour, CalendarViewType viewType)
		{
			this.LoadData(queryProperties, days, startHour, endHour, viewType, null);
		}

		public void LoadData(PropertyDefinition[] queryProperties, ExDateTime[] days, int startHour, int endHour, CalendarViewType viewType, ExTimeZone preferredTimeZone)
		{
			if (queryProperties == null || queryProperties.Length == 0)
			{
				throw new ArgumentNullException("queryProperties");
			}
			days = CalendarUtilities.GetViewDaysForPublishedView(this.SessionContext, days, viewType);
			try
			{
				this.publishedFolder = (PublishedCalendar)PublishedFolder.Create(this.SessionContext.PublishingUrl);
			}
			catch (PublishedFolderAccessDeniedException innerException)
			{
				throw new OwaInvalidRequestException("Cannot open published folder", innerException);
			}
			catch (NotSupportedException innerException2)
			{
				throw new OwaInvalidRequestException("Cannot open published folder", innerException2);
			}
			if (preferredTimeZone != null)
			{
				this.SessionContext.TimeZone = preferredTimeZone;
				this.publishedFolder.TimeZone = preferredTimeZone;
				CalendarUtilities.AdjustTimesWithTimeZone(days, preferredTimeZone);
			}
			else if (this.SessionContext.IsTimeZoneFromCookie)
			{
				this.publishedFolder.TimeZone = this.SessionContext.TimeZone;
			}
			else
			{
				this.SessionContext.TimeZone = this.publishedFolder.TimeZone;
				CalendarUtilities.AdjustTimesWithTimeZone(days, this.SessionContext.TimeZone);
			}
			base.DateRanges = CalendarAdapterBase.ConvertDateTimeArrayToDateRangeArray(days, startHour, endHour);
			try
			{
				base.DataSource = new PublishedCalendarDataSource(this.SessionContext, this.publishedFolder, base.DateRanges, queryProperties);
			}
			catch (FolderNotPublishedException)
			{
				Utilities.EndResponse(OwaContext.Current.HttpContext, HttpStatusCode.NotFound);
			}
			base.CalendarTitle = string.Format("{0} ({1})", this.publishedFolder.DisplayName, this.publishedFolder.OwnerDisplayName);
		}

		public AnonymousSessionContext SessionContext { get; private set; }

		public override string CalendarOwnerDisplayName
		{
			get
			{
				if (this.publishedFolder == null)
				{
					throw new InvalidOperationException("Need to call PublishedCalendarAdapter.LoadData first.");
				}
				return this.publishedFolder.OwnerDisplayName;
			}
		}

		public override string IdentityString
		{
			get
			{
				return "PublishedCalendar";
			}
		}

		public string ICalUrl
		{
			get
			{
				if (this.publishedFolder == null)
				{
					throw new InvalidOperationException("Need to call PublishedCalendarAdapter.LoadData first.");
				}
				return this.publishedFolder.ICalUrl.ToString();
			}
		}

		public ExDateTime PublishedFromDateTime
		{
			get
			{
				return this.publishedFolder.PublishedFromDateTime;
			}
		}

		public ExDateTime PublishedToDateTime
		{
			get
			{
				return this.publishedFolder.PublishedToDateTime;
			}
		}

		private PublishedCalendar publishedFolder;
	}
}
