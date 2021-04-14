using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface INonLeafSearchExpressionType
	{
		SearchExpressionType[] Items { get; set; }
	}
}
