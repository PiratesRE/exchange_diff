using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ExchangeServerRole", DefaultParameterSetName = "Identity")]
	public sealed class SetExchangeServerRole : SetTopologySystemConfigurationObjectTask<ServerIdParameter, ExchangeServerRole, Server>
	{
		protected override void InternalProcessRecord()
		{
			Server dataObject = this.DataObject;
			dataObject.SetExchangeVersion(dataObject.MaximumSupportedExchangeObjectVersion);
			dataObject.MinAdminVersion = new int?(dataObject.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				this.CreateConfigContainerAndTransportServers(dataObject);
				ServerIdParameter.ClearServerRoleCache();
			}
		}

		private void CreateConfigContainerAndTransportServers(Server serverToSave)
		{
			ITopologyConfigurationSession scSession = (ITopologyConfigurationSession)base.DataSession;
			if (!serverToSave.IsEdgeServer)
			{
				ADObjectId id = serverToSave.Id;
				ADObjectId childId = id.GetChildId("Transport Configuration");
				if (base.DataSession.Find<ExchangeTransportConfigContainer>(null, childId, true, null).Length == 0)
				{
					ExchangeTransportConfigContainer exchangeTransportConfigContainer = new ExchangeTransportConfigContainer();
					exchangeTransportConfigContainer.SetId(childId);
					base.DataSession.Save(exchangeTransportConfigContainer);
				}
				this.ProcessFrontendTransportServerRole(serverToSave.IsFrontendTransportServer, scSession, childId);
				this.ProcessMailboxTransportServerRole(serverToSave.IsMailboxServer, scSession, childId);
			}
		}

		private void ProcessFrontendTransportServerRole(bool isFrontendTransportServer, ITopologyConfigurationSession scSession, ADObjectId configContainerId)
		{
			ADObjectId childId = configContainerId.GetChildId("Frontend");
			IConfigurable[] array = base.DataSession.Find<FrontendTransportServer>(null, childId, true, null);
			if (isFrontendTransportServer)
			{
				if (array.Length == 0)
				{
					FrontendTransportServer frontendTransportServer = new FrontendTransportServer();
					frontendTransportServer.NetworkAddress = this.DataObject.NetworkAddress;
					frontendTransportServer.Name = "Frontend";
					frontendTransportServer.Edition = ServerEditionType.StandardEvaluation;
					frontendTransportServer.AdminDisplayVersion = ConfigurationContext.Setup.InstalledVersion;
					frontendTransportServer.VersionNumber = SystemConfigurationTasksHelper.GenerateVersionNumber(ConfigurationContext.Setup.InstalledVersion);
					frontendTransportServer.ExchangeLegacyDN = LegacyDN.GenerateLegacyDN(Server.GetParentLegacyDN(scSession), frontendTransportServer);
					frontendTransportServer.CurrentServerRole = ServerRole.FrontendTransport;
					frontendTransportServer.SetId(childId);
					base.DataSession.Save(frontendTransportServer);
					return;
				}
			}
			else if (array.Length > 0)
			{
				base.DataSession.Delete(array[0]);
			}
		}

		private void ProcessMailboxTransportServerRole(bool isMailboxTransportServer, ITopologyConfigurationSession scSession, ADObjectId configContainerId)
		{
			ADObjectId childId = configContainerId.GetChildId("Mailbox");
			IConfigurable[] array = base.DataSession.Find<MailboxTransportServer>(null, childId, true, null);
			if (isMailboxTransportServer)
			{
				if (array.Length == 0)
				{
					MailboxTransportServer mailboxTransportServer = new MailboxTransportServer();
					mailboxTransportServer.NetworkAddress = this.DataObject.NetworkAddress;
					mailboxTransportServer.Name = "Mailbox";
					mailboxTransportServer.Edition = ServerEditionType.StandardEvaluation;
					mailboxTransportServer.AdminDisplayVersion = ConfigurationContext.Setup.InstalledVersion;
					mailboxTransportServer.VersionNumber = SystemConfigurationTasksHelper.GenerateVersionNumber(ConfigurationContext.Setup.InstalledVersion);
					mailboxTransportServer.ExchangeLegacyDN = LegacyDN.GenerateLegacyDN(Server.GetParentLegacyDN(scSession), mailboxTransportServer);
					mailboxTransportServer.CurrentServerRole = ServerRole.Mailbox;
					mailboxTransportServer.SetId(childId);
					base.DataSession.Save(mailboxTransportServer);
					return;
				}
			}
			else if (array.Length > 0)
			{
				base.DataSession.Delete(array[0]);
			}
		}
	}
}
