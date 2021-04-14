using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Inference.Ranking
{
	public interface IRankingModel
	{
		HashSet<PropertyDefinition> Dependencies { get; }

		double Rank(object item);
	}
}
