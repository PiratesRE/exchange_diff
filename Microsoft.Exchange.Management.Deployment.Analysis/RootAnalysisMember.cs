using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class RootAnalysisMember : AnalysisMember<object>
	{
		public RootAnalysisMember(Analysis analysis) : base(analysis, RootAnalysisMember.RootFeatureSetBuilder.RootFeatureSet)
		{
		}

		public override Type ValueType
		{
			get
			{
				return typeof(object);
			}
		}

		public override IEnumerable<Result> UntypedResults
		{
			get
			{
				return RootAnalysisMember.rootResults;
			}
		}

		public override IEnumerable<Result> CachedResults
		{
			get
			{
				return RootAnalysisMember.rootResults;
			}
		}

		public override void Start()
		{
		}

		private static readonly Result<object> rootResult = new Result<object>(new object());

		private static readonly IEnumerable<Result> rootResults = new Result[]
		{
			RootAnalysisMember.rootResult
		};

		private sealed class RootFeatureSetBuilder : FeatureSet.Builder
		{
			public static FeatureSet RootFeatureSet
			{
				get
				{
					return RootAnalysisMember.RootFeatureSetBuilder.rootFeatureSet;
				}
			}

			private FeatureSet Build()
			{
				IEnumerable<Feature> features = Enumerable.Empty<Feature>();
				Feature[] array = new Feature[3];
				array[0] = new EvaluationModeFeature(Evaluate.OnDemand);
				array[1] = new ForEachResultFeature(() => null);
				array[2] = new ResultsFeature((Result x) => RootAnalysisMember.rootResults);
				return base.BuildFeatureSet(features, array);
			}

			private static readonly FeatureSet rootFeatureSet = new RootAnalysisMember.RootFeatureSetBuilder().Build();
		}
	}
}
