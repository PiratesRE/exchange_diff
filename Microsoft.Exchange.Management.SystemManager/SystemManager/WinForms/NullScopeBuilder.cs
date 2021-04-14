using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class NullScopeBuilder : IExchangeScopeBuilder
	{
		public string BuildScope(object scope)
		{
			return string.Empty;
		}
	}
}
