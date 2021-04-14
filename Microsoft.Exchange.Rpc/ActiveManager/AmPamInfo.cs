using System;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	public class AmPamInfo
	{
		public AmPamInfo(AmPamInfo pamInfo)
		{
			this.m_serverName = pamInfo.m_serverName;
		}

		public AmPamInfo(string pamServerName)
		{
			this.m_serverName = pamServerName;
		}

		public string ServerName
		{
			get
			{
				return this.m_serverName;
			}
			set
			{
				this.m_serverName = value;
			}
		}

		private string m_serverName;
	}
}
