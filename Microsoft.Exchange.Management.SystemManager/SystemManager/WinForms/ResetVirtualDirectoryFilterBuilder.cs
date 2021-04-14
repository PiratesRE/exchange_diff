using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ResetVirtualDirectoryFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			filter = null;
			preArgs = null;
			parameterList = ((!DBNull.Value.Equals(row["Server"])) ? string.Format("-Server '{0}'", row["Server"].ToQuotationEscapedString()) : null);
		}
	}
}
