using System;
using System.DirectoryServices.ActiveDirectory;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ExchangeServer")]
	public sealed class NewExchangeServer : NewFixedNameSystemConfigurationObjectTask<Server>
	{
		[Parameter]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			this.DataObject.Edition = ServerEditionType.StandardEvaluation;
			this.DataObject.AdminDisplayVersion = ConfigurationContext.Setup.InstalledVersion;
			this.DataObject.VersionNumber = SystemConfigurationTasksHelper.GenerateVersionNumber(ConfigurationContext.Setup.InstalledVersion);
			this.DataObject.MailboxRelease = MailboxRelease.E15;
			if (string.IsNullOrEmpty(this.Name))
			{
				string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(true);
				int num = localComputerFqdn.IndexOf('.');
				this.DataObject.Name = ((num == -1) ? localComputerFqdn : localComputerFqdn.Substring(0, num));
				NewExchangeServer.TcpNetworkAddress value = new NewExchangeServer.TcpNetworkAddress(NetworkProtocol.TcpIP, localComputerFqdn);
				this.DataObject.NetworkAddress = new NetworkAddressCollection(value);
			}
			this.DataObject.FaultZone = "FaultZone1";
			ADObjectId childId = topologyConfigurationSession.GetAdministrativeGroupId().GetChildId("Servers");
			ADObjectId childId2 = childId.GetChildId(this.DataObject.Name);
			this.DataObject.SetId(childId2);
			this.DataObject.ExchangeLegacyDN = LegacyDN.GenerateLegacyDN(Server.GetParentLegacyDN(topologyConfigurationSession), this.DataObject);
			using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, NativeHelpers.GetLocalComputerFqdn(true)))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey(NewExchangeServer.EdgeKeyName))
				{
					if (registryKey2 == null && this.IsDomainJoined())
					{
						this.SetServerSiteInformation(topologyConfigurationSession);
					}
				}
			}
			return this.DataObject;
		}

		private void SetServerSiteInformation(ITopologyConfigurationSession scSession)
		{
			if (scSession == null)
			{
				throw new ArgumentNullException("scSession");
			}
			Server dataObject = this.DataObject;
			if (dataObject == null)
			{
				this.WriteWarning(Strings.ServerObjectIsNullWarning);
				return;
			}
			if (dataObject.ServerSite != null)
			{
				base.WriteVerbose(Strings.ServerSiteInformationAlreadySet(dataObject.ServerSite.Name));
				return;
			}
			string siteName = NativeHelpers.GetSiteName(false);
			if (!string.IsNullOrEmpty(siteName))
			{
				string format = "CN={0},CN=Sites,{1}";
				ADObjectId configurationNamingContext = scSession.GetConfigurationNamingContext();
				if (configurationNamingContext != null)
				{
					ADObjectId serverSite = new ADObjectId(string.Format(format, siteName, configurationNamingContext.DistinguishedName));
					dataObject.ServerSite = serverSite;
					return;
				}
			}
			else
			{
				base.WriteVerbose(Strings.LocalSiteNameIsEmpty);
			}
		}

		private bool IsDomainJoined()
		{
			bool result = true;
			try
			{
				using (Domain.GetComputerDomain())
				{
				}
			}
			catch (ActiveDirectoryObjectNotFoundException)
			{
				result = false;
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				ADObjectId orgContainerId = ((IConfigurationSession)base.DataSession).GetOrgContainerId();
				ADObjectId childId = orgContainerId.GetChildId(NewExchangeServer.adminGroupContainer).GetChildId(NewExchangeServer.adminGroup).GetChildId(NewExchangeServer.serversContainer);
				ADObjectId childId2 = childId.GetChildId(this.DataObject.Name);
				ProtocolsContainer protocolsContainer = new ProtocolsContainer();
				ADObjectId childId3 = childId2.GetChildId(NewExchangeServer.protocolsContainer);
				protocolsContainer.SetId(childId3);
				base.DataSession.Save(protocolsContainer);
				SmtpContainer smtpContainer = new SmtpContainer();
				ADObjectId childId4 = childId3.GetChildId(NewExchangeServer.smtpContainer);
				smtpContainer.SetId(childId4);
				base.DataSession.Save(smtpContainer);
			}
		}

		private static readonly string adminGroupContainer = "Administrative Groups";

		private static readonly string serversContainer = "Servers";

		private static readonly string protocolsContainer = "Protocols";

		private static readonly string smtpContainer = "SMTP";

		private static readonly string adminGroup = AdministrativeGroup.DefaultName;

		private static readonly string EdgeKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole";

		internal class TcpNetworkAddress : NetworkAddress
		{
			public TcpNetworkAddress(NetworkProtocol protocol, string address) : base(protocol, address)
			{
			}
		}
	}
}
