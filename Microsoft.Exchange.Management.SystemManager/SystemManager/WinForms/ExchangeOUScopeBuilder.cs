using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeOUScopeBuilder : IExchangeScopeBuilder
	{
		public string BuildScope(object scope)
		{
			if (scope != null && !string.IsNullOrEmpty(scope.ToString()))
			{
				return string.Format("-OrganizationalUnit '{0}'", scope.ToQuotationEscapedString());
			}
			return null;
		}
	}
}
