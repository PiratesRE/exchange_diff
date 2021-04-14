using System;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal interface IFeatureBuilder
	{
		void AddFeature(Feature feature);
	}
}
