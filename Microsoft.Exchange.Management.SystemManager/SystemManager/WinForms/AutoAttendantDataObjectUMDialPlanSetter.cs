using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class AutoAttendantDataObjectUMDialPlanSetter : IPropertySetter
	{
		public void Set(object dataObject, object value)
		{
			((UMAutoAttendant)dataObject).SetDialPlan((ADObjectId)value);
		}
	}
}
