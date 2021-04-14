using System;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class AccessedFailedResultException : AnalysisException
	{
		public AccessedFailedResultException()
		{
		}

		public AccessedFailedResultException(AnalysisMember source, Exception inner) : base(source, Strings.AccessedFailedResult, inner)
		{
		}
	}
}
