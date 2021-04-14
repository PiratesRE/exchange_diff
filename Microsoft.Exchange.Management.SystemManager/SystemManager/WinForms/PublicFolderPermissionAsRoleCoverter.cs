using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class PublicFolderPermissionAsRoleCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			if (Enum.IsDefined(typeof(PublicFolderPermissionRole), (int)arg))
			{
				return LocalizedDescriptionAttribute.FromEnum(typeof(PublicFolderPermissionRole), (int)arg);
			}
			if (PublicFolderPermission.None.Equals(arg))
			{
				return Strings.PublicFolderPermissionRoleNone;
			}
			return Strings.PublicFolderPermissionRoleCustom;
		}
	}
}
