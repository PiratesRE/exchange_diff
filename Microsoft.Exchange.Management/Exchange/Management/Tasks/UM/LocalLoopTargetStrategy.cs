using System;
using System.ServiceProcess;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal abstract class LocalLoopTargetStrategy
	{
		public abstract string ServiceName { get; }

		public abstract string CertificateThumbprint { get; }

		public abstract ServerRole ServerRole { get; }

		public abstract UMStartupMode StartupMode { get; }

		public abstract string TargetFqdn { get; }

		public Server Server
		{
			get
			{
				this.EnsureServerInitialized();
				return this.server;
			}
		}

		public string LocalFqdn { get; private set; }

		private protected IConfigurationSession ConfigurationSession { protected get; private set; }

		protected LocalLoopTargetStrategy(IConfigurationSession configSession)
		{
			this.LocalFqdn = Utils.GetOwnerHostFqdn();
			this.ConfigurationSession = configSession;
		}

		public static LocalLoopTargetStrategy Create(IConfigurationSession configSession, bool callRouterSwitchSet)
		{
			if (callRouterSwitchSet)
			{
				return new LocalLoopTargetStrategy.CallRouterStrategy(configSession);
			}
			return new LocalLoopTargetStrategy.BackendStrategy(configSession);
		}

		public abstract int GetPort(bool isSecured);

		public abstract TestUMConnectivityHelper.UMConnectivityResults CreateTestResult();

		public void CheckServiceRunning()
		{
			this.EnsureServerInitialized();
			using (ServiceController serviceController = new ServiceController(this.ServiceName))
			{
				if (serviceController.Status != ServiceControllerStatus.Running)
				{
					throw new ServiceNotStarted(this.ServiceName);
				}
			}
		}

		private void EnsureServerInitialized()
		{
			if (this.server == null)
			{
				this.server = LocalServerCache.LocalServer;
				if (this.server == null || (this.server.CurrentServerRole & this.ServerRole) != this.ServerRole)
				{
					throw new LocalServerNotFoundException(this.LocalFqdn);
				}
			}
		}

		private Server server;

		private class CallRouterStrategy : LocalLoopTargetStrategy
		{
			private SIPFEServerConfiguration Configuration
			{
				get
				{
					if (this.configuration == null)
					{
						this.configuration = SIPFEServerConfiguration.Find(base.Server, base.ConfigurationSession);
						if (this.configuration == null)
						{
							throw new SIPFEServerConfigurationNotFoundException(base.LocalFqdn);
						}
					}
					return this.configuration;
				}
			}

			public override ServerRole ServerRole
			{
				get
				{
					return ServerRole.Cafe;
				}
			}

			public override UMStartupMode StartupMode
			{
				get
				{
					return this.Configuration.UMStartupMode;
				}
			}

			public override string ServiceName
			{
				get
				{
					return "MSExchangeUMCR";
				}
			}

			public override string CertificateThumbprint
			{
				get
				{
					return this.Configuration.UMCertificateThumbprint;
				}
			}

			public override string TargetFqdn
			{
				get
				{
					if (this.Configuration.ExternalHostFqdn == null)
					{
						return base.LocalFqdn;
					}
					return this.Configuration.ExternalHostFqdn.ToString();
				}
			}

			public CallRouterStrategy(IConfigurationSession configSession) : base(configSession)
			{
			}

			public override int GetPort(bool isSecured)
			{
				if (!isSecured)
				{
					return this.Configuration.SipTcpListeningPort;
				}
				return this.Configuration.SipTlsListeningPort;
			}

			public override TestUMConnectivityHelper.UMConnectivityResults CreateTestResult()
			{
				return new TestUMConnectivityHelper.LocalUMConnectivityOptionsResults();
			}

			private SIPFEServerConfiguration configuration;
		}

		private class BackendStrategy : LocalLoopTargetStrategy
		{
			private UMServer Configuration
			{
				get
				{
					if (this.configuration == null)
					{
						this.configuration = new UMServer(base.Server);
					}
					return this.configuration;
				}
			}

			public override ServerRole ServerRole
			{
				get
				{
					return ServerRole.UnifiedMessaging;
				}
			}

			public override UMStartupMode StartupMode
			{
				get
				{
					return this.Configuration.UMStartupMode;
				}
			}

			public override string ServiceName
			{
				get
				{
					return "MSExchangeUM";
				}
			}

			public override string CertificateThumbprint
			{
				get
				{
					return this.Configuration.UMCertificateThumbprint;
				}
			}

			public override string TargetFqdn
			{
				get
				{
					if (this.Configuration.ExternalHostFqdn == null)
					{
						return base.LocalFqdn;
					}
					return this.Configuration.ExternalHostFqdn.ToString();
				}
			}

			public BackendStrategy(IConfigurationSession configSession) : base(configSession)
			{
			}

			public override int GetPort(bool isSecured)
			{
				if (!isSecured)
				{
					return this.Configuration.SipTcpListeningPort;
				}
				return this.Configuration.SipTlsListeningPort;
			}

			public override TestUMConnectivityHelper.UMConnectivityResults CreateTestResult()
			{
				return new TestUMConnectivityHelper.LocalUMConnectivityResults();
			}

			private UMServer configuration;
		}
	}
}
