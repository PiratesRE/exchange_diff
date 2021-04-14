using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class DagNetworkQueryFilter : QueryFilter
	{
		public DagNetworkQueryFilter(DagNetworkObjectId networkNames)
		{
			this.m_networkNames = networkNames;
		}

		public DagNetworkObjectId NamesToMatch
		{
			get
			{
				return this.m_networkNames;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(DagNetworkQueryFilter)");
		}

		private DagNetworkObjectId m_networkNames;
	}
}
