using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum DiagnosticSource : uint
	{
		Base,
		Task,
		Rpc,
		Mapi,
		Admin
	}
}
