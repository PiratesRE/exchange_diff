using System;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	internal interface IPageableTask
	{
		int Page { get; set; }

		int PageSize { get; set; }
	}
}
