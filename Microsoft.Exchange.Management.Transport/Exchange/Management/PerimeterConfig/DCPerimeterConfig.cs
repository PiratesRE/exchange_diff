using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.PerimeterConfig
{
	[Serializable]
	public class DCPerimeterConfig
	{
		public DCPerimeterConfig(IPAddress[] inboundIPAddresses, IPAddress[] outboundIPAddresses)
		{
			if (inboundIPAddresses != null)
			{
				this.inboundIPAddresses = new MultiValuedProperty<IPAddress>(inboundIPAddresses);
			}
			else
			{
				this.inboundIPAddresses = MultiValuedProperty<IPAddress>.Empty;
			}
			if (outboundIPAddresses != null)
			{
				this.outboundIPAddresses = new MultiValuedProperty<IPAddress>(outboundIPAddresses);
				return;
			}
			this.outboundIPAddresses = MultiValuedProperty<IPAddress>.Empty;
		}

		[Parameter]
		public MultiValuedProperty<IPAddress> InboundIPAddresses
		{
			get
			{
				return this.inboundIPAddresses;
			}
			set
			{
				this.inboundIPAddresses = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPAddress> OutboundIPAddresses
		{
			get
			{
				return this.outboundIPAddresses;
			}
			set
			{
				this.outboundIPAddresses = value;
			}
		}

		private MultiValuedProperty<IPAddress> inboundIPAddresses;

		private MultiValuedProperty<IPAddress> outboundIPAddresses;
	}
}
