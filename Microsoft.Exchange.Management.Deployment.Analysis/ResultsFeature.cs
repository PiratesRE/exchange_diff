using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal sealed class ResultsFeature : Feature
	{
		public ResultsFeature(Func<Result, IEnumerable<Result>> resultFunc)
		{
			this.resultFunc = resultFunc;
		}

		public Func<Result, IEnumerable<Result>> ResultFunc
		{
			get
			{
				return this.resultFunc;
			}
		}

		private readonly Func<Result, IEnumerable<Result>> resultFunc;
	}
}
