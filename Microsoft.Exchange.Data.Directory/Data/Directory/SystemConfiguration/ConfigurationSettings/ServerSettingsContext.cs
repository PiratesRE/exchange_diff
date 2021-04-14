using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerSettingsContext : SettingsContextBase
	{
		static ServerSettingsContext()
		{
			SettingsContextBase.DefaultContextGetter = (() => ServerSettingsContext.LocalServer);
		}

		private ServerSettingsContext() : base(null)
		{
			Server localServer = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				localServer = Microsoft.Exchange.Data.Directory.SystemConfiguration.LocalServer.GetServer();
			});
			this.Initialize(localServer, Process.GetCurrentProcess().MainModule.ModuleName);
			this.DateCreated = DateTime.UtcNow;
		}

		public ServerSettingsContext(Server server, string processName, SettingsContextBase nextContext = null) : base(nextContext)
		{
			this.Initialize(server, processName);
			this.DateCreated = DateTime.UtcNow;
		}

		public ServerSettingsContext(string serverName, string processName, SettingsContextBase nextContext = null) : base(nextContext)
		{
			this.serverName = serverName;
			this.processName = processName;
			this.DateCreated = DateTime.UtcNow;
		}

		public ServerSettingsContext(string serverName, string serverRole, string processName, SettingsContextBase nextContext = null) : base(nextContext)
		{
			this.serverName = serverName;
			this.serverRole = serverRole;
			this.processName = processName;
			this.DateCreated = DateTime.UtcNow;
		}

		public static ServerSettingsContext LocalServer
		{
			get
			{
				ServerSettingsContext result;
				lock (ServerSettingsContext.lockObj)
				{
					if (ServerSettingsContext.localServer == null || ServerSettingsContext.localServer.CacheExpired)
					{
						ServerSettingsContext.localServer = new ServerSettingsContext();
					}
					result = ServerSettingsContext.localServer;
				}
				return result;
			}
		}

		public override Guid? ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		public override string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public override ServerVersion ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public override string ServerRole
		{
			get
			{
				return this.serverRole;
			}
		}

		public override string ProcessName
		{
			get
			{
				return this.processName;
			}
		}

		private bool CacheExpired
		{
			get
			{
				return this.DateCreated + TimeSpan.FromMinutes(15.0) < DateTime.UtcNow;
			}
		}

		private DateTime DateCreated { get; set; }

		private void Initialize(Server server, string processName)
		{
			if (server != null)
			{
				this.serverGuid = new Guid?(server.Guid);
				this.serverName = server.Name;
				this.serverVersion = server.AdminDisplayVersion;
				this.serverRole = ExchangeServer.ConvertE15ServerRoleToOutput(server.CurrentServerRole).ToString();
			}
			else
			{
				this.serverName = NativeHelpers.GetLocalComputerFqdn(false);
			}
			this.processName = processName;
		}

		private static ServerSettingsContext localServer;

		private static object lockObj = new object();

		private Guid? serverGuid;

		private string serverName;

		private ServerVersion serverVersion;

		private string serverRole;

		private string processName;
	}
}
