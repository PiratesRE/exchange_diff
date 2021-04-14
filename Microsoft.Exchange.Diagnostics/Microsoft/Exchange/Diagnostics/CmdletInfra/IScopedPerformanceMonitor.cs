using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal interface IScopedPerformanceMonitor
	{
		bool Start(ScopeInfo scopeInfo);

		void End(ScopeInfo scopeInfo);
	}
}
