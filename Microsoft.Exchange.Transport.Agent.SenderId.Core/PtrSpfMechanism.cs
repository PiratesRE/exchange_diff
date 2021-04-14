using System;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.SenderId;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class PtrSpfMechanism : SpfMechanismWithDomainSpec
	{
		public PtrSpfMechanism(SenderIdValidationContext context, SenderIdStatus prefix, MacroTermSpfNode domainSpec) : base(context, prefix, domainSpec)
		{
		}

		protected override void ProcessWithExpandedDomainSpec(string expandedDomain)
		{
			ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Processing PTR mechanism");
			ExTraceGlobals.ValidationTracer.TraceDebug<string>((long)this.GetHashCode(), "Looking up PTR record for domain: {0}", expandedDomain);
			this.domain = expandedDomain;
			Util.AsyncDns.BeginPtrRecordQuery(this.context.BaseContext.IPAddress, new AsyncCallback(this.PtrCallback), null);
		}

		private void PtrCallback(IAsyncResult ar)
		{
			string[] array;
			DnsStatus status = Util.AsyncDns.EndPtrRecordQuery(ar, out array);
			if (Util.AsyncDns.IsAcceptable(status))
			{
				this.validatedHosts = array;
				this.validatedHostIndex = 0;
				this.LookupRemainingValidatedHostIPs();
				return;
			}
			base.ProcessNextTerm();
		}

		private void LookupRemainingValidatedHostIPs()
		{
			bool flag = false;
			while (!flag)
			{
				if (this.validatedHostIndex >= 10)
				{
					this.context.ValidationCompleted(SenderIdStatus.PermError);
					return;
				}
				if (this.validatedHostIndex >= this.validatedHosts.Length)
				{
					base.ProcessNextTerm();
					return;
				}
				if (Util.AsyncDns.IsValidName(this.validatedHosts[this.validatedHostIndex]))
				{
					flag = true;
					break;
				}
				this.validatedHostIndex++;
			}
			if (flag)
			{
				Util.AsyncDns.BeginARecordQuery(this.validatedHosts[this.validatedHostIndex], this.context.BaseContext.IPAddress.AddressFamily, new AsyncCallback(this.ACallback), null);
			}
		}

		private void ACallback(IAsyncResult ar)
		{
			IPAddress[] array;
			DnsStatus status = Util.AsyncDns.EndARecordQuery(ar, out array);
			if (Util.AsyncDns.IsAcceptable(status))
			{
				IPAddress[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					IPAddress ipaddress = array2[i];
					if (ipaddress.Equals(this.context.BaseContext.IPAddress))
					{
						if (this.validatedHosts[this.validatedHostIndex].EndsWith(this.domain, StringComparison.OrdinalIgnoreCase))
						{
							base.SetMatchResult();
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			this.validatedHostIndex++;
			this.LookupRemainingValidatedHostIPs();
		}

		public const int MaxPtrNames = 10;

		private string[] validatedHosts = new string[0];

		private int validatedHostIndex;

		private string domain = string.Empty;
	}
}
