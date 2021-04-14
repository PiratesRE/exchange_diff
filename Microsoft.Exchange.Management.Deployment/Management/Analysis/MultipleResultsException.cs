using System;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class MultipleResultsException : AnalysisException
	{
		public MultipleResultsException()
		{
		}

		public MultipleResultsException(AnalysisMember source) : base(source, Strings.AccessedValueWhenMultipleResults)
		{
		}
	}
}
