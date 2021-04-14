using System;
using System.Collections;

namespace Microsoft.Exchange.Services.OData.Model
{
	public interface IFindEntitiesResult : IEnumerable
	{
		int TotalCount { get; }
	}
}
