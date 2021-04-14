using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CompositeQuery : Query
	{
		protected CompositeQuery(IList<Query> queries)
		{
			this.Queries = queries;
		}

		protected readonly IList<Query> Queries;
	}
}
