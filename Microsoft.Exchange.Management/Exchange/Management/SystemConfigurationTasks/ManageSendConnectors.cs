using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class ManageSendConnectors
	{
		public static LocalizedException ValidateTransportServers(IConfigurationSession session, SendConnector sendConnector, ref ADObjectId routingGroupId, Task task, bool sourceValidation, out bool multiSiteConnector)
		{
			bool flag;
			return ManageSendConnectors.ValidateTransportServers(session, sendConnector, ref routingGroupId, false, sourceValidation, task, out flag, out multiSiteConnector);
		}

		public static LocalizedException ValidateTransportServers(IConfigurationSession session, SendConnector connector, ref ADObjectId routingGroupId, bool allowEdgeServers, bool sourceValidation, Task task, out bool edgeConnector, out bool multiSiteConnector)
		{
			edgeConnector = false;
			multiSiteConnector = false;
			MultiValuedProperty<ADObjectId> sourceTransportServers = connector.SourceTransportServers;
			if (sourceTransportServers != null && sourceTransportServers.Count != 0)
			{
				bool flag = false;
				ADObjectId adobjectId = null;
				MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
				List<string> list = new List<string>();
				foreach (ADObjectId adobjectId2 in sourceTransportServers)
				{
					if (adobjectId2 != null)
					{
						Server server = session.Read<Server>(adobjectId2);
						if (server == null)
						{
							list.Add(adobjectId2.Name);
						}
						else
						{
							if (adobjectId == null)
							{
								if (routingGroupId == null)
								{
									routingGroupId = ManageSendConnectors.GetServerRoutingGroup(server);
									if (routingGroupId == null)
									{
										return new SendConnectorUndefinedServerRgException(adobjectId2.Name);
									}
								}
								flag = ManageSendConnectors.IsE12RoutingGroup(routingGroupId);
								if (allowEdgeServers)
								{
									edgeConnector = server.IsEdgeServer;
								}
								adobjectId = server.ServerSite;
							}
							else if (!multiSiteConnector)
							{
								multiSiteConnector = !adobjectId.Equals(server.ServerSite);
							}
							if (!routingGroupId.Equals(server.HomeRoutingGroup))
							{
								bool flag2 = false;
								if (!flag)
								{
									flag2 = true;
								}
								else if (server.HomeRoutingGroup != null)
								{
									flag2 = true;
								}
								else if (!server.IsExchange2007OrLater)
								{
									flag2 = true;
								}
								if (flag2)
								{
									return sourceValidation ? new SendConnectorWrongSourceServerRgException(adobjectId2.Name) : new SendConnectorWrongTargetServerRgException(adobjectId2.Name);
								}
							}
							if (flag)
							{
								if ((!server.IsEdgeServer || !allowEdgeServers) && !server.IsHubTransportServer)
								{
									return sourceValidation ? new SendConnectorWrongSourceServerRoleException(adobjectId2.Name) : new SendConnectorWrongTargetServerRoleException(adobjectId2.Name);
								}
								if (edgeConnector != server.IsEdgeServer)
								{
									return new SendConnectorMixedSourceServerRolesException();
								}
							}
							multiValuedProperty.Add(adobjectId2);
						}
					}
				}
				if (multiValuedProperty.Count != 0)
				{
					if (multiValuedProperty.Count != connector.SourceTransportServers.Count)
					{
						connector.SourceTransportServers = multiValuedProperty;
						if (task != null)
						{
							task.WriteWarning(Strings.WarningSourceServersSkipped(string.Join(", ", list)));
						}
					}
					return null;
				}
				if (!sourceValidation)
				{
					return new SendConnectorValidTargetServerNotFoundException();
				}
				return new SendConnectorValidSourceServerNotFoundException();
			}
			if (!sourceValidation)
			{
				return new SendConnectorTargetServersNotSetException();
			}
			return new SendConnectorSourceServersNotSetException();
		}

		public static ADObjectId GetServerRoutingGroup(Server server)
		{
			ADObjectId adobjectId = server.HomeRoutingGroup;
			if (adobjectId == null && server.IsExchange2007OrLater)
			{
				ADObjectId parent = server.Id.Parent.Parent;
				adobjectId = parent.GetChildId("Routing Groups").GetChildId(RoutingGroup.DefaultName);
			}
			return adobjectId;
		}

		public static bool IsE12RoutingGroup(ADObjectId routingGroupId)
		{
			return RoutingGroup.DefaultName.Equals(routingGroupId.Name, StringComparison.OrdinalIgnoreCase);
		}

		public static void SetConnectorId(SendConnector connector, ADObjectId sourceRgId)
		{
			connector.SetId(sourceRgId.GetChildId("Connections").GetChildId(connector.Name));
			TaskLogger.Trace("Set connector ID to '{0}'", new object[]
			{
				connector.Id
			});
		}

		public static void SetConnectorHomeMta(SendConnector connector, IConfigurationSession configSession)
		{
			Server server = configSession.Read<Server>(connector.SourceTransportServers[0]);
			connector.HomeMTA = server.ResponsibleMTA;
			TaskLogger.Trace("Set connector homeMTA to '{0}'", new object[]
			{
				connector.HomeMTA
			});
		}

		public static void AddServersToGroup(IList<ADObjectId> serverIds, Guid groupGuid, ITopologyConfigurationSession configSession, ManageSendConnectors.ThrowTerminatingErrorDelegate throwDelegate)
		{
			TaskLogger.LogEnter();
			IRecipientSession recipSession;
			ITopologyConfigurationSession gcSession;
			ADGroup wellKnownGroup = ManageSendConnectors.GetWellKnownGroup(groupGuid, configSession, throwDelegate, out recipSession, out gcSession);
			ManageSendConnectors.AddServersToGroup(serverIds, wellKnownGroup, recipSession, gcSession, throwDelegate);
			TaskLogger.LogExit();
		}

		public static void AddServersToGroup(IList<ADObjectId> serverIds, ADGroup group, IRecipientSession recipSession, ITopologyConfigurationSession gcSession, ManageSendConnectors.ThrowTerminatingErrorDelegate throwDelegate)
		{
			TaskLogger.LogEnter();
			if (serverIds.Count == 0)
			{
				TaskLogger.LogExit();
				return;
			}
			foreach (ADObjectId adobjectId in serverIds)
			{
				ADComputer computerObject = ManageSendConnectors.GetComputerObject(adobjectId.Name, gcSession, throwDelegate);
				if (!group.Members.Contains(computerObject.Id))
				{
					TaskLogger.Trace("Adding server '{0}' to group '{1}'", new object[]
					{
						adobjectId.Name,
						group.Name
					});
					group.Members.Add(computerObject.Id);
				}
			}
			recipSession.Save(group);
			TaskLogger.LogExit();
		}

		public static void RemoveServersFromGroup(IList<ADObjectId> serverIds, Guid groupGuid, ITopologyConfigurationSession configSession, ManageSendConnectors.ThrowTerminatingErrorDelegate throwDelegate)
		{
			TaskLogger.LogEnter();
			IRecipientSession recipSession;
			ITopologyConfigurationSession gcSession;
			ADGroup wellKnownGroup = ManageSendConnectors.GetWellKnownGroup(groupGuid, configSession, throwDelegate, out recipSession, out gcSession);
			ManageSendConnectors.RemoveServersFromGroup(serverIds, wellKnownGroup, recipSession, gcSession, throwDelegate);
			TaskLogger.LogExit();
		}

		public static void RemoveServersFromGroup(IList<ADObjectId> serverIds, ADGroup group, IRecipientSession recipSession, ITopologyConfigurationSession gcSession, ManageSendConnectors.ThrowTerminatingErrorDelegate throwDelegate)
		{
			TaskLogger.LogEnter();
			if (serverIds.Count == 0)
			{
				TaskLogger.LogExit();
				return;
			}
			foreach (ADObjectId adobjectId in serverIds)
			{
				ADComputer computerObject = ManageSendConnectors.GetComputerObject(adobjectId.Name, gcSession, throwDelegate);
				if (group.Members.Contains(computerObject.Id))
				{
					TaskLogger.Trace("Removing server '{0}' from group '{1}'", new object[]
					{
						adobjectId.Name,
						group.Name
					});
					group.Members.Remove(computerObject.Id);
				}
			}
			recipSession.Save(group);
			TaskLogger.LogExit();
		}

		public static ADGroup GetWellKnownGroup(Guid groupGuid, IConfigurationSession configSession, ManageSendConnectors.ThrowTerminatingErrorDelegate throwDelegate, out IRecipientSession recipSession, out ITopologyConfigurationSession gcSession)
		{
			TaskLogger.LogEnter();
			ADGroup adgroup = null;
			recipSession = null;
			recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 530, "GetWellKnownGroup", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Transport\\ManageSendConnectors.cs");
			try
			{
				recipSession.UseGlobalCatalog = true;
				adgroup = recipSession.ResolveWellKnownGuid<ADGroup>(groupGuid, configSession.ConfigurationNamingContext);
				recipSession.UseGlobalCatalog = false;
				adgroup = (ADGroup)recipSession.Read(adgroup.Id);
			}
			finally
			{
				recipSession.UseGlobalCatalog = false;
			}
			if (adgroup == null)
			{
				try
				{
					ADDomain addomain = ADForest.GetLocalForest().FindRootDomain(true);
					if (addomain != null)
					{
						recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(addomain.OriginatingServer, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 564, "GetWellKnownGroup", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Transport\\ManageSendConnectors.cs");
						adgroup = recipSession.ResolveWellKnownGuid<ADGroup>(groupGuid, configSession.ConfigurationNamingContext);
					}
				}
				catch (ADReferralException)
				{
				}
			}
			if (adgroup == null)
			{
				throwDelegate(new ErrorExchangeGroupNotFoundException(groupGuid), ErrorCategory.ObjectNotFound, null);
			}
			gcSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 589, "GetWellKnownGroup", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Transport\\ManageSendConnectors.cs");
			gcSession.UseConfigNC = false;
			gcSession.UseGlobalCatalog = true;
			TaskLogger.LogExit();
			return adgroup;
		}

		public static IList<RoutingGroupConnector> GetAlternateRgcs(RoutingGroupConnector connectorBeingProcessed, IConfigurationSession session, out RoutingGroupConnector savedConnector)
		{
			TaskLogger.Trace("Reading alternate RGCs for RG '{0}'", new object[]
			{
				connectorBeingProcessed.SourceRoutingGroup
			});
			ADPagedReader<RoutingGroupConnector> adpagedReader = session.FindPaged<RoutingGroupConnector>(connectorBeingProcessed.SourceRoutingGroup, QueryScope.SubTree, null, null, ADGenericPagedReader<RoutingGroupConnector>.DefaultPageSize);
			savedConnector = null;
			IList<RoutingGroupConnector> list = new List<RoutingGroupConnector>();
			foreach (RoutingGroupConnector routingGroupConnector in adpagedReader)
			{
				if (connectorBeingProcessed.TargetRoutingGroup.Equals(routingGroupConnector.TargetRoutingGroup))
				{
					if (connectorBeingProcessed.Id.Equals(routingGroupConnector.Id))
					{
						savedConnector = routingGroupConnector;
					}
					else
					{
						list.Add(routingGroupConnector);
					}
				}
			}
			TaskLogger.Trace("Found {0} alternate RGCs", new object[]
			{
				list.Count
			});
			return list;
		}

		public static void PruneServerList(IList<ADObjectId> serverIds, IList<RoutingGroupConnector> alternateConnectors)
		{
			TaskLogger.LogEnter();
			int i = 0;
			while (i < serverIds.Count)
			{
				bool flag = false;
				foreach (RoutingGroupConnector routingGroupConnector in alternateConnectors)
				{
					if (routingGroupConnector.SourceTransportServers.Contains(serverIds[i]))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					TaskLogger.Trace("Removing server '{0}' from the list", new object[]
					{
						serverIds[i].Name
					});
					serverIds.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			TaskLogger.LogExit();
		}

		public static void UpdateGwartLastModified(ITopologyConfigurationSession configSession, ADObjectId sourceRoutingGroup, ManageSendConnectors.ThrowTerminatingErrorDelegate throwDelegate)
		{
			TaskLogger.LogEnter();
			if (ManageSendConnectors.IsE12RoutingGroup(sourceRoutingGroup))
			{
				configSession.UpdateGwartLastModified();
				TaskLogger.Trace("Updated E12 Legacy GWART {0}", new object[]
				{
					LegacyGwart.DefaultName
				});
			}
			TaskLogger.LogExit();
		}

		internal static void AdjustAddressSpaces(MailGateway connector)
		{
			if (MultiValuedPropertyBase.IsNullOrEmpty(connector.AddressSpaces))
			{
				return;
			}
			MultiValuedProperty<AddressSpace> multiValuedProperty = new MultiValuedProperty<AddressSpace>();
			foreach (AddressSpace addressSpace in connector.AddressSpaces)
			{
				if (addressSpace.IsLocal != connector.IsScopedConnector)
				{
					multiValuedProperty.Add(addressSpace);
				}
			}
			if (multiValuedProperty.Count > 0)
			{
				foreach (AddressSpace addressSpace2 in multiValuedProperty)
				{
					AddressSpace item = addressSpace2.ToggleScope();
					connector.AddressSpaces.Remove(addressSpace2);
					connector.AddressSpaces.Add(item);
				}
			}
		}

		private static ADComputer GetComputerObject(string name, ITopologyConfigurationSession gcSession, ManageSendConnectors.ThrowTerminatingErrorDelegate throwDelegate)
		{
			TaskLogger.LogEnter();
			ADComputer adcomputer = gcSession.FindComputerByHostName(name);
			if (adcomputer == null)
			{
				throwDelegate(new SendConnectorComputerNotFoundException(name), ErrorCategory.ObjectNotFound, null);
			}
			TaskLogger.LogExit();
			return adcomputer;
		}

		public delegate void ThrowTerminatingErrorDelegate(Exception exception, ErrorCategory category, object target);

		public delegate IConfigurable GetServerDelegate<Server>(ADIdParameter dataObjectId, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError);
	}
}
