using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CalendarPermissionSetProperty : PermissionSetPropertyBase<CalendarPermissionType>, IToServiceObjectCommand, IPropertyCommand
	{
		public CalendarPermissionSetProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static CalendarPermissionSetProperty CreateCommand(CommandContext commandContext)
		{
			return new CalendarPermissionSetProperty(commandContext);
		}

		void IToServiceObjectCommand.ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			CalendarFolder calendarFolder = commandSettings.StoreObject as CalendarFolder;
			CalendarFolderType calendarFolderType = commandSettings.ServiceObject as CalendarFolderType;
			if (calendarFolderType != null)
			{
				calendarFolderType.PermissionSet = new CalendarPermissionRenderer(calendarFolder).Render();
			}
		}

		protected override void ConfirmFolderIsProperType(Folder folder)
		{
			if (!(folder is CalendarFolder))
			{
				throw new CannotSetNonCalendarPermissionOnCalendarFolderException();
			}
		}

		protected override BasePermissionSetType GetPermissionSet(BaseFolderType serviceObject)
		{
			return (serviceObject as CalendarFolderType).PermissionSet;
		}

		protected override PermissionInformationBase<CalendarPermissionType>[] ParsePermissions(BasePermissionSetType permissionSet, Folder folder)
		{
			List<CalendarPermissionInformation> list = new List<CalendarPermissionInformation>();
			foreach (CalendarPermissionType permissionElement in (permissionSet as CalendarPermissionSetType).CalendarPermissions)
			{
				list.Add(new CalendarPermissionInformation(permissionElement));
			}
			return list.ToArray();
		}
	}
}
