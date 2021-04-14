using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class SecurityPrincipalFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			filter = null;
			preArgs = null;
			parameterList = null;
			if (!DBNull.Value.Equals(row["IncludeDomainLocalFrom"]))
			{
				SmtpDomain item = (SmtpDomain)row["IncludeDomainLocalFrom"];
				parameterList = string.Format("-IncludeDomainLocalFrom '{0}'", item.ToQuotationEscapedString());
			}
		}
	}
}
