using System;
using Microsoft.Exchange.Diagnostics.Components.SenderId;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class IPSpfMechanism : SpfMechanism
	{
		public IPSpfMechanism(SenderIdValidationContext context, SenderIdStatus prefix, IPNetwork network) : base(context, prefix)
		{
			this.network = network;
		}

		public override void Process()
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Processing IP4/IP6 mechanism");
			if (this.network.Contains(this.context.BaseContext.IPAddress))
			{
				base.SetMatchResult();
				return;
			}
			base.ProcessNextTerm();
		}

		private IPNetwork network;
	}
}
