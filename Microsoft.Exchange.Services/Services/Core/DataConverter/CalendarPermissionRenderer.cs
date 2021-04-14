using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CalendarPermissionRenderer : PermissionRendererBase<CalendarFolderPermission, CalendarPermissionSetType, CalendarPermissionType>
	{
		internal CalendarPermissionRenderer(CalendarFolder calendarFolder)
		{
			this.calendarPermissionSet = CalendarPermissionRenderer.GetPermissionSet(calendarFolder);
		}

		protected override CalendarFolderPermission GetDefaultPermission()
		{
			return this.calendarPermissionSet.DefaultPermission;
		}

		protected override CalendarFolderPermission GetAnonymousPermission()
		{
			return (CalendarFolderPermission)this.calendarPermissionSet.AnonymousPermission;
		}

		protected override IEnumerator<CalendarFolderPermission> GetPermissionEnumerator()
		{
			return this.calendarPermissionSet.GetEnumerator();
		}

		protected override string GetPermissionsArrayElementName()
		{
			return "CalendarPermissions";
		}

		protected override string GetPermissionElementName()
		{
			return "CalendarPermission";
		}

		private CalendarPermissionLevelType CreatePermissionLevel(CalendarFolderPermission permission)
		{
			switch (permission.PermissionLevel)
			{
			case PermissionLevel.None:
				return CalendarPermissionLevelType.None;
			case PermissionLevel.Owner:
				return CalendarPermissionLevelType.Owner;
			case PermissionLevel.PublishingEditor:
				return CalendarPermissionLevelType.PublishingEditor;
			case PermissionLevel.Editor:
				return CalendarPermissionLevelType.Editor;
			case PermissionLevel.PublishingAuthor:
				return CalendarPermissionLevelType.PublishingAuthor;
			case PermissionLevel.Author:
				return CalendarPermissionLevelType.Author;
			case PermissionLevel.NonEditingAuthor:
				return CalendarPermissionLevelType.NoneditingAuthor;
			case PermissionLevel.Reviewer:
				return CalendarPermissionLevelType.Reviewer;
			case PermissionLevel.Contributor:
				return CalendarPermissionLevelType.Contributor;
			default:
				if (!permission.CanCreateItems && !permission.CanCreateSubfolders && !permission.CanReadItems && permission.DeleteItems == ItemPermissionScope.None && permission.EditItems == ItemPermissionScope.None && !permission.IsFolderOwner)
				{
					if (permission.FreeBusyAccess == FreeBusyAccess.Basic)
					{
						return CalendarPermissionLevelType.FreeBusyTimeOnly;
					}
					if (permission.FreeBusyAccess == FreeBusyAccess.Details)
					{
						return CalendarPermissionLevelType.FreeBusyTimeAndSubjectAndLocation;
					}
				}
				return CalendarPermissionLevelType.Custom;
			}
		}

		private CalendarPermissionReadAccess CreateCalendarPermissionReadAccessType(bool canReadItems, FreeBusyAccess freeBusyAccess)
		{
			if (canReadItems)
			{
				return CalendarPermissionReadAccess.FullDetails;
			}
			if (FreeBusyAccess.Details == freeBusyAccess)
			{
				return CalendarPermissionReadAccess.TimeAndSubjectAndLocation;
			}
			if (FreeBusyAccess.Basic == freeBusyAccess)
			{
				return CalendarPermissionReadAccess.TimeOnly;
			}
			return CalendarPermissionReadAccess.None;
		}

		protected override void RenderByTypePermissionDetails(CalendarPermissionType permissionType, CalendarFolderPermission permission)
		{
			permissionType.ReadItems = new CalendarPermissionReadAccess?(this.CreateCalendarPermissionReadAccessType(permission.CanReadItems, permission.FreeBusyAccess));
			permissionType.CalendarPermissionLevel = this.CreatePermissionLevel(permission);
		}

		internal static CalendarFolderPermissionSet GetPermissionSet(CalendarFolder folder)
		{
			CalendarFolderPermissionSet permissionSet;
			try
			{
				FaultInjection.GenerateFault((FaultInjection.LIDs)3024497981U);
				permissionSet = folder.GetPermissionSet();
			}
			catch (StoragePermanentException ex)
			{
				if (ex.InnerException is MapiExceptionAmbiguousAlias)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<StoragePermanentException>(0L, "Error occurred when fetching permission set for calendar folder. Exception '{0}'.", ex);
					throw new ObjectCorruptException(ex, false);
				}
				throw;
			}
			return permissionSet;
		}

		protected override CalendarPermissionType CreatePermissionElement()
		{
			return new CalendarPermissionType();
		}

		protected override void SetPermissionsOnSerializationObject(CalendarPermissionSetType serviceProperty, List<CalendarPermissionType> renderedPermissions)
		{
			serviceProperty.CalendarPermissions = renderedPermissions.ToArray();
		}

		protected override void SetUnknownEntriesOnSerializationObject(CalendarPermissionSetType serviceProperty, string[] entries)
		{
			serviceProperty.UnknownEntries = entries;
		}

		protected override CalendarPermissionSetType CreatePermissionSetElement()
		{
			return new CalendarPermissionSetType();
		}

		private CalendarFolderPermissionSet calendarPermissionSet;
	}
}
