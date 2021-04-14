using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal class Utility
	{
		public static UMIPGateway[] FindGatewayByIPAddress(string addressOrFQDN, IConfigurationSession session)
		{
			if (addressOrFQDN == null)
			{
				throw new ArgumentNullException("addressOrFQDN");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, UMIPGatewaySchema.Address, addressOrFQDN);
			return session.Find<UMIPGateway>(null, QueryScope.SubTree, filter, null, 0);
		}

		public static UMDialPlan GetDialPlanFromId(ADObjectId dialPlanId, IConfigurationSession session)
		{
			if (dialPlanId == null)
			{
				throw new ArgumentNullException("dialPlanId");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return session.Read<UMDialPlan>(dialPlanId);
		}

		public static bool IsUMLanguageAvailable(UMLanguage language)
		{
			MultiValuedProperty<UMLanguage> multiValuedProperty = Utils.ComputeUnionOfUmServerLanguages();
			return multiValuedProperty != null && multiValuedProperty.Contains(language);
		}

		public static bool DialPlanHasIncompatibleServers(UMDialPlan dialPlan, IConfigurationSession session, Dictionary<Guid, bool> checkedServers)
		{
			if (CommonConstants.UseDataCenterCallRouting)
			{
				return false;
			}
			if (dialPlan == null || dialPlan.UMServers.Count == 0)
			{
				return false;
			}
			foreach (ADObjectId adobjectId in dialPlan.UMServers)
			{
				if (!checkedServers.ContainsKey(adobjectId.ObjectGuid))
				{
					Server server = session.Read<Server>(adobjectId);
					if (server != null)
					{
						checkedServers[adobjectId.ObjectGuid] = true;
						if (!CommonUtil.IsServerCompatible(server))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static UMMailboxPolicy GetPolicyFromUser(ADUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (user.UMMailboxPolicy == null)
			{
				throw new UMMailboxPolicyNotPresentException(user.PrimarySmtpAddress.ToString());
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), user.OrganizationId, null, false), 164, "GetPolicyFromUser", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\Utils.cs");
			UMMailboxPolicy ummailboxPolicy = tenantOrTopologyConfigurationSession.Read<UMMailboxPolicy>(user.UMMailboxPolicy);
			if (ummailboxPolicy == null)
			{
				throw new UMMailboxPolicyIdNotFoundException(user.UMMailboxPolicy.Name.ToString());
			}
			return ummailboxPolicy;
		}

		public static UMServer GetUmServerFromId(ADObjectId serverId, IConfigurationSession session)
		{
			if (serverId == null)
			{
				throw new ArgumentNullException("serverId");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			Server server = session.Read<Server>(serverId);
			if (server != null && (server.CurrentServerRole & ServerRole.UnifiedMessaging) == ServerRole.UnifiedMessaging)
			{
				return new UMServer(server);
			}
			return null;
		}

		public static IRecipientSession GetRecipientSessionScopedToOrganization(OrganizationId orgId, bool readOnly)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(readOnly, ConsistencyMode.IgnoreInvalid, sessionSettings, 224, "GetRecipientSessionScopedToOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\Utils.cs");
		}

		public static Server GetServerFromName(string serverName, IConfigurationSession session)
		{
			if (serverName == null)
			{
				throw new ArgumentNullException("serverName");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			Server result = null;
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serverName),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.Fqdn, serverName)
			});
			Server[] array = session.Find<Server>(null, QueryScope.SubTree, filter, null, 0);
			if (array != null && array.Length == 1)
			{
				result = array[0];
			}
			return result;
		}

		public static List<UMServer> GetCompatibleUMRpcServers(ADObjectId site, UMDialPlan dialPlan, ITopologyConfigurationSession session)
		{
			List<UMServer> list = new List<UMServer>();
			if (CommonConstants.UseDataCenterCallRouting)
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					CommonUtil.GetCompatibleServersWithRole(VersionEnum.Compatible, ServerRole.UnifiedMessaging),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, site)
				});
				IEnumerable<Server> enumerable = session.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
				foreach (Server dataObject in enumerable)
				{
					list.Add(new UMServer(dataObject));
				}
				return list;
			}
			return Utility.GetCompatibleUMRpcServerList(dialPlan, session);
		}

		public static List<UMServer> GetCompatibleUMRpcServerList(UMDialPlan dialPlan, ITopologyConfigurationSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ADObjectId localSiteId = null;
			ADObjectId adobjectId = null;
			try
			{
				Server server = session.FindLocalServer();
				localSiteId = server.ServerSite;
				adobjectId = server.Id;
			}
			catch (LocalServerNotFoundException)
			{
			}
			Dictionary<ADObjectId, Server> dictionary = new Dictionary<ADObjectId, Server>();
			Utility.FillDictionaryWithCompatibleLocalUMServers(session, dictionary, localSiteId);
			int startIndex = 0;
			if (dictionary.Count == 0)
			{
				Utility.FillDictionaryWithCompatibleUMServers(session, dictionary);
			}
			List<UMServer> list = new List<UMServer>();
			if (dictionary.Count == 0)
			{
				return list;
			}
			if (adobjectId != null)
			{
				Utility.AddToUMRpcServerList(list, dictionary, adobjectId);
				startIndex = list.Count;
			}
			if (dialPlan != null && dialPlan.UMServers != null && dialPlan.UMServers.Count != 0)
			{
				foreach (ADObjectId serverId in dialPlan.UMServers)
				{
					Utility.AddToUMRpcServerList(list, dictionary, serverId);
				}
			}
			foreach (Server s in dictionary.Values)
			{
				Utility.AddUMServerToList(list, s);
			}
			dictionary.Clear();
			Utils.Shuffle<UMServer>(list, startIndex);
			return list;
		}

		public static ADRecipient GetSystemMailbox(ADObject dataObject, IConfigurationSession configSession)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			IRecipientSession recipientSessionScopedToOrganization = Utility.GetRecipientSessionScopedToOrganization(dataObject.OrganizationId, false);
			ADRecipient discoveryMailbox = MailboxDataProvider.GetDiscoveryMailbox(recipientSessionScopedToOrganization);
			ExAssert.RetailAssert(dataObject.OrganizationId.Equals(discoveryMailbox.OrganizationId), "The organization ids of the data object and system mailbox do not match.");
			return discoveryMailbox;
		}

		public static ADUser GetSystemMailbox(IRecipientSession recipientSession, Task task)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			ADUser result = null;
			try
			{
				result = MailboxDataProvider.GetDiscoveryMailbox(recipientSession);
			}
			catch (ObjectNotFoundException exception)
			{
				task.WriteError(exception, ErrorCategory.ReadError, null);
			}
			catch (NonUniqueRecipientException exception2)
			{
				task.WriteError(exception2, ErrorCategory.ReadError, null);
			}
			return result;
		}

		public static void ResetUMADProperties(ADUser user, bool keepProperties)
		{
			ValidateArgument.NotNull(user, "user");
			user.ClearEUMProxy(false, null);
			user.ClearASUMProxy();
			Utility.ResetADProperty(user, ADUserSchema.UMMailboxPolicy);
			Utility.ResetADProperty(user, ADRecipientSchema.UMRecipientDialPlanId);
			Utility.ResetADProperty(user, ADUserSchema.VoiceMailSettings);
			Utility.ResetADProperty(user, ADRecipientSchema.UMProvisioningRequested);
			if (keepProperties)
			{
				Utility.SetADProperty(user, ADUserSchema.UMEnabledFlags, user.UMEnabledFlags & ~UMEnabledFlags.UMEnabled);
				return;
			}
			foreach (ADPropertyDefinition property in DisableUMMailboxBase<MailboxIdParameter>.PropertiesToReset)
			{
				Utility.ResetADProperty(user, property);
			}
		}

		private static void ResetADProperty(ADUser user, ADPropertyDefinition property)
		{
			Utility.SetADProperty(user, property, property.DefaultValue);
		}

		private static void SetADProperty(ADUser user, ADPropertyDefinition property, object value)
		{
			if (!user.ExchangeVersion.IsOlderThan(property.VersionAdded))
			{
				user[property] = value;
			}
		}

		public static void CheckForPilotIdentifierDuplicates(ADConfigurationObject dataObject, IConfigurationSession session, MultiValuedProperty<string> e164NumberList, Task.TaskErrorLoggingDelegate writeErrorDelegate)
		{
			Utility.CheckForPilotIdentifierDuplicates<UMDialPlan>(dataObject, session, UMDialPlanSchema.PilotIdentifierList, e164NumberList, writeErrorDelegate);
			Utility.CheckForPilotIdentifierDuplicates<UMAutoAttendant>(dataObject, session, UMAutoAttendantSchema.PilotIdentifierList, e164NumberList, writeErrorDelegate);
		}

		private static void CheckForPilotIdentifierDuplicates<TResult>(ADConfigurationObject dataObject, IConfigurationSession session, ADPropertyDefinition propertyDefinition, MultiValuedProperty<string> e164NumberList, Task.TaskErrorLoggingDelegate writeErrorDelegate) where TResult : ADConfigurationObject, new()
		{
			ValidateArgument.NotNull(session, "session");
			ValidateArgument.NotNull(dataObject, "dataObject");
			if (e164NumberList == null || e164NumberList.Count == 0)
			{
				return;
			}
			QueryFilter[] array = new QueryFilter[e164NumberList.Count];
			for (int i = 0; i < e164NumberList.Count; i++)
			{
				string propertyValue = e164NumberList[i];
				array[i] = new ComparisonFilter(ComparisonOperator.Equal, propertyDefinition, propertyValue);
			}
			TResult[] array2 = session.Find<TResult>(null, QueryScope.SubTree, new OrFilter(array), null, 2);
			if (array2 != null && array2.Length > 0 && (array2.Length > 1 || !array2[0].Id.Equals(dataObject.Id)))
			{
				writeErrorDelegate(new DuplicateE164PilotIdentifierListEntryException(array2[0].Name), ErrorCategory.InvalidOperation, dataObject);
			}
		}

		private static void AddToUMRpcServerList(List<UMServer> serverList, Dictionary<ADObjectId, Server> searchTable, ADObjectId serverId)
		{
			if (serverId != null && searchTable.ContainsKey(serverId))
			{
				Server s = searchTable[serverId];
				Utility.AddUMServerToList(serverList, s);
				searchTable.Remove(serverId);
			}
		}

		private static void FillDictionaryWithCompatibleLocalUMServers(IConfigurationSession session, Dictionary<ADObjectId, Server> searchTable, ADObjectId localSiteId)
		{
			if (localSiteId != null)
			{
				QueryFilter queryFilter = new AndFilter(new QueryFilter[]
				{
					CommonUtil.GetCompatibleServersWithRole(VersionEnum.Compatible, ServerRole.UnifiedMessaging),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, localSiteId)
				});
				Utility.FillServerDictionary(session, searchTable, queryFilter);
			}
		}

		private static void FillDictionaryWithCompatibleUMServers(IConfigurationSession session, Dictionary<ADObjectId, Server> searchTable)
		{
			QueryFilter compatibleServersWithRole = CommonUtil.GetCompatibleServersWithRole(VersionEnum.Compatible, ServerRole.UnifiedMessaging);
			Utility.FillServerDictionary(session, searchTable, compatibleServersWithRole);
		}

		private static void FillServerDictionary(IConfigurationSession session, Dictionary<ADObjectId, Server> searchTable, QueryFilter queryFilter)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ADPagedReader<Server> adpagedReader = session.FindPaged<Server>(null, QueryScope.SubTree, queryFilter, null, 100);
			if (adpagedReader != null)
			{
				foreach (Server server in adpagedReader)
				{
					if (server != null && !searchTable.ContainsKey(server.Id))
					{
						searchTable[server.Id] = server;
					}
				}
			}
		}

		private static void AddUMServerToList(List<UMServer> serverList, Server s)
		{
			if (s != null && s.IsUnifiedMessagingServer && CommonUtil.IsServerCompatible(s))
			{
				serverList.Add(new UMServer(s));
			}
		}
	}
}
