using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class DAGNetworkFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			ADObjectId adobjectId = row["Server"] as ADObjectId;
			filter = null;
			parameterList = ((adobjectId != null) ? string.Format(" -Server '{0}'", adobjectId.ToQuotationEscapedString()) : null);
			preArgs = null;
		}
	}
}
