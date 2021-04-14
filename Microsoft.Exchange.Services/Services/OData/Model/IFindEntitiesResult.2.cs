using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.OData.Model
{
	public interface IFindEntitiesResult<out T> : IFindEntitiesResult, IEnumerable<T>, IEnumerable
	{
	}
}
