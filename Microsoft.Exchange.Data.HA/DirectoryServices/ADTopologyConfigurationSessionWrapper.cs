using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADTopologyConfigurationSessionWrapper : IADToplogyConfigurationSession
	{
		public bool UseConfigNC
		{
			get
			{
				return this.m_session.UseConfigNC;
			}
			set
			{
				this.m_session.UseConfigNC = value;
			}
		}

		public bool UseGlobalCatalog
		{
			get
			{
				return this.m_session.UseGlobalCatalog;
			}
			set
			{
				this.m_session.UseGlobalCatalog = value;
			}
		}

		public static ADTopologyConfigurationSessionWrapper CreateWrapper(ITopologyConfigurationSession session)
		{
			ExAssert.RetailAssert(session != null, "'session' instance to wrap must not be null!");
			return new ADTopologyConfigurationSessionWrapper(session);
		}

		private ADTopologyConfigurationSessionWrapper(ITopologyConfigurationSession session)
		{
			ExAssert.RetailAssert(session != null, "'session' instance to wrap must not be null!");
			this.m_session = session;
		}

		public IADServer FindServerByName(string serverShortName)
		{
			Server server = this.m_session.FindServerByName(serverShortName);
			return ADObjectWrapperFactory.CreateWrapper(server);
		}

		public IADDatabaseAvailabilityGroup FindDagByServer(IADServer server)
		{
			if (server != null)
			{
				ADObjectId databaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
				if (databaseAvailabilityGroup != null && !databaseAvailabilityGroup.IsDeleted)
				{
					DatabaseAvailabilityGroup dag = this.m_session.Read<DatabaseAvailabilityGroup>(databaseAvailabilityGroup);
					return ADObjectWrapperFactory.CreateWrapper(dag);
				}
			}
			return null;
		}

		public IADComputer FindComputerByHostName(string hostName)
		{
			ADComputer adComputer = this.m_session.FindComputerByHostName(hostName);
			return ADObjectWrapperFactory.CreateWrapper(adComputer);
		}

		public IEnumerable<IADDatabase> GetAllDatabases(IADServer server)
		{
			IEnumerable<DatabaseCopy> copies = this.GetAllRealDatabaseCopies(server);
			foreach (DatabaseCopy copy in copies)
			{
				ADObjectId dbId = ((ADObjectId)copy.Identity).Parent;
				IADDatabase database = this.ReadADObject<IADDatabase>(dbId);
				if (database != null)
				{
					yield return database;
				}
			}
			yield break;
		}

		public IEnumerable<IADDatabaseCopy> GetAllDatabaseCopies(IADServer server)
		{
			IEnumerable<DatabaseCopy> allRealDatabaseCopies = this.GetAllRealDatabaseCopies(server);
			return from copy in allRealDatabaseCopies
			select ADObjectWrapperFactory.CreateWrapper(copy);
		}

		private IEnumerable<DatabaseCopy> GetAllRealDatabaseCopies(IADServer server)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, server.Name);
			return this.m_session.FindPaged<DatabaseCopy>(null, QueryScope.SubTree, filter, null, 0);
		}

		public IADDatabase FindDatabaseByGuid(Guid dbGuid)
		{
			if (dbGuid == Guid.Empty)
			{
				throw new ArgumentException("dbGuid cannot be Empty.");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, dbGuid);
			IADDatabase[] array = this.Find<IADDatabase>(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length <= 0)
			{
				return null;
			}
			return array[0];
		}

		public TADWrapperObject ReadADObject<TADWrapperObject>(ADObjectId objectId) where TADWrapperObject : class, IADObjectCommon
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, objectId);
			TADWrapperObject[] array = this.Find<TADWrapperObject>(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length <= 0)
			{
				return default(TADWrapperObject);
			}
			return array[0];
		}

		public TADWrapperObject[] Find<TADWrapperObject>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults) where TADWrapperObject : class, IADObjectCommon
		{
			if (typeof(TADWrapperObject).Equals(typeof(IADDatabase)))
			{
				IADDatabase[] array = (IADDatabase[])this.FindInternal<TADWrapperObject, MiniDatabase>(rootId, scope, filter, sortBy, maxResults, ADDatabaseWrapper.PropertiesNeededForDatabase);
				if (array != null)
				{
					foreach (IADDatabase iaddatabase in array)
					{
						((ADDatabaseWrapper)iaddatabase).FinishConstructionFromMiniDatabase(this.m_session);
					}
				}
				return (TADWrapperObject[])array;
			}
			if (typeof(TADWrapperObject).Equals(typeof(IADDatabaseCopy)))
			{
				return this.FindInternal<TADWrapperObject, DatabaseCopy>(rootId, scope, filter, sortBy, maxResults, null);
			}
			if (typeof(TADWrapperObject).Equals(typeof(IADServer)))
			{
				return this.FindInternal<TADWrapperObject, MiniServer>(rootId, scope, filter, sortBy, maxResults, ADServerWrapper.PropertiesNeededForServer);
			}
			if (typeof(TADWrapperObject).Equals(typeof(IADDatabaseAvailabilityGroup)))
			{
				return this.FindInternal<TADWrapperObject, DatabaseAvailabilityGroup>(rootId, scope, filter, sortBy, maxResults, null);
			}
			if (typeof(TADWrapperObject).Equals(typeof(IADClientAccessArray)))
			{
				return this.FindInternal<TADWrapperObject, ClientAccessArray>(rootId, scope, QueryFilter.AndTogether(new QueryFilter[]
				{
					filter,
					ClientAccessArray.PriorTo15ExchangeObjectVersionFilter
				}), sortBy, maxResults, null);
			}
			ExAssert.RetailAssert(false, "Unhandled type '{0}' !", new object[]
			{
				typeof(TADWrapperObject).FullName
			});
			return null;
		}

		private TADWrapperObject[] FindInternal<TADWrapperObject, TADObject>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties = null) where TADWrapperObject : class, IADObjectCommon where TADObject : ADObject, new()
		{
			TADObject[] array = this.m_session.Find<TADObject>(rootId, scope, filter, sortBy, maxResults, properties);
			if (array == null)
			{
				return null;
			}
			return (from o in array
			select (TADWrapperObject)((object)ADObjectWrapperFactory.CreateWrapper(o))).ToArray<TADWrapperObject>();
		}

		public IADServer ReadMiniServer(ADObjectId entryId)
		{
			MiniServer server = this.m_session.ReadMiniServer(entryId, ADServerWrapper.PropertiesNeededForServer);
			return ADObjectWrapperFactory.CreateWrapper(server);
		}

		public IADServer FindMiniServerByName(string serverName)
		{
			MiniServer server = this.m_session.FindMiniServerByName(serverName, ADServerWrapper.PropertiesNeededForServer);
			return ADObjectWrapperFactory.CreateWrapper(server);
		}

		public bool TryFindByExchangeLegacyDN(string legacyExchangeDN, out IADMiniClientAccessServerOrArray adMiniClientAccessServerOrArray)
		{
			MiniClientAccessServerOrArray caServerOrArray = null;
			bool result = this.m_session.TryFindByExchangeLegacyDN(legacyExchangeDN, ADMiniClientAccessServerOrArrayWrapper.PropertiesNeededForCas, out caServerOrArray);
			adMiniClientAccessServerOrArray = ADObjectWrapperFactory.CreateWrapper(caServerOrArray);
			return result;
		}

		public IADMiniClientAccessServerOrArray ReadMiniClientAccessServerOrArray(ADObjectId entryId)
		{
			MiniClientAccessServerOrArray caServerOrArray = this.m_session.ReadMiniClientAccessServerOrArray(entryId, ADMiniClientAccessServerOrArrayWrapper.PropertiesNeededForCas);
			return ADObjectWrapperFactory.CreateWrapper(caServerOrArray);
		}

		public IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByFqdn(string serverFqdn)
		{
			MiniClientAccessServerOrArray caServerOrArray = this.m_session.FindMiniClientAccessServerOrArrayByFqdn(serverFqdn, ADMiniClientAccessServerOrArrayWrapper.PropertiesNeededForCas);
			return ADObjectWrapperFactory.CreateWrapper(caServerOrArray);
		}

		public IADSite GetLocalSite()
		{
			ADSite localSite = this.m_session.GetLocalSite();
			return ADObjectWrapperFactory.CreateWrapper(localSite);
		}

		private ITopologyConfigurationSession m_session;
	}
}
