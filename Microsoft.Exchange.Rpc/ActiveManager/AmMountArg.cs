using System;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	public class AmMountArg
	{
		public AmMountArg(AmMountArg amMountArg)
		{
			this.m_lastMountedServer = amMountArg.m_lastMountedServer;
			this.m_storeFlags = amMountArg.m_storeFlags;
			this.m_amMountFlags = amMountArg.m_amMountFlags;
			this.m_reason = amMountArg.m_reason;
		}

		public AmMountArg(int storeFlags, int amFlags, string lastMountedServer, int reason)
		{
			this.m_lastMountedServer = lastMountedServer;
			this.m_storeFlags = storeFlags;
			this.m_amMountFlags = amFlags;
			this.m_reason = reason;
		}

		public string LastMountedServer
		{
			get
			{
				return this.m_lastMountedServer;
			}
			set
			{
				this.m_lastMountedServer = value;
			}
		}

		public int StoreFlags
		{
			get
			{
				return this.m_storeFlags;
			}
			set
			{
				this.m_storeFlags = value;
			}
		}

		public int AmMountFlags
		{
			get
			{
				return this.m_amMountFlags;
			}
			set
			{
				this.m_amMountFlags = value;
			}
		}

		public int Reason
		{
			get
			{
				return this.m_reason;
			}
			set
			{
				this.m_reason = value;
			}
		}

		private int m_storeFlags;

		private int m_amMountFlags;

		private int m_reason;

		private string m_lastMountedServer;
	}
}
