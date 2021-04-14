using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class EmptyResultsException : AnalysisException
	{
		public EmptyResultsException()
		{
		}

		public EmptyResultsException(AnalysisMember source) : base(source, Strings.EmptyResults)
		{
		}
	}
}
