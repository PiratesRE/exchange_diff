using System;
using System.IO;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FullPathToFileNameObjectConverter : ValueToDisplayObjectConverter
	{
		public object Convert(object valueObject)
		{
			return Path.GetFileName((string)valueObject);
		}
	}
}
