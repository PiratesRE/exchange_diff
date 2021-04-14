using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OrQuery : CompositeQuery
	{
		internal OrQuery(IList<Query> queries) : base(queries)
		{
		}

		public override bool IsMatch(object[] row)
		{
			for (int i = 0; i < this.Queries.Count; i++)
			{
				if (this.Queries[i].IsMatch(row))
				{
					return true;
				}
			}
			return false;
		}
	}
}
