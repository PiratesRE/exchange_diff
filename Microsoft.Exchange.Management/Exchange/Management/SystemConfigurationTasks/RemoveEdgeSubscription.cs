using System;
using System.DirectoryServices.Protocols;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessageSecurity;
using Microsoft.Exchange.MessageSecurity.EdgeSync;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "EdgeSubscription", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveEdgeSubscription : RemoveSystemConfigurationObjectTask<TransportServerIdParameter, Server>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveEdgeSubscription(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			internal get
			{
				return new SwitchParameter(this.force);
			}
			set
			{
				this.force = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			Server server = null;
			try
			{
				server = topologyConfigurationSession.ReadLocalServer();
			}
			catch (ADTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, base.DataObject);
				return;
			}
			catch (TransientException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ResourceUnavailable, base.DataObject);
				return;
			}
			catch (LocalServerNotFoundException)
			{
			}
			if (server != null && server.IsEdgeServer)
			{
				this.HandleRemoveOnEdge(topologyConfigurationSession);
				return;
			}
			this.HandleRemoveInsideOrg(topologyConfigurationSession);
		}

		private void HandleRemoveInsideOrg(IConfigurationSession scSession)
		{
			Server dataObject = base.DataObject;
			if (!dataObject.IsEdgeServer)
			{
				base.WriteError(new InvalidOperationException(Strings.WrongSubscriptionIdentity), ErrorCategory.InvalidOperation, null);
			}
			try
			{
				this.RemoveEdgeFromConnectorSourceServers(scSession, dataObject);
				AdamUserManagement.RemoveSubscriptionCredentialsOnAllBHs(dataObject.Fqdn);
				scSession.DeleteTree(base.DataObject, delegate(ADTreeDeleteNotFinishedException de)
				{
					base.WriteVerbose(de.LocalizedString);
				});
			}
			catch (ADTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
		}

		private void HandleRemoveOnEdge(ITopologyConfigurationSession scSession)
		{
			try
			{
				this.RemoveEdgeSyncedObjects<DomainContentConfig>(scSession);
				this.RemoveEdgeSyncedObjects<MicrosoftExchangeRecipient>(scSession);
				this.RemoveEdgeSyncedObjects<AcceptedDomain>(scSession);
				this.RemoveEdgeSyncedObjects<MessageClassification>(scSession);
				this.RemoveEdgeSyncedObjects<MailGateway>(scSession);
				this.RemoveEdgeSyncedHubServerObjects(scSession);
				AdamUserManagement.RemoveAllADAMPrincipals();
				base.DataObject.EdgeSyncLease = null;
				base.DataObject.EdgeSyncCredentials = null;
				base.DataObject.EdgeSyncStatus = null;
				base.DataObject.GatewayEdgeSyncSubscribed = false;
				base.DataObject.EdgeSyncCookies = null;
				base.DataObject.EdgeSyncSourceGuid = null;
				scSession.Save(base.DataObject);
				this.RestoreTransportSettings(scSession);
				if (this.force || base.ShouldContinue(Strings.ConfirmationMessageRemoveEdgeSubscriptionRecipients))
				{
					this.RemoveAllEdgeSyncedRecipients();
				}
			}
			catch (ADTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (MessageSecurityException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
			}
			catch (LdapException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
		}

		private void RemoveEdgeFromConnectorSourceServers(IConfigurationSession scSession, Server server)
		{
			SmtpSendConnectorConfig[] array = scSession.Find<SmtpSendConnectorConfig>(null, QueryScope.SubTree, null, null, -1);
			foreach (SmtpSendConnectorConfig smtpSendConnectorConfig in array)
			{
				if (smtpSendConnectorConfig != null && smtpSendConnectorConfig.SourceTransportServers != null)
				{
					MultiValuedProperty<ADObjectId> sourceTransportServers = smtpSendConnectorConfig.SourceTransportServers;
					ADObjectId adobjectId = null;
					foreach (ADObjectId adobjectId2 in sourceTransportServers)
					{
						if (adobjectId2.Name == server.Id.Name)
						{
							if (smtpSendConnectorConfig.SourceTransportServers.Count == 1)
							{
								scSession.Delete(smtpSendConnectorConfig);
							}
							else
							{
								adobjectId = adobjectId2;
							}
						}
					}
					if (adobjectId != null)
					{
						sourceTransportServers.Remove(adobjectId);
						smtpSendConnectorConfig.SourceTransportServers = sourceTransportServers;
						scSession.Save(smtpSendConnectorConfig);
					}
				}
			}
		}

		private void RestoreTransportSettings(IConfigurationSession scSession)
		{
			ADPagedReader<TransportConfigContainer> adpagedReader = scSession.FindPaged<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 0);
			foreach (TransportConfigContainer transportConfigContainer in adpagedReader)
			{
				transportConfigContainer.TLSSendDomainSecureList = null;
				transportConfigContainer.TLSReceiveDomainSecureList = null;
				transportConfigContainer.InternalSMTPServers = null;
				scSession.Save(transportConfigContainer);
			}
		}

		private void RemoveEdgeSyncedHubServerObjects(ITopologyConfigurationSession scSession)
		{
			ADPagedReader<Server> adpagedReader = scSession.FindAllServersWithVersionNumber(Server.E2007MinVersion);
			foreach (Server server in adpagedReader)
			{
				if (server.IsHubTransportServer)
				{
					scSession.DeleteTree(server, delegate(ADTreeDeleteNotFinishedException de)
					{
						base.WriteVerbose(de.LocalizedString);
					});
				}
			}
		}

		private void RemoveAllEdgeSyncedRecipients()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 324, "RemoveAllEdgeSyncedRecipients", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ExchangeServer\\RemoveEdgeSubscription.cs");
			ADPagedReader<ADRecipient> adpagedReader = tenantOrRootOrgRecipientSession.FindPaged(null, QueryScope.SubTree, new TextFilter(ADRecipientSchema.EmailAddresses, "sh:", MatchOptions.Prefix, MatchFlags.IgnoreCase), null, 0);
			foreach (ADRecipient instanceToDelete in adpagedReader)
			{
				tenantOrRootOrgRecipientSession.Delete(instanceToDelete);
			}
		}

		private void RemoveEdgeSyncedObjects<T>(IConfigurationSession scSession) where T : ADConfigurationObject, new()
		{
			ADPagedReader<T> adpagedReader = scSession.FindPaged<T>(null, QueryScope.SubTree, null, null, 0);
			foreach (T t in adpagedReader)
			{
				scSession.DeleteTree(t, delegate(ADTreeDeleteNotFinishedException de)
				{
					base.WriteVerbose(de.LocalizedString);
				});
			}
		}

		private SwitchParameter force;
	}
}
