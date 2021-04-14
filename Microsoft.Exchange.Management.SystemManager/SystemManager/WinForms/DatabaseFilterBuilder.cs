using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class DatabaseFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			filter = (((bool)row["IsExchange2007OrLaterOnly"]) ? " | Filter-PropertyEqualOrGreaterThan -Property ExchangeVersion -Value 0x72000000" : null);
			parameterList = null;
			preArgs = null;
		}
	}
}
