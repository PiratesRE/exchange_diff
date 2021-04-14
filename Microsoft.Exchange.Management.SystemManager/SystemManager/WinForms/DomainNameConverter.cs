using System;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DomainNameConverter : ValueToDisplayObjectConverter
	{
		public object Convert(object valueObject)
		{
			if (valueObject != null)
			{
				return "@" + valueObject.ToString().ToLowerInvariant();
			}
			return string.Empty;
		}
	}
}
