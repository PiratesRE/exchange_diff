using System;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class PublishedCalendarDataSource : CalendarFolderDataSourceBase
	{
		public PublishedCalendarDataSource(AnonymousSessionContext sessionContext, PublishedCalendar folder, DateRange[] dateRanges, PropertyDefinition[] properties) : base(dateRanges, properties)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.sessionContext = sessionContext;
			this.folder = folder;
			base.Load((ExDateTime start, ExDateTime end) => folder.GetCalendarView(start, end, properties));
		}

		public override OwaStoreObjectId GetItemId(int index)
		{
			return null;
		}

		public StoreObjectId GetItemStoreObjectId(int index)
		{
			VersionedId versionedId;
			if (base.TryGetPropertyValue<VersionedId>(index, ItemSchema.Id, out versionedId))
			{
				return versionedId.ObjectId;
			}
			ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Couldn't get id from the item, skipping...");
			return null;
		}

		public override string GetChangeKey(int index)
		{
			return string.Empty;
		}

		public override bool IsPrivate(int index)
		{
			return this.DetailLevel != DetailLevelEnumType.AvailabilityOnly && base.IsPrivate(index);
		}

		public override string GetOrganizerDisplayName(int index)
		{
			return string.Empty;
		}

		public override string GetCssClassName(int index)
		{
			return "noClrCal";
		}

		public override bool HasAttachment(int index)
		{
			return false;
		}

		public PublishedCalendarItemData? GetItem(int index)
		{
			if (this.IsPrivate(index))
			{
				return null;
			}
			return new PublishedCalendarItemData?(this.folder.GetItemData(this.GetItemStoreObjectId(index)));
		}

		public override SharedType SharedType
		{
			get
			{
				return SharedType.AnonymousAccess;
			}
		}

		public override WorkingHours WorkingHours
		{
			get
			{
				return this.sessionContext.WorkingHours;
			}
		}

		public override bool UserCanReadItem
		{
			get
			{
				return true;
			}
		}

		public override bool UserCanCreateItem
		{
			get
			{
				return false;
			}
		}

		public override string FolderClassName
		{
			get
			{
				return "IPF.Appointment";
			}
		}

		public DetailLevelEnumType DetailLevel
		{
			get
			{
				return this.folder.DetailLevel;
			}
		}

		private AnonymousSessionContext sessionContext;

		private PublishedCalendar folder;
	}
}
