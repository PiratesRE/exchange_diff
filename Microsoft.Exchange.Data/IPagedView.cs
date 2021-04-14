using System;

namespace Microsoft.Exchange.Data
{
	internal interface IPagedView
	{
		object[][] GetRows(int rowCount);
	}
}
