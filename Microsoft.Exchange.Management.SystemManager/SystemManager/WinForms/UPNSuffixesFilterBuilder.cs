using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class UPNSuffixesFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			ADObjectId adobjectId = null;
			if (row.Table.Columns.Contains("OrganizationalUnit") && !DBNull.Value.Equals(row["OrganizationalUnit"]))
			{
				adobjectId = (ADObjectId)row["OrganizationalUnit"];
			}
			filter = null;
			preArgs = null;
			parameterList = ((adobjectId != null) ? string.Format("-OrganizationalUnit '{0}'", adobjectId.ToQuotationEscapedString()) : null);
		}
	}
}
