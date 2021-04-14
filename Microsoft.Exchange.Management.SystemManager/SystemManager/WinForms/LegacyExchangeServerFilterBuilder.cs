using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class LegacyExchangeServerFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			filter = "| where {-not($_.IsExchange2007OrLater)}";
			parameterList = null;
			preArgs = null;
		}
	}
}
