using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class FilteredException : AnalysisException
	{
		public FilteredException() : base(null, Strings.FilteredResult)
		{
		}

		public FilteredException(AnalysisMember source, Result filteredResult) : base(source, Strings.FilteredResult)
		{
			this.filteredResult = filteredResult;
		}

		public Result FilteredResult
		{
			get
			{
				return this.filteredResult;
			}
		}

		private readonly Result filteredResult;
	}
}
