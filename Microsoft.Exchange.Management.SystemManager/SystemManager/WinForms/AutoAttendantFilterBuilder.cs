using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class AutoAttendantFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			ADObjectId adobjectId = row["UMDialPlan"] as ADObjectId;
			filter = ((adobjectId != null) ? string.Format(" | Filter-PropertyEqualTo -Property UMDialPlan -Value '{0}'", adobjectId.ObjectGuid.ToQuotationEscapedString()) : null);
			parameterList = null;
			preArgs = null;
		}
	}
}
