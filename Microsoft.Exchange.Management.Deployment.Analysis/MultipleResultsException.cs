using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class MultipleResultsException : AnalysisException
	{
		public MultipleResultsException()
		{
		}

		public MultipleResultsException(AnalysisMember source) : base(source, Strings.AccessedValueWhenMultipleResults)
		{
		}
	}
}
