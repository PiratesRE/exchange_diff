using System;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class EmptyResultsException : AnalysisException
	{
		public EmptyResultsException()
		{
		}

		public EmptyResultsException(AnalysisMember source) : base(source, Strings.EmptyResults)
		{
		}
	}
}
