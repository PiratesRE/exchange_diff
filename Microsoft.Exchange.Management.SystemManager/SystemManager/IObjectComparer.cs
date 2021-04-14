using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public interface IObjectComparer : ISupportTextComparer
	{
		ITextComparer TextComparer { get; }

		SortMode GetSortMode(Type type);
	}
}
