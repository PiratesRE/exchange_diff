using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadRemoteRunspaceFactory : RemoteRunspaceFactory
	{
		static MonadRemoteRunspaceFactory()
		{
			AppDomain.CurrentDomain.DomainUnload += MonadRemoteRunspaceFactory.AppDomainUnloadEventHandler;
		}

		public MonadRemoteRunspaceFactory(MonadConnectionInfo connectionInfo) : this(connectionInfo, null)
		{
		}

		public MonadRemoteRunspaceFactory(MonadConnectionInfo connectionInfo, RunspaceServerSettingsPresentationObject serverSettings) : base(new RunspaceConfigurationFactory(), MonadHostFactory.GetInstance(), connectionInfo)
		{
			this.clientVersion = connectionInfo.ClientVersion;
			this.serverSettings = serverSettings;
		}

		public static SupportedVersionList TestConnection(Uri uri, string shell, PSCredential credential, AuthenticationMechanism mechanism, int maxRedirectionCount, bool skipCertificateCheck)
		{
			WSManConnectionInfo wsmanConnectionInfo = new WSManConnectionInfo(uri, shell, credential);
			wsmanConnectionInfo.AuthenticationMechanism = mechanism;
			wsmanConnectionInfo.MaximumConnectionRedirectionCount = maxRedirectionCount;
			if (skipCertificateCheck)
			{
				wsmanConnectionInfo.SkipCACheck = true;
				wsmanConnectionInfo.SkipCNCheck = true;
				wsmanConnectionInfo.SkipRevocationCheck = true;
			}
			Runspace runspace = null;
			SupportedVersionList result = null;
			Runspace runspace2;
			runspace = (runspace2 = RunspaceFactory.CreateRunspace(wsmanConnectionInfo));
			try
			{
				lock (MonadRemoteRunspaceFactory.syncObject)
				{
					MonadRemoteRunspaceFactory.runspaceInstances.Add(runspace);
				}
				runspace.Open();
				result = MonadRemoteRunspaceFactory.ExtractSupportedVersionList(runspace);
			}
			finally
			{
				if (runspace2 != null)
				{
					((IDisposable)runspace2).Dispose();
				}
			}
			lock (MonadRemoteRunspaceFactory.syncObject)
			{
				MonadRemoteRunspaceFactory.runspaceInstances.Remove(runspace);
			}
			return result;
		}

		protected override Runspace CreateRunspace(PSHost host)
		{
			Runspace runspace = base.CreateRunspace(host);
			if (!string.IsNullOrEmpty(this.clientVersion))
			{
				SupportedVersionList supportedVersionList = MonadRemoteRunspaceFactory.ExtractSupportedVersionList(runspace);
				if (supportedVersionList.IsSupported(this.clientVersion))
				{
					runspace.Dispose();
					throw new VersionMismatchException(Strings.VersionMismatchDuringCreateRemoteRunspace, supportedVersionList);
				}
			}
			return runspace;
		}

		internal static void ClearAppDomainRemoteRunspaceConnections()
		{
			lock (MonadRemoteRunspaceFactory.syncObject)
			{
				MonadRemoteRunspaceFactory.accessingRunspaceList = true;
				foreach (Runspace runspace in MonadRemoteRunspaceFactory.runspaceInstances)
				{
					if (runspace.RunspaceStateInfo.State == RunspaceState.Closed)
					{
						if (runspace.RunspaceStateInfo.State == RunspaceState.Closing)
						{
							continue;
						}
					}
					try
					{
						runspace.Close();
						runspace.Dispose();
					}
					catch (Exception)
					{
					}
				}
				MonadRemoteRunspaceFactory.runspaceInstances.Clear();
				MonadRemoteRunspaceFactory.accessingRunspaceList = false;
			}
		}

		protected override void InitializeRunspace(Runspace runspace)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("ConsoleInitialize.ps1");
			this.ExecuteCommand(pscommand, runspace);
			if (this.serverSettings != null)
			{
				pscommand = new PSCommand();
				pscommand.AddCommand("Set-ADServerSettingsForLogonUser");
				pscommand.AddParameter("RunspaceServerSettings", MonadCommand.Serialize(this.serverSettings));
				this.ExecuteCommand(pscommand, runspace);
			}
		}

		protected override void ConfigureRunspace(Runspace runspace)
		{
			lock (MonadRemoteRunspaceFactory.syncObject)
			{
				MonadRemoteRunspaceFactory.runspaceInstances.Add(runspace);
			}
			base.ConfigureRunspace(runspace);
		}

		protected override void OnRunspaceDisposed(Runspace runspace)
		{
			if (!MonadRemoteRunspaceFactory.accessingRunspaceList)
			{
				lock (MonadRemoteRunspaceFactory.syncObject)
				{
					MonadRemoteRunspaceFactory.runspaceInstances.Remove(runspace);
				}
			}
			base.OnRunspaceDisposed(runspace);
		}

		private static void AppDomainUnloadEventHandler(object sender, EventArgs args)
		{
			MonadRemoteRunspaceFactory.ClearAppDomainRemoteRunspaceConnections();
		}

		private void ExecuteCommand(PSCommand command, Runspace runspace)
		{
			using (PowerShell powerShell = PowerShell.Create())
			{
				powerShell.Commands = command;
				powerShell.Runspace = runspace;
				try
				{
					powerShell.Invoke();
				}
				catch (CmdletInvocationException ex)
				{
					if (ex.InnerException != null)
					{
						throw ex.InnerException;
					}
					throw;
				}
				if (powerShell.Streams.Error.Count > 0)
				{
					ErrorRecord errorRecord = powerShell.Streams.Error[0];
					throw new CmdletInvocationException(errorRecord.Exception.Message, errorRecord.Exception);
				}
			}
		}

		private static SupportedVersionList ExtractSupportedVersionList(Runspace newRunspace)
		{
			PSPrimitiveDictionary applicationPrivateData = newRunspace.GetApplicationPrivateData();
			object obj = applicationPrivateData["SupportedVersions"];
			return SupportedVersionList.Parse((obj != null) ? obj.ToString() : string.Empty);
		}

		private static List<Runspace> runspaceInstances = new List<Runspace>();

		private static volatile bool accessingRunspaceList = false;

		private static object syncObject = new object();

		private string clientVersion;

		private RunspaceServerSettingsPresentationObject serverSettings;
	}
}
