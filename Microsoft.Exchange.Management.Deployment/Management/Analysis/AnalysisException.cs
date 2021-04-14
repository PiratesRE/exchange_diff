using System;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class AnalysisException : Exception
	{
		public AnalysisException()
		{
		}

		public AnalysisException(AnalysisMember source)
		{
			this.AnalysisMemberSource = source;
		}

		public AnalysisException(AnalysisMember source, string message) : base(message)
		{
			this.AnalysisMemberSource = source;
		}

		public AnalysisException(AnalysisMember source, string message, Exception inner) : base(message, inner)
		{
			this.AnalysisMemberSource = source;
		}

		public AnalysisMember AnalysisMemberSource { get; set; }
	}
}
