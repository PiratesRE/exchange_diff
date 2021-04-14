using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class NoMatch : SpfTerm
	{
		public NoMatch(SenderIdValidationContext context) : base(context)
		{
		}

		public override void Process()
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "No matching mechanism, defaulting to result of Neutral");
			this.context.ValidationCompleted(SenderIdStatus.Neutral);
		}

		public override SpfTerm Append(SpfTerm newTerm)
		{
			throw new InvalidOperationException("Cannot append anything to the chain after a NoMatch term");
		}
	}
}
