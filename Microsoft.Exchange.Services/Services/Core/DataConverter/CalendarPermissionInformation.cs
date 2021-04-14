using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CalendarPermissionInformation : PermissionInformationBase<CalendarPermissionType>
	{
		public CalendarPermissionInformation()
		{
		}

		public CalendarPermissionInformation(CalendarPermissionType permissionElement) : base(permissionElement)
		{
			this.permissionLevel = permissionElement.CalendarPermissionLevel;
			this.freeBusyAccess = this.ComputeFreeBusyAccess();
		}

		internal FreeBusyAccess? FreeBusyAccess
		{
			get
			{
				return this.freeBusyAccess;
			}
			set
			{
				this.freeBusyAccess = value;
			}
		}

		internal CalendarPermissionLevelType CalendarPermissionLevel
		{
			get
			{
				return this.permissionLevel;
			}
			set
			{
				this.permissionLevel = value;
			}
		}

		internal override bool DoAnyNonPermissionLevelFieldsHaveValue()
		{
			return base.DoAnyNonPermissionLevelFieldsHaveValue() || this.FreeBusyAccess != null;
		}

		private static PermissionLevel ConvertCalendarPermissionLevel(CalendarPermissionLevelType calendarPermissionLevel)
		{
			if (calendarPermissionLevel == CalendarPermissionLevelType.FreeBusyTimeOnly || calendarPermissionLevel == CalendarPermissionLevelType.FreeBusyTimeAndSubjectAndLocation)
			{
				return PermissionLevel.Custom;
			}
			return (PermissionLevel)calendarPermissionLevel;
		}

		protected override PermissionLevel GetPermissionLevelToSet()
		{
			return CalendarPermissionInformation.ConvertCalendarPermissionLevel(this.CalendarPermissionLevel);
		}

		internal override bool IsNonCustomPermissionLevelSet()
		{
			return this.CalendarPermissionLevel != CalendarPermissionLevelType.Custom;
		}

		protected override void SetByTypePermissionFieldsOntoPermission(Permission permission)
		{
			CalendarFolderPermission calendarFolderPermission = (CalendarFolderPermission)permission;
			if (this.FreeBusyAccess != null)
			{
				calendarFolderPermission.FreeBusyAccess = this.FreeBusyAccess.Value;
			}
			else
			{
				switch (this.CalendarPermissionLevel)
				{
				case CalendarPermissionLevelType.Owner:
				case CalendarPermissionLevelType.PublishingEditor:
				case CalendarPermissionLevelType.Editor:
				case CalendarPermissionLevelType.PublishingAuthor:
				case CalendarPermissionLevelType.Author:
				case CalendarPermissionLevelType.NoneditingAuthor:
				case CalendarPermissionLevelType.Reviewer:
					calendarFolderPermission.FreeBusyAccess = Microsoft.Exchange.Data.Storage.FreeBusyAccess.Details;
					goto IL_7A;
				case CalendarPermissionLevelType.FreeBusyTimeOnly:
				case CalendarPermissionLevelType.FreeBusyTimeAndSubjectAndLocation:
					goto IL_7A;
				}
				calendarFolderPermission.FreeBusyAccess = Microsoft.Exchange.Data.Storage.FreeBusyAccess.None;
			}
			IL_7A:
			if (this.CalendarPermissionLevel == CalendarPermissionLevelType.FreeBusyTimeOnly)
			{
				calendarFolderPermission.CanReadItems = false;
				calendarFolderPermission.CanCreateItems = false;
				calendarFolderPermission.CanCreateSubfolders = false;
				calendarFolderPermission.IsFolderOwner = false;
				calendarFolderPermission.IsFolderVisible = false;
				calendarFolderPermission.IsFolderContact = false;
				calendarFolderPermission.EditItems = ItemPermissionScope.None;
				calendarFolderPermission.DeleteItems = ItemPermissionScope.None;
				calendarFolderPermission.FreeBusyAccess = Microsoft.Exchange.Data.Storage.FreeBusyAccess.Basic;
				return;
			}
			if (this.CalendarPermissionLevel == CalendarPermissionLevelType.FreeBusyTimeAndSubjectAndLocation)
			{
				calendarFolderPermission.CanReadItems = false;
				calendarFolderPermission.CanCreateItems = false;
				calendarFolderPermission.CanCreateSubfolders = false;
				calendarFolderPermission.IsFolderOwner = false;
				calendarFolderPermission.IsFolderVisible = false;
				calendarFolderPermission.IsFolderContact = false;
				calendarFolderPermission.EditItems = ItemPermissionScope.None;
				calendarFolderPermission.DeleteItems = ItemPermissionScope.None;
				calendarFolderPermission.FreeBusyAccess = Microsoft.Exchange.Data.Storage.FreeBusyAccess.Details;
			}
		}

		protected override CalendarPermissionType CreateDefaultBasePermissionType()
		{
			return new CalendarPermissionType();
		}

		internal override bool? CanReadItems
		{
			get
			{
				base.EnsurePermissionElementIsNotNull();
				if (base.PermissionElement.ReadItems != null)
				{
					return new bool?(base.PermissionElement.ReadItems.Value == CalendarPermissionReadAccess.FullDetails);
				}
				return null;
			}
			set
			{
				base.EnsurePermissionElementIsNotNull();
				if (value == null)
				{
					base.PermissionElement.ReadItems = null;
					return;
				}
				if (value.Value)
				{
					base.PermissionElement.ReadItems = new CalendarPermissionReadAccess?(CalendarPermissionReadAccess.FullDetails);
					return;
				}
				base.PermissionElement.ReadItems = new CalendarPermissionReadAccess?(CalendarPermissionReadAccess.None);
			}
		}

		private FreeBusyAccess? ComputeFreeBusyAccess()
		{
			if (base.PermissionElement.ReadItems != null)
			{
				switch (base.PermissionElement.ReadItems.Value)
				{
				case CalendarPermissionReadAccess.TimeOnly:
					return new FreeBusyAccess?(Microsoft.Exchange.Data.Storage.FreeBusyAccess.Basic);
				case CalendarPermissionReadAccess.TimeAndSubjectAndLocation:
					return new FreeBusyAccess?(Microsoft.Exchange.Data.Storage.FreeBusyAccess.Details);
				case CalendarPermissionReadAccess.FullDetails:
					return new FreeBusyAccess?(Microsoft.Exchange.Data.Storage.FreeBusyAccess.Details);
				}
				return new FreeBusyAccess?(Microsoft.Exchange.Data.Storage.FreeBusyAccess.None);
			}
			return null;
		}

		private FreeBusyAccess? freeBusyAccess;

		private CalendarPermissionLevelType permissionLevel;
	}
}
