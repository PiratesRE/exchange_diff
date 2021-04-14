using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory.ExchangeDirectory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class TopologyConfigurationSessionAdapter
	{
		public abstract TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults) where TResult : ADObject, new();

		public abstract IEnumerable<T> FindAllPaged<T>() where T : ADConfigurationObject, new();

		public abstract IEnumerable<TResult> FindPaged<TResult>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where TResult : ADObject, new();

		public abstract Server FindServerByFqdn(Fqdn fqdn);

		public abstract IEnumerable<MailboxDatabase> GetDatabasesOnServer(DirectoryIdentity serverIdentity);

		public abstract T Read<T>(ADObjectId objectId) where T : ADConfigurationObject, new();

		public abstract MiniServer ReadMiniServer(ADObjectId entryId, IEnumerable<PropertyDefinition> properties);

		public static readonly Hookable<TopologyConfigurationSessionAdapter> Instance = Hookable<TopologyConfigurationSessionAdapter>.Create(true, new TopologyConfigurationSessionAdapter.ADDriverTopologyConfigurationAdapter());

		private class ADDriverTopologyConfigurationAdapter : TopologyConfigurationSessionAdapter
		{
			private ITopologyConfigurationSession ConfigurationSession
			{
				get
				{
					return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 111, "ConfigurationSession", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\MailboxLoadBalance\\Directory\\ExchangeDirectory\\TopologyConfigurationSessionAdapter.cs");
				}
			}

			public override TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
			{
				return this.ConfigurationSession.Find<TResult>(null, scope, filter, sortBy, maxResults);
			}

			public override IEnumerable<T> FindAllPaged<T>()
			{
				return this.ConfigurationSession.FindAllPaged<T>();
			}

			public override IEnumerable<TResult> FindPaged<TResult>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
			{
				return this.ConfigurationSession.FindPaged<TResult>(filter, rootId, deepSearch, sortBy, pageSize);
			}

			public override Server FindServerByFqdn(Fqdn fqdn)
			{
				return this.ConfigurationSession.FindServerByFqdn(fqdn.ToString());
			}

			public override IEnumerable<MailboxDatabase> GetDatabasesOnServer(DirectoryIdentity serverIdentity)
			{
				QueryFilter serverNameFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serverIdentity.Name);
				IEnumerable<DatabaseCopy> copies = this.ConfigurationSession.Find<DatabaseCopy>(null, QueryScope.SubTree, serverNameFilter, null, 0);
				IEnumerable<DatabaseCopy> validCopies = from dbCopy in copies
				where dbCopy.IsValidDatabaseCopy(false)
				select dbCopy;
				IEnumerable<MailboxDatabase> databases = from dbCopy in validCopies
				select dbCopy.GetDatabase<MailboxDatabase>() into db
				where db != null
				select db;
				foreach (MailboxDatabase database in databases)
				{
					if (object.Equals((from ap in database.ActivationPreference
					orderby ap.Value
					select ap).FirstOrDefault<KeyValuePair<ADObjectId, int>>().Key.ObjectGuid, serverIdentity.Guid))
					{
						yield return database;
					}
				}
				yield break;
			}

			public override T Read<T>(ADObjectId objectId)
			{
				return this.ConfigurationSession.Read<T>(objectId);
			}

			public override MiniServer ReadMiniServer(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
			{
				return this.ConfigurationSession.ReadMiniServer(entryId, properties);
			}
		}
	}
}
