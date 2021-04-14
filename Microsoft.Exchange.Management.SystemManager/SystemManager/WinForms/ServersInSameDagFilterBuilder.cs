using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ServersInSameDagFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			string item = null;
			if (!DBNull.Value.Equals(row["DagMemberServer"]))
			{
				item = (string)row["DagMemberServer"];
			}
			filter = string.Format(" | Filter-ServersInSameDag -dagMemberServer '{0}'", item.ToQuotationEscapedString());
			preArgs = null;
			parameterList = null;
		}
	}
}
