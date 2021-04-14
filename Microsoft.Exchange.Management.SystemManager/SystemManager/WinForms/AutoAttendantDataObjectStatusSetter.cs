using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class AutoAttendantDataObjectStatusSetter : IPropertySetter
	{
		public void Set(object dataObject, object value)
		{
			((UMAutoAttendant)dataObject).SetStatus((StatusEnum)value);
		}
	}
}
