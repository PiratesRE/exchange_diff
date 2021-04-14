using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class FilterFeature : Feature
	{
		public FilterFeature(Func<FeatureSet, Result, bool> filterFunc)
		{
			this.filterFunc = filterFunc;
		}

		public Func<FeatureSet, Result, bool> FilterFunc
		{
			get
			{
				return this.filterFunc;
			}
		}

		private readonly Func<FeatureSet, Result, bool> filterFunc;
	}
}
