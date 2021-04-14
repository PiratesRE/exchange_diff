using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class AccessedFailedResultException : AnalysisException
	{
		public AccessedFailedResultException()
		{
		}

		public AccessedFailedResultException(AnalysisMember source, Exception inner) : base(source, Strings.AccessedFailedResult, inner)
		{
		}
	}
}
