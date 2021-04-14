using System;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class FailureException : AnalysisException
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
