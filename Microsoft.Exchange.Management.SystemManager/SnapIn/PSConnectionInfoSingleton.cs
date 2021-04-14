using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class PSConnectionInfoSingleton
	{
		static PSConnectionInfoSingleton()
		{
			AppDomain.CurrentDomain.DomainUnload += PSConnectionInfoSingleton.AppDomainUnloadEventHandler;
		}

		private static void AppDomainUnloadEventHandler(object sender, EventArgs args)
		{
			PSConnectionInfoSingleton.connection.Dispose();
		}

		public static void DisposeCurrentInstance()
		{
			if (PSConnectionInfoSingleton.instance != null)
			{
				PSConnectionInfoSingleton.instance = null;
			}
		}

		protected PSConnectionInfoSingleton()
		{
			this.Type = OrganizationType.ToolOrEdge;
			this.LogonWithDefaultCredential = true;
		}

		public static PSConnectionInfoSingleton GetInstance()
		{
			if (PSConnectionInfoSingleton.instance == null)
			{
				PSConnectionInfoSingleton.instance = new PSConnectionInfoSingleton();
			}
			return PSConnectionInfoSingleton.instance;
		}

		public IReportProgress ReportProgress { get; set; }

		public bool Enabled { get; set; }

		public string DisplayName { get; set; }

		public Uri Uri { get; set; }

		public bool LogonWithDefaultCredential { get; set; }

		public string CredentialKey { get; set; }

		public OrganizationType Type { get; set; }

		public PSCredential ProposedCredential { get; set; }

		public string UserAccount
		{
			get
			{
				if (this.LogonWithDefaultCredential)
				{
					using (WindowsIdentity current = WindowsIdentity.GetCurrent())
					{
						return current.Name;
					}
				}
				if (this.DefaultConnectionInfo != null)
				{
					return this.DefaultConnectionInfo.Credentials.UserName;
				}
				if (this.ProposedCredential != null)
				{
					return this.ProposedCredential.UserName;
				}
				PSCredential pscredential = CredentialHelper.ReadCredential(this.CredentialKey);
				if (pscredential != null)
				{
					return pscredential.UserName;
				}
				return null;
			}
		}

		public string ServerName
		{
			get
			{
				if (this.Uri != null)
				{
					return this.Uri.Host;
				}
				return null;
			}
		}

		public Fqdn EcpServer
		{
			get
			{
				if (this.Type == OrganizationType.Cloud && this.Uri == new Uri(PSConnectionInfoSingleton.ExchangeOnlineUri))
				{
					return new Fqdn(PSConnectionInfoSingleton.ExchangeOnlineEcpServer);
				}
				return this.RemotePSServer;
			}
		}

		public Fqdn RemotePSServer
		{
			get
			{
				return this.remotePSServer;
			}
			private set
			{
				if (this.remotePSServer != value)
				{
					this.remotePSServer = value;
					this.OnRemotePSServerChanged();
				}
			}
		}

		private void OnRemotePSServerChanged()
		{
			EventHandler eventHandler = (EventHandler)this.eventHandlers[this.remotePSServerChangedEvent];
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}

		public event EventHandler RemotePSServerChanged
		{
			add
			{
				this.eventHandlers.AddHandler(this.remotePSServerChangedEvent, value);
			}
			remove
			{
				this.eventHandlers.RemoveHandler(this.remotePSServerChangedEvent, value);
			}
		}

		private MonadConnectionInfo DefaultConnectionInfo
		{
			get
			{
				return this.defaultConnectionInfo;
			}
			set
			{
				if (this.DefaultConnectionInfo != value)
				{
					this.defaultConnectionInfo = value;
					this.RemotePSServer = ((this.DefaultConnectionInfo == null) ? null : new Fqdn(this.DefaultConnectionInfo.ServerUri.Host));
				}
			}
		}

		public SynchronizationContext UISyncContext { get; set; }

		public string SlotVersion { get; set; }

		public void UpdateRemotePSServer(Fqdn server)
		{
			if (this.Type != OrganizationType.LocalOnPremise && this.Type != OrganizationType.ToolOrEdge)
			{
				throw new NotSupportedException();
			}
			Uri uri = this.Uri;
			Uri remotePowerShellUri = PSConnectionInfoSingleton.GetRemotePowerShellUri(server);
			bool flag = true;
			try
			{
				this.Uri = remotePowerShellUri;
				this.DefaultConnectionInfo = this.DiscoverOrganizationConnectionInfo();
				flag = false;
			}
			finally
			{
				if (flag)
				{
					this.Uri = uri;
				}
			}
		}

		public MonadConnectionInfo GetMonadConnectionInfo(ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel)
		{
			MonadConnectionInfo monadConnectionInfo = this.GenerateMonadConnectionInfo();
			if (serializationLevel != ExchangeRunspaceConfigurationSettings.SerializationLevel.Full && monadConnectionInfo != null)
			{
				monadConnectionInfo = new MonadConnectionInfo(PSConnectionInfoSingleton.ExtractValidUri(monadConnectionInfo), monadConnectionInfo.Credentials, monadConnectionInfo.ShellUri, monadConnectionInfo.FileTypesXml, monadConnectionInfo.AuthenticationMechanism, serializationLevel, monadConnectionInfo.ClientApplication, monadConnectionInfo.ClientVersion, monadConnectionInfo.MaximumConnectionRedirectionCount, monadConnectionInfo.SkipServerCertificateCheck);
			}
			return monadConnectionInfo;
		}

		private static Uri ExtractValidUri(MonadConnectionInfo connectionInfo)
		{
			if (connectionInfo.ServerUri.ToString().StartsWith(PSConnectionInfoSingleton.GetExchangeOnlineUri().ToString()))
			{
				return PSConnectionInfoSingleton.GetExchangeOnlineUri();
			}
			return new Uri(connectionInfo.ServerUri.GetLeftPart(UriPartial.Path));
		}

		public MonadConnectionInfo GetMonadConnectionInfo()
		{
			return this.GetMonadConnectionInfo(ExchangeRunspaceConfigurationSettings.SerializationLevel.Full);
		}

		private MonadConnectionInfo GenerateMonadConnectionInfo()
		{
			if (this.DefaultConnectionInfo != null)
			{
				return this.DefaultConnectionInfo;
			}
			MonadConnectionInfo result;
			lock (this.syncObject)
			{
				if (this.DefaultConnectionInfo == null)
				{
					this.DefaultConnectionInfo = this.DiscoverOrganizationConnectionInfo();
				}
				result = this.DefaultConnectionInfo;
			}
			return result;
		}

		private void ReportConnectToSpecifiedServerProgress(int percent)
		{
			if (this.ReportProgress != null)
			{
				string host = this.Uri.Host;
				this.ReportProgress.ReportProgress(percent, 100, Strings.ProgressReportConnectToSpecifiedServer(host), Strings.ProgressReportConnectToSpecifiedServerErrorText(host));
			}
		}

		private void ReportDiscoverServerProgress(int percent)
		{
			if (this.ReportProgress != null)
			{
				this.ReportProgress.ReportProgress(percent, 100, Strings.ProgressReportDiscoverExchangeServer, Strings.ProgressReportDiscoverExchangeServerErrorText);
			}
		}

		private void ReportConnectToServerProgress(int percent)
		{
			if (this.ReportProgress != null)
			{
				string host = this.Uri.Host;
				this.ReportProgress.ReportProgress(percent, 100, Strings.ProgressReportConnectToServer(host), Strings.ProgressReportConnectToServerErrorText(host));
			}
		}

		private MonadConnectionInfo DiscoverOrganizationConnectionInfo()
		{
			if (!WinformsHelper.IsRemoteEnabled())
			{
				return null;
			}
			if (this.Type == OrganizationType.RemoteOnPremise || this.Type == OrganizationType.Cloud)
			{
				this.ReportConnectToSpecifiedServerProgress(0);
				return this.DiscoverConnectionInfo();
			}
			if (this.Uri != null)
			{
				try
				{
					this.ReportConnectToSpecifiedServerProgress(0);
					return this.DiscoverConnectionInfo();
				}
				catch (PSRemotingDataStructureException)
				{
				}
				catch (PSRemotingTransportException)
				{
				}
			}
			this.ReportDiscoverServerProgress(15);
			this.Uri = PSConnectionInfoSingleton.GetRemotePowerShellUri(PSConnectionInfoSingleton.DiscoverExchangeServer());
			this.ReportConnectToServerProgress(40);
			return this.DiscoverConnectionInfo();
		}

		private MonadConnectionInfo DiscoverConnectionInfo()
		{
			SupportedVersionList supportedVersionList = null;
			MonadConnectionInfo result = new ConnectionRetryDiscoverer(this).DiscoverConnectionInfo(out supportedVersionList, (OrganizationType.LocalOnPremise == this.Type || OrganizationType.Cloud == this.Type || OrganizationType.RemoteOnPremise == this.Type) ? this.SlotVersion : string.Empty);
			if (this.Type == OrganizationType.Cloud)
			{
				if (supportedVersionList.Count == 0)
				{
					throw new SupportedVersionListFormatException(Strings.AtLeastOneVersionMustBeSupported);
				}
				if (supportedVersionList.IsSupported(this.SlotVersion))
				{
					throw new VersionMismatchException(Strings.MicrosoftExchangeOnPremise, supportedVersionList);
				}
			}
			else if (this.Type == OrganizationType.RemoteOnPremise)
			{
				if (supportedVersionList == null || supportedVersionList.Count == 0)
				{
					throw new InvalidOperationException(Strings.SP1ConnectToRTMServerError);
				}
				if (!supportedVersionList.IsSupported(this.SlotVersion))
				{
					throw new InvalidOperationException(Strings.IncampatibleVersionConnectionError(supportedVersionList.GetLatestVersion(), this.SlotVersion));
				}
			}
			return result;
		}

		private Fqdn GetAutoDiscoveredServer()
		{
			if (string.IsNullOrEmpty(this.autoDiscoveredServer))
			{
				this.autoDiscoveredServer = PSConnectionInfoSingleton.DiscoverExchangeServer();
			}
			return this.autoDiscoveredServer;
		}

		public MonadConnectionInfo GetMonadConnectionInfo(IUIService uiService, OrganizationSetting forestInfo)
		{
			lock (this.forestConnectionInfos)
			{
				if (this.forestConnectionInfos.ContainsKey(forestInfo.Key))
				{
					return this.forestConnectionInfos[forestInfo.Key];
				}
			}
			MonadConnectionInfo monadConnectionInfo = this.DiscoverForestConnectionInfo(uiService, forestInfo);
			MonadConnectionInfo result;
			lock (this.forestConnectionInfos)
			{
				this.forestConnectionInfos[forestInfo.Key] = monadConnectionInfo;
				result = monadConnectionInfo;
			}
			return result;
		}

		private MonadConnectionInfo DiscoverForestConnectionInfo(IUIService uiService, OrganizationSetting forestInfo)
		{
			SupportedVersionList supportedVersionList = null;
			switch (forestInfo.Type)
			{
			case OrganizationType.LocalOnPremise:
				return new ConnectionRetryDiscoverer(uiService, OrganizationType.LocalOnPremise, Strings.MicrosoftExchangeOnPremise, PSConnectionInfoSingleton.GetRemotePowerShellUri(this.GetAutoDiscoveredServer()), true).DiscoverConnectionInfo(out supportedVersionList, string.Empty);
			case OrganizationType.RemoteOnPremise:
			case OrganizationType.Cloud:
				return new ConnectionRetryDiscoverer(forestInfo, uiService).DiscoverConnectionInfo(out supportedVersionList, string.Empty);
			default:
				return null;
			}
		}

		public string GetConnectionStringForScript()
		{
			if (!WinformsHelper.IsRemoteEnabled())
			{
				return "pooled=false";
			}
			return "timeout=30";
		}

		private static Fqdn DiscoverExchangeServer()
		{
			string fqdn = null;
			using (new OpenConnection(PSConnectionInfoSingleton.connection))
			{
				string str = Path.Combine(ConfigurationContext.Setup.BinPath, "ConnectFunctions.ps1");
				using (MonadCommand monadCommand = new LoggableMonadCommand(". '" + str + "'", PSConnectionInfoSingleton.connection))
				{
					monadCommand.CommandType = CommandType.Text;
					monadCommand.ExecuteNonQuery();
				}
				using (MonadCommand monadCommand2 = new LoggableMonadCommand("Discover-ExchangeServer", PSConnectionInfoSingleton.connection))
				{
					monadCommand2.CommandType = CommandType.StoredProcedure;
					monadCommand2.Parameters.Add(new MonadParameter("UseWIA", true));
					monadCommand2.Parameters.Add(new MonadParameter("SuppressError", true));
					monadCommand2.Parameters.Add(new MonadParameter("CurrentVersion", ServerVersion.InstalledVersion));
					object[] array = monadCommand2.Execute();
					if (array == null || array.Length <= 0)
					{
						throw new CmdletInvocationException(Strings.FailedToAutoDiscoverExchangeServer);
					}
					fqdn = (array[0] as string);
				}
			}
			return new Fqdn(fqdn);
		}

		public static Uri GetRemotePowerShellUri(Fqdn server)
		{
			if (server == null)
			{
				return null;
			}
			return new Uri(string.Format(PSConnectionInfoSingleton.RemotePowerShellUrlFormat, server.ToString()));
		}

		public static Uri GetExchangeOnlineUri()
		{
			if (PSConnectionInfoSingleton.exchangeOnlineUri != null)
			{
				return PSConnectionInfoSingleton.exchangeOnlineUri;
			}
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools";
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
				{
					if (registryKey != null)
					{
						string text = registryKey.GetValue("EMC.ExchangeOnlineUri") as string;
						if (!string.IsNullOrEmpty(text))
						{
							PSConnectionInfoSingleton.exchangeOnlineUri = new Uri(text);
						}
					}
				}
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			if (PSConnectionInfoSingleton.exchangeOnlineUri == null)
			{
				PSConnectionInfoSingleton.exchangeOnlineUri = new Uri(PSConnectionInfoSingleton.ExchangeOnlineUri);
			}
			return PSConnectionInfoSingleton.exchangeOnlineUri;
		}

		private static readonly string RemotePowerShellUrlFormat = "http://{0}/PowerShell";

		private static readonly string ExchangeOnlineUri = "https://ps.outlook.com/PowerShell/PowerShell.htm";

		private static readonly string ExchangeOnlineEcpServer = "www.outlook.com";

		private static PSConnectionInfoSingleton instance;

		private object syncObject = new object();

		private EventHandlerList eventHandlers = new EventHandlerList();

		private static MonadConnection connection = new MonadConnection("pooled=false");

		private Fqdn remotePSServer;

		private object remotePSServerChangedEvent = new object();

		private MonadConnectionInfo defaultConnectionInfo;

		private Fqdn autoDiscoveredServer;

		private IDictionary<string, MonadConnectionInfo> forestConnectionInfos = new Dictionary<string, MonadConnectionInfo>();

		private static Uri exchangeOnlineUri;
	}
}
