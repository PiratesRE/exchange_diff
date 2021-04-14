using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MiniServer : MiniObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniServer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MiniServer.mostDerivedClass;
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this[MiniServerSchema.Fqdn];
			}
		}

		public int VersionNumber
		{
			get
			{
				return (int)this[MiniServerSchema.VersionNumber];
			}
		}

		public int MajorVersion
		{
			get
			{
				return (int)this[MiniServerSchema.MajorVersion];
			}
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[MiniServerSchema.AdminDisplayVersion];
			}
		}

		public bool IsE14OrLater
		{
			get
			{
				return (bool)this[MiniServerSchema.IsE14OrLater];
			}
		}

		public ADObjectId ServerSite
		{
			get
			{
				return (ADObjectId)this[MiniServerSchema.ServerSite];
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[ServerSchema.ExchangeLegacyDN];
			}
		}

		public bool IsClientAccessServer
		{
			get
			{
				return (bool)this[ServerSchema.IsClientAccessServer];
			}
		}

		public bool IsExchange2007OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsExchange2007OrLater];
			}
		}

		public bool IsMailboxServer
		{
			get
			{
				return (bool)this[ServerSchema.IsMailboxServer];
			}
		}

		public ADObjectId DatabaseAvailabilityGroup
		{
			get
			{
				return (ADObjectId)this[ServerSchema.DatabaseAvailabilityGroup];
			}
		}

		public DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy
		{
			get
			{
				return (DatabaseCopyAutoActivationPolicyType)this[ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy];
			}
		}

		public bool DatabaseCopyActivationDisabledAndMoveNow
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.DatabaseCopyActivationDisabledAndMoveNow];
			}
		}

		public bool AutoDagServerConfigured
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.AutoDagServerConfigured];
			}
		}

		public MultiValuedProperty<string> ComponentStates
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.ComponentStates];
			}
		}

		public AutoDatabaseMountDial AutoDatabaseMountDial
		{
			get
			{
				return (AutoDatabaseMountDial)this[ActiveDirectoryServerSchema.AutoDatabaseMountDialType];
			}
		}

		public ServerRole CurrentServerRole
		{
			get
			{
				return (ServerRole)this[ServerSchema.CurrentServerRole];
			}
		}

		public ServerEditionType Edition
		{
			get
			{
				return (ServerEditionType)this[ServerSchema.Edition];
			}
		}

		public long? ContinuousReplicationMaxMemoryPerDatabase
		{
			get
			{
				return (long?)this[ActiveDirectoryServerSchema.ContinuousReplicationMaxMemoryPerDatabase];
			}
		}

		public int? MaximumActiveDatabases
		{
			get
			{
				return (int?)this[ServerSchema.MaxActiveMailboxDatabases];
			}
		}

		public int? MaximumPreferredActiveDatabases
		{
			get
			{
				return (int?)this[ServerSchema.MaxPreferredActiveDatabases];
			}
		}

		internal ITopologyConfigurationSession Session
		{
			get
			{
				return (ITopologyConfigurationSession)this.m_Session;
			}
		}

		internal Database[] GetDatabases()
		{
			return this.GetDatabases<Database>(false);
		}

		internal TDatabase[] GetDatabases<TDatabase>(bool allowInvalidCopies) where TDatabase : IConfigurable, new()
		{
			if (this.Session == null)
			{
				throw new InvalidOperationException("Server object does not have a session reference, so cannot get databases.");
			}
			List<TDatabase> list = new List<TDatabase>();
			if (this.IsE14OrLater)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, base.Name);
				DatabaseCopy[] array = this.Session.Find<DatabaseCopy>(null, QueryScope.SubTree, filter, null, 0);
				foreach (DatabaseCopy databaseCopy in array)
				{
					if (databaseCopy.IsValidDatabaseCopy(allowInvalidCopies))
					{
						TDatabase database = databaseCopy.GetDatabase<TDatabase>();
						if (database != null)
						{
							list.Add(database);
						}
					}
				}
			}
			else
			{
				list.AddRange(this.Session.FindPaged<TDatabase>(null, base.Id, true, null, 0));
			}
			return list.ToArray();
		}

		private static MiniServerSchema schema = ObjectSchema.GetInstance<MiniServerSchema>();

		private static string mostDerivedClass = "msExchExchangeServer";
	}
}
