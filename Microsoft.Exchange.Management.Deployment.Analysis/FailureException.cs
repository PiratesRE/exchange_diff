using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class FailureException : AnalysisException
	{
		public FailureException() : base(null, Strings.FailedResult)
		{
		}

		public FailureException(string message) : base(null, message)
		{
		}

		public FailureException(AnalysisMember source, string message) : base(source, message)
		{
		}
	}
}
