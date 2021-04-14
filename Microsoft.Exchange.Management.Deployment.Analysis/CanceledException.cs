using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class CanceledException : AnalysisException
	{
		public CanceledException() : base(null, Strings.CanceledMessage)
		{
		}

		public CanceledException(string message) : base(null, message)
		{
		}
	}
}
