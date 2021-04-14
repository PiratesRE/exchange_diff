using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class OrganizationalUnitScopeBuilder : IExchangeScopeBuilder
	{
		public string BuildScope(object scope)
		{
			if (scope == null || string.IsNullOrEmpty(scope.ToString()))
			{
				return "-SingleNodeOnly";
			}
			if (scope is DataRow)
			{
				ADObjectId item = (ADObjectId)(scope as DataRow)["Identity"];
				return string.Format("-Identity '{0}' -SingleNodeOnly", item.ToQuotationEscapedString());
			}
			return string.Format("-Identity '{0}'", scope.ToQuotationEscapedString());
		}
	}
}
