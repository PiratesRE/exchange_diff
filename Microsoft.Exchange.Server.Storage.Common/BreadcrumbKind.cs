using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public enum BreadcrumbKind
	{
		None,
		Exception,
		RopError,
		Abort,
		AdminError
	}
}
