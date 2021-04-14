using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public sealed class RuleResult : Result<bool>
	{
		public RuleResult(bool value) : base(value)
		{
		}

		public RuleResult(Exception exception) : base(exception)
		{
		}

		internal RuleResult(RuleResult toCopy, AnalysisMember source, Result parent, ExDateTime startTime, ExDateTime stopTime) : base(toCopy, source, parent, startTime, stopTime)
		{
			this.Severity = toCopy.Severity;
		}

		public Severity? Severity { get; set; }
	}
}
