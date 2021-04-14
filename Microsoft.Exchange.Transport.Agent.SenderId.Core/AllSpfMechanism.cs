using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class AllSpfMechanism : SpfMechanism
	{
		public AllSpfMechanism(SenderIdValidationContext context, SenderIdStatus prefix) : base(context, prefix)
		{
		}

		public override void Process()
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Processing All mechanism");
			base.SetMatchResult();
		}
	}
}
