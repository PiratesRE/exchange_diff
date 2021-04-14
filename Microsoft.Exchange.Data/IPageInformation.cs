using System;

namespace Microsoft.Exchange.Data
{
	internal interface IPageInformation
	{
		bool? MorePagesAvailable { get; }

		int PageSize { get; }
	}
}
