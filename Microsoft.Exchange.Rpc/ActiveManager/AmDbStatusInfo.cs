using System;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	public class AmDbStatusInfo
	{
		public AmDbStatusInfo(AmDbStatusInfo info)
		{
			this.m_masterServerFqdn = info.m_masterServerFqdn;
			this.m_isHighlyAvailable = info.m_isHighlyAvailable;
		}

		public AmDbStatusInfo(string masterServerFqdn, int isHighlyAvailable)
		{
			this.m_masterServerFqdn = masterServerFqdn;
			this.m_isHighlyAvailable = isHighlyAvailable;
		}

		public string MasterServerFqdn
		{
			get
			{
				return this.m_masterServerFqdn;
			}
			set
			{
				this.m_masterServerFqdn = value;
			}
		}

		public int IsHighlyAvailable
		{
			get
			{
				return this.m_isHighlyAvailable;
			}
		}

		private string m_masterServerFqdn;

		private int m_isHighlyAvailable;
	}
}
