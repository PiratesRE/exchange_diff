using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal abstract class BuildContext<T> : IFeatureBuilder
	{
		public BuildContext()
		{
			this.features = new List<Feature>();
			this.Parent = null;
			this.RunAs = ConcurrencyType.Synchronous;
			this.Analysis = null;
			this.features = new List<Feature>();
		}

		public Func<AnalysisMember> Parent { get; set; }

		public ConcurrencyType RunAs { get; set; }

		public Analysis Analysis { get; set; }

		public IEnumerable<Feature> Features
		{
			get
			{
				return this.features;
			}
		}

		public abstract AnalysisMember<T> Construct();

		void IFeatureBuilder.AddFeature(Feature feature)
		{
			if (feature == null)
			{
				throw new ArgumentNullException("feature");
			}
			if (!feature.AllowsMultiple)
			{
				this.features.RemoveAll((Feature x) => x.GetType() == this.features.GetType());
			}
			this.features.Add(feature);
		}

		private List<Feature> features;
	}
}
