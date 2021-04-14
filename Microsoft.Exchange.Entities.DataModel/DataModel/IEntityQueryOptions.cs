using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Entities.DataModel
{
	public interface IEntityQueryOptions
	{
		int? Skip { get; }

		IReadOnlyList<OrderByClause> OrderBy { get; }

		int? Take { get; }

		Expression Filter { get; }
	}
}
