using System;
using System.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class TargetHostTraceWrapper : ITraceWrapper<TargetHost>
	{
		public TargetHostTraceWrapper()
		{
		}

		public TargetHostTraceWrapper(TargetHost host)
		{
			this.host = host;
		}

		public TargetHost Element
		{
			set
			{
				this.host = value;
			}
		}

		public override string ToString()
		{
			if (this.host == null)
			{
				return "<null>";
			}
			return string.Format("Target host {{ name='{0}' addresses={{ {1} }} TTL={2} }}", this.host.Name ?? "<null>", new ListTracer<IPAddress>(this.host.IPAddresses), this.host.TimeToLive);
		}

		private TargetHost host;
	}
}
