using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class RecipientPreviewFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			QueryFilter queryFilter = (QueryFilter)row["RecipientPreviewFilter"];
			filter = null;
			preArgs = null;
			parameterList = "-RecipientPreviewFilter '" + LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter).ToQuotationEscapedString() + "'";
		}
	}
}
