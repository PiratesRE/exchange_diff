using System;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal abstract class BaseSingleFilterConverter
	{
		internal abstract bool IsLeafFilter { get; }
	}
}
