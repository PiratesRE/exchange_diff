using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ElcFoldeFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			preArgs = null;
			ElcFolderFilter elcFolderFilter = ElcFolderFilter.All;
			if (!DBNull.Value.Equals(row["FolderFilter"]))
			{
				elcFolderFilter = (ElcFolderFilter)row["FolderFilter"];
			}
			switch (elcFolderFilter)
			{
			case ElcFolderFilter.Default:
				filter = " | Filter-PropertyNotEqualTo -Property 'FolderType' -Value 'ManagedCustomFolder'";
				break;
			case ElcFolderFilter.Organizational:
				filter = " | Filter-PropertyEqualTo -Property 'FolderType' -Value 'ManagedCustomFolder'";
				break;
			default:
				filter = null;
				break;
			}
			parameterList = null;
		}
	}
}
