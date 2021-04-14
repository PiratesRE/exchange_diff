using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class AddressListScopeBuilder : IExchangeScopeBuilder
	{
		public string BuildScope(object scope)
		{
			if (scope == null || string.IsNullOrEmpty(scope.ToString()))
			{
				return "\\";
			}
			if (scope is DataRow)
			{
				scope = (ADObjectId)(scope as DataRow)["Identity"];
			}
			return string.Format("-Container '{0}'", scope.ToQuotationEscapedString());
		}
	}
}
