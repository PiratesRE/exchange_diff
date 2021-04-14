using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class RootAnalysisMember : AnalysisMember
	{
		public RootAnalysisMember(Analysis analysis) : base(() => null, ConcurrencyType.Synchronous, analysis, Enumerable.Empty<Feature>())
		{
		}

		public override Type ValueType
		{
			get
			{
				return typeof(object);
			}
		}

		public override void Start()
		{
		}

		public override IEnumerable<Result> GetResults()
		{
			return new Result<object>[]
			{
				Result<object>.Default
			};
		}
	}
}
