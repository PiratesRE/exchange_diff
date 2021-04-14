using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Inference.Ranking
{
	internal delegate double FeatureValueCalculator(IStorePropertyBag conversation);
}
