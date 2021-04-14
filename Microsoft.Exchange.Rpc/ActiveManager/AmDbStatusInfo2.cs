using System;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	public class AmDbStatusInfo2
	{
		public AmDbStatusInfo2(AmDbStatusInfo2 info)
		{
			this.m_masterServerFqdn = info.m_masterServerFqdn;
			this.m_isHighlyAvailable = info.m_isHighlyAvailable;
			this.m_lastMountedServerFqdn = info.m_lastMountedServerFqdn;
			DateTime mountedTime = info.m_mountedTime;
			this.m_mountedTime = mountedTime;
		}

		public AmDbStatusInfo2(string masterServerFqdn, int isHighlyAvailable, string lastMountedServerFqdn, DateTime mountedTime)
		{
			this.m_masterServerFqdn = masterServerFqdn;
			this.m_isHighlyAvailable = isHighlyAvailable;
			this.m_lastMountedServerFqdn = lastMountedServerFqdn;
			this.m_mountedTime = mountedTime;
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

		public string LastMountedServerFqdn
		{
			get
			{
				return this.m_lastMountedServerFqdn;
			}
			set
			{
				this.m_lastMountedServerFqdn = value;
			}
		}

		public DateTime MountedTime
		{
			get
			{
				return this.m_mountedTime;
			}
		}

		private string m_masterServerFqdn;

		private int m_isHighlyAvailable;

		private string m_lastMountedServerFqdn;

		private DateTime m_mountedTime;
	}
}
