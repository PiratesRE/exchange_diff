using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Providers;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class AlternateMailboxQueryFilter : QueryFilter
	{
		public AlternateMailboxQueryFilter(AlternateMailboxObjectId amNames)
		{
			this.m_amNames = amNames;
		}

		public AlternateMailboxObjectId NamesToMatch
		{
			get
			{
				return this.m_amNames;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(AlternateMailboxQueryFilter)");
		}

		private AlternateMailboxObjectId m_amNames;
	}
}
