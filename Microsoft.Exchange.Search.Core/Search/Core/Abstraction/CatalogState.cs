using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal enum CatalogState
	{
		Unknown,
		Healthy,
		Seeding,
		Suspended,
		Failed
	}
}
