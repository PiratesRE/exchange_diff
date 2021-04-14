using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class UnknownSpfMechanism : SpfMechanism
	{
		public UnknownSpfMechanism(SenderIdValidationContext context) : base(context, SenderIdStatus.None)
		{
		}

		public override void Process()
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Processing unknown mechanism - returning PermError");
			this.context.ValidationCompleted(SenderIdStatus.PermError);
		}
	}
}
