using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum ScriptFlags
	{
		None = 0,
		IncludeUglobal = 1,
		DeferredLoading = 2
	}
}
