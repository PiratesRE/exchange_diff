using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class UMDialPlanFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			filter = null;
			parameterList = null;
			preArgs = null;
			if (true.Equals(row["ExcludeSipNameURIType"]))
			{
				filter = " | Filter-PropertyNotEqualTo -Property URIType -Value SipName";
			}
		}
	}
}
