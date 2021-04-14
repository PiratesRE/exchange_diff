using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public enum ElcFolderFilter
	{
		[LocDescription(Strings.IDs.ELCFolderTypeAll)]
		All,
		[LocDescription(Strings.IDs.ELCFolderTypeDefault)]
		Default,
		[LocDescription(Strings.IDs.ELCFolderTypeOrganizational)]
		Organizational
	}
}
