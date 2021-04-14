using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	[Serializable]
	public sealed class HybridMailflowDatacenterIPs : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return HybridMailflowDatacenterIPs.schema;
			}
		}

		public List<IPRange> DatacenterIPs
		{
			get
			{
				return this.myDatacenterIPs;
			}
			set
			{
				this.myDatacenterIPs = value;
			}
		}

		internal HybridMailflowDatacenterIPs() : base(new SimpleProviderPropertyBag())
		{
		}

		internal HybridMailflowDatacenterIPs(IList<IPRange> datacenterIPs) : base(new SimpleProviderPropertyBag())
		{
			this.DatacenterIPs = new List<IPRange>(datacenterIPs);
		}

		private const string MostDerivedClass = "msHybridMailflowDatacenterIPs";

		private static HybridMailflowDatacenterIPsSchema schema = ObjectSchema.GetInstance<HybridMailflowDatacenterIPsSchema>();

		private List<IPRange> myDatacenterIPs;
	}
}
