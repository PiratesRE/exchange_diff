using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MapiTaskHelper
	{
		public static bool IsDatacenter
		{
			get
			{
				if (!MapiTaskHelper.checkedForDatacenter)
				{
					MapiTaskHelper.isDatacenter = (Datacenter.GetExchangeSku() == Datacenter.ExchangeSku.ExchangeDatacenter);
					MapiTaskHelper.checkedForDatacenter = true;
				}
				return MapiTaskHelper.isDatacenter;
			}
		}

		public static bool IsDatacenterDedicated
		{
			get
			{
				if (!MapiTaskHelper.checkedForDatacenterDedicated)
				{
					MapiTaskHelper.isDatacenterDedicated = (Datacenter.GetExchangeSku() == Datacenter.ExchangeSku.DatacenterDedicated);
					MapiTaskHelper.checkedForDatacenterDedicated = true;
				}
				return MapiTaskHelper.isDatacenterDedicated;
			}
		}

		public static Server GetMailboxServer(ServerIdParameter serverIdParameter, ITopologyConfigurationSession configurationSession, Task.ErrorLoggerDelegate errorHandler)
		{
			if (serverIdParameter == null)
			{
				throw new ArgumentNullException("serverIdParameter");
			}
			if (configurationSession == null)
			{
				throw new ArgumentNullException("serverIdParameter");
			}
			if (errorHandler == null)
			{
				throw new ArgumentNullException("errorHandler");
			}
			IEnumerable<Server> objects = serverIdParameter.GetObjects<Server>(null, configurationSession);
			Server server = null;
			using (IEnumerator<Server> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					errorHandler(new ManagementObjectNotFoundException(Strings.ErrorServerNotFound(serverIdParameter.ToString())), ExchangeErrorCategory.Client, null);
					return null;
				}
				server = enumerator.Current;
				if (enumerator.MoveNext())
				{
					errorHandler(new ManagementObjectAmbiguousException(Strings.ErrorServerNotUnique(serverIdParameter.ToString())), ExchangeErrorCategory.Client, null);
					return null;
				}
			}
			if (!server.IsExchange2007OrLater)
			{
				errorHandler(new TaskInvalidOperationException(Strings.ExceptionLegacyObjects(serverIdParameter.ToString())), ExchangeErrorCategory.Context, null);
				return null;
			}
			if (!server.IsMailboxServer)
			{
				errorHandler(new TaskInvalidOperationException(Strings.ErrorNotMailboxServer(serverIdParameter.ToString())), ExchangeErrorCategory.Client, null);
				return null;
			}
			return server;
		}

		internal static OrganizationId ResolveTargetOrganization(Fqdn domainController, OrganizationIdParameter organization, ADObjectId rootOrgContainerId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId)
		{
			if (organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, currentOrganizationId, executingUserOrganizationId, false);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 183, "ResolveTargetOrganization", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\MapiTaskHelper.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = null;
				LocalizedString? localizedString = null;
				IEnumerable<ADOrganizationalUnit> objects = organization.GetObjects<ADOrganizationalUnit>(null, tenantOrTopologyConfigurationSession, null, out localizedString);
				using (IEnumerator<ADOrganizationalUnit> enumerator = objects.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						throw new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(organization.ToString()));
					}
					adorganizationalUnit = enumerator.Current;
					if (enumerator.MoveNext())
					{
						throw new ManagementObjectAmbiguousException(Strings.ErrorOrganizationNotUnique(organization.ToString()));
					}
				}
				return adorganizationalUnit.OrganizationId;
			}
			return currentOrganizationId ?? executingUserOrganizationId;
		}

		internal static OrganizationIdParameter ResolveTargetOrganizationIdParameter(OrganizationIdParameter organizationParameter, IIdentityParameter identity, OrganizationId currentOrganizationId, Task.ErrorLoggerDelegate errorHandler, Task.TaskWarningLoggingDelegate warningHandler)
		{
			OrganizationIdParameter organizationIdParameter = null;
			if (identity != null)
			{
				if (identity is MailPublicFolderIdParameter)
				{
					organizationIdParameter = (identity as MailPublicFolderIdParameter).Organization;
				}
				else if (identity is PublicFolderIdParameter)
				{
					organizationIdParameter = (identity as PublicFolderIdParameter).Organization;
				}
			}
			if (!currentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				if (organizationIdParameter != null)
				{
					errorHandler(new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(identity.ToString())), ExchangeErrorCategory.Client, identity);
				}
			}
			else
			{
				if (organizationParameter != null)
				{
					if (organizationIdParameter != null)
					{
						warningHandler(Strings.WarningDuplicateOrganizationSpecified(organizationParameter.ToString(), organizationIdParameter.ToString()));
					}
					organizationIdParameter = organizationParameter;
				}
				if (organizationIdParameter == null && !(identity is MailPublicFolderIdParameter))
				{
					errorHandler(new ErrorMissOrganizationException(), ExchangeErrorCategory.Client, null);
				}
			}
			return organizationIdParameter;
		}

		private static void GetServerForDatabase(Guid publicFolderDatabaseGuid, out string serverLegacyDn, out Fqdn serverFqdn)
		{
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(publicFolderDatabaseGuid, GetServerForDatabaseFlags.ThrowServerForDatabaseNotFoundException);
			serverFqdn = Fqdn.Parse(serverForDatabase.ServerFqdn);
			serverLegacyDn = serverForDatabase.ServerLegacyDN;
		}

		internal static DatabaseId ConvertDatabaseADObjectToDatabaseId(Database adObject)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			string serverName = adObject.ServerName;
			string text = adObject.Name;
			Guid guid = adObject.Guid;
			if (adObject.Identity != null)
			{
				DatabaseId databaseId = MapiTaskHelper.ConvertDatabaseADObjectIdToDatabaseId((ADObjectId)adObject.Identity);
				if (string.IsNullOrEmpty(serverName))
				{
					serverName = databaseId.ServerName;
				}
				if (string.IsNullOrEmpty(text))
				{
					text = databaseId.DatabaseName;
				}
				if (Guid.Empty == guid)
				{
					guid = databaseId.Guid;
				}
			}
			return new DatabaseId(null, serverName, text, guid);
		}

		internal static DatabaseId ConvertDatabaseADObjectIdToDatabaseId(ADObjectId adObjectId)
		{
			if (adObjectId == null)
			{
				throw new ArgumentNullException("adObjectId");
			}
			if (string.IsNullOrEmpty(adObjectId.DistinguishedName) || 3 > adObjectId.Depth)
			{
				return new DatabaseId(adObjectId.ObjectGuid);
			}
			return new DatabaseId(null, null, adObjectId.Name, adObjectId.ObjectGuid);
		}

		internal static MailboxStatistics[] GetStoreMailboxesFromId(IConfigDataProvider dataSession, StoreMailboxIdParameter identity, ObjectId rootId)
		{
			List<MailboxStatistics> list = new List<MailboxStatistics>();
			IEnumerable<MailboxStatistics> objects = identity.GetObjects<MailboxStatistics>(rootId, dataSession);
			if (objects == null)
			{
				return null;
			}
			foreach (MailboxStatistics item in objects)
			{
				list.Add(item);
			}
			return list.ToArray();
		}

		internal static MapiAdministrationSession GetAdminSession(ActiveManager activeManager, Guid databaseGuid)
		{
			DatabaseLocationInfo serverForDatabase = activeManager.GetServerForDatabase(databaseGuid);
			return new MapiAdministrationSession(serverForDatabase.ServerLegacyDN, Fqdn.Parse(serverForDatabase.ServerFqdn));
		}

		internal static string GetMailboxLegacyDN(MapiAdministrationSession mapiAdminSession, ADObjectId databaseId, Guid mailboxGuid)
		{
			string result = null;
			DatabaseId root = MapiTaskHelper.ConvertDatabaseADObjectIdToDatabaseId(databaseId);
			MailboxContextFilter filter = new MailboxContextFilter(mailboxGuid);
			MailboxStatistics[] array = null;
			try
			{
				array = mapiAdminSession.Find<MailboxStatistics>(filter, root, QueryScope.SubTree, null, 1);
			}
			catch (MapiObjectNotFoundException)
			{
			}
			if (array != null)
			{
				result = array[0].LegacyDN;
				array[0].Dispose();
			}
			return result;
		}

		internal static void VerifyIsWithinConfigWriteScope(ADSessionSettings sessionSettings, ADObject obj, Task.ErrorLoggerDelegate errorHandler)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 481, "VerifyIsWithinConfigWriteScope", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\MapiTaskHelper.cs");
			ADScopeException ex;
			if (!tenantOrTopologyConfigurationSession.TryVerifyIsWithinScopes(obj, true, out ex))
			{
				errorHandler(new IsOutofConfigWriteScopeException(obj.GetType().ToString(), obj.Name), ExchangeErrorCategory.Client, null);
			}
		}

		internal static void VerifyDatabaseAndItsOwningServerInScope(ADSessionSettings sessionSettings, Database database, Task.ErrorLoggerDelegate errorHandler)
		{
			MapiTaskHelper.VerifyDatabaseIsWithinScope(sessionSettings, database, errorHandler, true);
		}

		internal static void VerifyDatabaseIsWithinScope(ADSessionSettings sessionSettings, Database database, Task.ErrorLoggerDelegate errorHandler)
		{
			MapiTaskHelper.VerifyDatabaseIsWithinScope(sessionSettings, database, errorHandler, false);
		}

		internal static void VerifyServerIsWithinScope(Database database, Task.ErrorLoggerDelegate errorHandler, ITopologyConfigurationSession adConfigSession)
		{
			ADObjectId[] array = database.IsExchange2009OrLater ? database.Servers : new ADObjectId[]
			{
				database.Server
			};
			if (array == null || array.Length == 0)
			{
				errorHandler(new NoServersForDatabaseException(database.Name), ExchangeErrorCategory.Client, null);
			}
			bool flag = false;
			ADScopeException ex = null;
			foreach (ADObjectId adObjectId in array)
			{
				Server mailboxServer = MapiTaskHelper.GetMailboxServer(new ServerIdParameter(adObjectId), adConfigSession, errorHandler);
				if (adConfigSession.TryVerifyIsWithinScopes(mailboxServer, true, out ex))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				errorHandler(new IsOutofDatabaseScopeException(database.Name, ex.Message), ExchangeErrorCategory.Authorization, null);
			}
		}

		private static void VerifyDatabaseIsWithinScope(ADSessionSettings sessionSettings, Database database, Task.ErrorLoggerDelegate errorHandler, bool includeCheckForServer)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			if (errorHandler == null)
			{
				throw new ArgumentNullException("errorHandler");
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 613, "VerifyDatabaseIsWithinScope", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\MapiTaskHelper.cs");
			ADScopeException ex;
			if (!topologyConfigurationSession.TryVerifyIsWithinScopes(database, true, out ex))
			{
				errorHandler(new TaskInvalidOperationException(Strings.ErrorIsOutOfDatabaseScopeNoServerCheck(database.Name, ex.Message)), ExchangeErrorCategory.Authorization, null);
			}
			if (includeCheckForServer)
			{
				MapiTaskHelper.VerifyServerIsWithinScope(database, errorHandler, topologyConfigurationSession);
			}
		}

		private const int ServerRdnGenerationInDatabaseDN = 3;

		private const int StorageGroupRdnGenerationInDatabaseDN = 1;

		private static bool checkedForDatacenter;

		private static bool isDatacenter;

		private static bool checkedForDatacenterDedicated;

		private static bool isDatacenterDedicated;
	}
}
