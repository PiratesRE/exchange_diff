using System;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.SenderId;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class ASpfMechanism : SpfMechanismWithDomainSpec
	{
		public ASpfMechanism(SenderIdValidationContext context, SenderIdStatus prefix, MacroTermSpfNode domainSpec, int ip4CidrLength, int ip6CidrLength) : base(context, prefix, domainSpec)
		{
			this.ip4CidrLength = ip4CidrLength;
			this.ip6CidrLength = ip6CidrLength;
		}

		protected override void ProcessWithExpandedDomainSpec(string expandedDomain)
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Process A mechanism");
			ExTraceGlobals.ValidationTracer.TraceDebug<string>((long)this.GetHashCode(), "Looking up A record for domain {0}", expandedDomain);
			if (!Util.AsyncDns.IsValidName(expandedDomain))
			{
				this.context.ValidationCompleted(SenderIdStatus.PermError);
				return;
			}
			Util.AsyncDns.BeginARecordQuery(expandedDomain, this.context.BaseContext.IPAddress.AddressFamily, new AsyncCallback(this.ACallback), null);
		}

		private void ACallback(IAsyncResult ar)
		{
			IPAddress[] array;
			DnsStatus status = Util.AsyncDns.EndARecordQuery(ar, out array);
			if (Util.AsyncDns.IsAcceptable(status))
			{
				IPNetwork ipnetwork = IPNetwork.Create(this.context.BaseContext.IPAddress, this.ip4CidrLength, this.ip6CidrLength);
				foreach (IPAddress address in array)
				{
					if (ipnetwork.Contains(address))
					{
						base.SetMatchResult();
						return;
					}
				}
				base.ProcessNextTerm();
				return;
			}
			this.context.ValidationCompleted(SenderIdStatus.TempError);
		}

		private int ip4CidrLength;

		private int ip6CidrLength;
	}
}
