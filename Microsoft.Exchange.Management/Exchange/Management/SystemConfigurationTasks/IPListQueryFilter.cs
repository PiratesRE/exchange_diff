using System;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class IPListQueryFilter : QueryFilter
	{
		public IPListQueryFilter(IPAddress address)
		{
			this.address = ((address != null) ? new IPvxAddress(address) : IPvxAddress.None);
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.address.ToString());
			sb.Append(")");
		}

		public IPvxAddress Address
		{
			get
			{
				return this.address;
			}
		}

		private IPvxAddress address;
	}
}
