using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ADObjectIdToNameConverter : ValueToDisplayObjectConverter
	{
		public object Convert(object valueObject)
		{
			ADObjectId adobjectId = (ADObjectId)valueObject;
			return adobjectId.Name;
		}
	}
}
