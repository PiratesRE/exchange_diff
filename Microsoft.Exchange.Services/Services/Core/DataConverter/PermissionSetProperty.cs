using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PermissionSetProperty : PermissionSetPropertyBase<PermissionType>, IToServiceObjectCommand, IPropertyCommand
	{
		public PermissionSetProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static PermissionSetProperty CreateCommand(CommandContext commandContext)
		{
			return new PermissionSetProperty(commandContext);
		}

		void IToServiceObjectCommand.ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			Folder folder = commandSettings.StoreObject as Folder;
			BaseFolderType baseFolderType = commandSettings.ServiceObject as BaseFolderType;
			FolderType folderType = baseFolderType as FolderType;
			ContactsFolderType contactsFolderType = baseFolderType as ContactsFolderType;
			if (folderType != null)
			{
				folderType.PermissionSet = new PermissionRenderer(folder).Render();
				return;
			}
			if (contactsFolderType != null)
			{
				contactsFolderType.PermissionSet = new PermissionRenderer(folder).Render();
			}
		}

		protected override void ConfirmFolderIsProperType(Folder folder)
		{
			if (folder is CalendarFolder)
			{
				throw new CannotSetCalendarPermissionOnNonCalendarFolderException();
			}
		}

		protected override PermissionInformationBase<PermissionType>[] ParsePermissions(BasePermissionSetType permissionSet, Folder folder)
		{
			List<PermissionInformation> list = new List<PermissionInformation>();
			foreach (PermissionType permissionType in (permissionSet as PermissionSetType).Permissions)
			{
				list.Add(new PermissionInformation(permissionType));
			}
			return list.ToArray();
		}

		protected override BasePermissionSetType GetPermissionSet(BaseFolderType serviceObject)
		{
			FolderType folderType = serviceObject as FolderType;
			ContactsFolderType contactsFolderType = serviceObject as ContactsFolderType;
			if (folderType != null)
			{
				return folderType.PermissionSet;
			}
			if (contactsFolderType != null)
			{
				return contactsFolderType.PermissionSet;
			}
			throw new InvalidOperationException("[PermissionSetProperty::GetPermissionSet]");
		}
	}
}
