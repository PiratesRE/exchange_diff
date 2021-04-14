using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ConnectToServerParams
	{
		public ConnectToServerParams(bool defaultServer, string name)
		{
			this.setAsDefaultServer = defaultServer;
			this.serverName = name;
		}

		public bool SetAsDefaultServer
		{
			get
			{
				return this.setAsDefaultServer;
			}
			set
			{
				this.setAsDefaultServer = value;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				this.serverName = value;
			}
		}

		private bool setAsDefaultServer;

		private string serverName;
	}
}
