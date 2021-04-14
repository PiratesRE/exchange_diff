using System;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class CalendarDataSource : CalendarFolderDataSourceBase
	{
		public CalendarDataSource(UserContext userContext, CalendarFolder folder, DateRange[] dateRanges, PropertyDefinition[] properties) : base(dateRanges, properties)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.userContext = userContext;
			this.folder = folder;
			this.folderId = OwaStoreObjectId.CreateFromStoreObject(folder);
			base.Load((ExDateTime start, ExDateTime end) => folder.GetCalendarView(start, end, properties));
		}

		public override OwaStoreObjectId GetItemId(int index)
		{
			VersionedId versionedId;
			if (base.TryGetPropertyValue<VersionedId>(index, ItemSchema.Id, out versionedId))
			{
				return OwaStoreObjectId.CreateFromItemId(versionedId.ObjectId, this.folder);
			}
			ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Couldn't get id from the item, skipping...");
			return null;
		}

		public override string GetChangeKey(int index)
		{
			VersionedId versionedId;
			if (base.TryGetPropertyValue<VersionedId>(index, ItemSchema.Id, out versionedId))
			{
				return versionedId.ChangeKeyAsBase64String();
			}
			ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Couldn't get id from the item, skipping...");
			return null;
		}

		public override string GetCssClassName(int index)
		{
			string text = "noClrCal";
			bool flag = false;
			string[] categories = base.GetCategories(index);
			if (categories != null && 0 < categories.Length)
			{
				MasterCategoryList masterCategoryList = this.userContext.GetMasterCategoryList(this.folderId);
				if (masterCategoryList != null)
				{
					for (int i = 0; i < categories.Length; i++)
					{
						Category category = masterCategoryList[categories[i]];
						if (category != null && category.Color != -1)
						{
							flag = true;
							text = CategorySwatch.GetCategoryClassName(category);
							break;
						}
					}
				}
			}
			int num;
			if (!flag && base.TryGetPropertyValue<int>(index, CalendarItemBaseSchema.AppointmentColor, out num))
			{
				if (num < 0 || num >= CalendarDataSource.categoryColorOldLabelsToColors.Length)
				{
					num = 0;
				}
				text = CategorySwatch.GetCategoryClassNameFromColor(CalendarDataSource.categoryColorOldLabelsToColors[num]);
			}
			if (StringComparer.OrdinalIgnoreCase.Compare(text, "noClr") == 0)
			{
				text = "noClrCal";
			}
			return text;
		}

		public override SharedType SharedType
		{
			get
			{
				if (this.sharedType == null)
				{
					if (this.userContext.IsInOtherMailbox(this.folder))
					{
						this.sharedType = new SharedType?(SharedType.InternalFullDetail);
					}
					else if (Utilities.IsCrossOrgFolder(this.folder))
					{
						this.sharedType = new SharedType?(SharedType.CrossOrg);
					}
					else if (Utilities.IsWebCalendarFolder(this.folder))
					{
						this.sharedType = new SharedType?(SharedType.WebCalendar);
					}
					else
					{
						this.sharedType = new SharedType?(SharedType.None);
					}
				}
				return this.sharedType.Value;
			}
		}

		public override WorkingHours WorkingHours
		{
			get
			{
				if (this.workingHours == null)
				{
					if (Utilities.IsOtherMailbox(this.folder))
					{
						this.workingHours = this.userContext.GetOthersWorkingHours(this.folderId);
					}
					else
					{
						this.workingHours = this.userContext.WorkingHours;
					}
				}
				return this.workingHours;
			}
		}

		public override bool UserCanReadItem
		{
			get
			{
				return CalendarUtilities.UserHasRightToLoad(this.folder);
			}
		}

		public override bool UserCanCreateItem
		{
			get
			{
				return Utilities.CanCreateItemInFolder(this.folder) && this.SharedType != SharedType.CrossOrg && this.SharedType != SharedType.WebCalendar;
			}
		}

		public override string FolderClassName
		{
			get
			{
				return this.folder.ClassName;
			}
		}

		private static readonly int[] categoryColorOldLabelsToColors = new int[]
		{
			-1,
			0,
			7,
			4,
			12,
			1,
			22,
			21,
			8,
			5,
			3
		};

		private CalendarFolder folder;

		private UserContext userContext;

		private WorkingHours workingHours;

		private SharedType? sharedType;

		private OwaStoreObjectId folderId;
	}
}
