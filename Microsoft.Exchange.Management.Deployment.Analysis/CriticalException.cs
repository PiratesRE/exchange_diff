using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class CriticalException : AnalysisException
	{
		public CriticalException() : base(null, Strings.CriticalMessage)
		{
		}

		public CriticalException(string message) : base(null, message)
		{
		}

		public CriticalException(AnalysisMember source, string message) : base(source, message)
		{
		}

		public CriticalException(AnalysisMember source, Exception inner) : base(source, Strings.CriticalMessage, inner)
		{
		}

		public CriticalException(AnalysisMember source, string message, Exception inner) : base(source, message, inner)
		{
		}
	}
}
