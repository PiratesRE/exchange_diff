using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics.Components.SenderId;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class ExistsSpfMechanism : SpfMechanismWithDomainSpec
	{
		public ExistsSpfMechanism(SenderIdValidationContext context, SenderIdStatus prefix, MacroTermSpfNode domainSpec) : base(context, prefix, domainSpec)
		{
		}

		protected override void ProcessWithExpandedDomainSpec(string expandedDomain)
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Processing Exists mechansim");
			ExTraceGlobals.ValidationTracer.TraceDebug<string>((long)this.GetHashCode(), "Looking up A record for domain {0}", expandedDomain);
			if (!Util.AsyncDns.IsValidName(expandedDomain))
			{
				this.context.ValidationCompleted(SenderIdStatus.PermError);
				return;
			}
			Util.AsyncDns.BeginARecordQuery(expandedDomain, AddressFamily.InterNetwork, new AsyncCallback(this.ACallback), null);
		}

		private void ACallback(IAsyncResult ar)
		{
			IPAddress[] array;
			DnsStatus status = Util.AsyncDns.EndARecordQuery(ar, out array);
			if (!Util.AsyncDns.IsAcceptable(status))
			{
				this.context.ValidationCompleted(SenderIdStatus.TempError);
				return;
			}
			if (array.Length > 0)
			{
				base.SetMatchResult();
				return;
			}
			base.ProcessNextTerm();
		}
	}
}
