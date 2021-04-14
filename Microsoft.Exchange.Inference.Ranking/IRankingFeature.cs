using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Inference.Ranking
{
	public interface IRankingFeature
	{
		IList<PropertyDefinition> SupportingProperties { get; }

		double FeatureValue(object item);
	}
}
