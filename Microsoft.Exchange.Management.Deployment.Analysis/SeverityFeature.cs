using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class SeverityFeature : Feature
	{
		public SeverityFeature(Severity severity)
		{
			this.severity = severity;
		}

		public Severity Severity
		{
			get
			{
				return this.severity;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.Severity);
		}

		private readonly Severity severity;
	}
}
