using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IExchangeScopeBuilder
	{
		string BuildScope(object scope);
	}
}
