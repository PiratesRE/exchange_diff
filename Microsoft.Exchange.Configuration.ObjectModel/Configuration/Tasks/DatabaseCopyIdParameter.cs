using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DatabaseCopyIdParameter : ADIdParameter
	{
		public DatabaseCopyIdParameter(DatabaseCopy databaseCopy) : base(databaseCopy.Id)
		{
		}

		public DatabaseCopyIdParameter(MailboxServerIdParameter mailboxServerId) : base("*\\" + mailboxServerId.ToString())
		{
			this.Initialize(base.RawIdentity);
		}

		public DatabaseCopyIdParameter(MailboxServer server) : base("*\\" + server.Identity)
		{
			this.Initialize(base.RawIdentity);
		}

		public DatabaseCopyIdParameter(Database database) : base(database.Name + "\\*")
		{
			this.Initialize(base.RawIdentity);
		}

		public DatabaseCopyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
			this.Initialize(base.RawIdentity);
		}

		public DatabaseCopyIdParameter()
		{
		}

		public DatabaseCopyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected DatabaseCopyIdParameter(string identity) : base(identity)
		{
			this.Initialize(base.RawIdentity);
		}

		internal bool AllowInvalid { get; set; }

		internal bool AllowLegacy
		{
			get
			{
				return this.allowLegacy;
			}
			set
			{
				this.allowLegacy = value;
			}
		}

		internal string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		internal string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		protected ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.Mailbox;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return this.additionalQueryFilter;
			}
		}

		public static DatabaseCopyIdParameter Parse(string identity)
		{
			return new DatabaseCopyIdParameter(identity);
		}

		public override string ToString()
		{
			string result;
			if (base.InternalADObjectId == null)
			{
				result = this.DatabaseName + '\\' + this.ServerName;
			}
			else
			{
				result = base.ToString();
			}
			return result;
		}

		internal static DatabaseCopyIdParameter TestHookCreateDatabaseCopyIdParameter(string identity)
		{
			return new DatabaseCopyIdParameter(identity);
		}

		internal void SetAdditionalQueryFilter(QueryFilter newFilter)
		{
			this.additionalQueryFilter = newFilter;
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			var func = null;
			var func2 = null;
			var func3 = null;
			Func<DatabaseCopy, int> func4 = null;
			if (!typeof(DatabaseCopy).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			new List<T>();
			notFoundReason = null;
			if (base.InternalADObjectId != null)
			{
				return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			}
			IEnumerable<DatabaseCopy> enumerable;
			if (this.DatabaseName.Equals("*"))
			{
				enumerable = from dbCopy in base.PerformPrimarySearch<DatabaseCopy>(base.CreateWildcardOrEqualFilter(ADObjectSchema.Name, this.ServerName), rootId, session, true, optionalData)
				where dbCopy.IsValidDatabaseCopy(this.AllowInvalid)
				select dbCopy;
			}
			else
			{
				IEnumerable<MiniDatabase> source = this.PerformSearch<MiniDatabase>(base.CreateWildcardOrEqualFilter(ADObjectSchema.Name, this.DatabaseName), null, session, true);
				if (func == null)
				{
					func = ((MiniDatabase db) => new
					{
						db = db,
						tmpRootId = (db.Id ?? new ADObjectId(db.Name))
					});
				}
				var source2 = source.Select(func);
				var collectionSelector = <>h__TransparentIdentifier0 => this.PerformPrimarySearch<DatabaseCopy>(this.CreateWildcardOrEqualFilter(ADObjectSchema.Name, this.ServerName), <>h__TransparentIdentifier0.tmpRootId, session, true, optionalData);
				if (func2 == null)
				{
					func2 = ((<>h__TransparentIdentifier0, DatabaseCopy dbCopy) => new
					{
						<>h__TransparentIdentifier0,
						dbCopy
					});
				}
				var source3 = from <>h__TransparentIdentifier1 in source2.SelectMany(collectionSelector, func2)
				where <>h__TransparentIdentifier1.dbCopy.IsValidDatabaseCopy(this.AllowInvalid)
				select <>h__TransparentIdentifier1;
				if (func3 == null)
				{
					func3 = (<>h__TransparentIdentifier1 => <>h__TransparentIdentifier1.dbCopy);
				}
				enumerable = source3.Select(func3);
			}
			if (!this.DatabaseName.Contains("*"))
			{
				IEnumerable<DatabaseCopy> source4 = enumerable;
				if (func4 == null)
				{
					func4 = ((DatabaseCopy dbCopy) => dbCopy.ActivationPreferenceInternal);
				}
				enumerable = source4.OrderBy(func4);
			}
			return (IEnumerable<T>)enumerable;
		}

		protected void Initialize(string identity)
		{
			if (base.InternalADObjectId != null)
			{
				if (!(base.InternalADObjectId.Rdn != null))
				{
					Guid objectGuid = base.InternalADObjectId.ObjectGuid;
				}
				return;
			}
			string[] array = identity.Split(new char[]
			{
				'\\'
			});
			if (array.Length == 2)
			{
				this.databaseName = array[0];
				this.serverName = DatabaseCopyIdParameter.GetServerNameFromServerShortNameOrFqdn(array[1]);
			}
			else if (array.Length == 1)
			{
				this.databaseName = array[0];
				this.serverName = "*";
			}
			if (array.Length > 2)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
			if (string.IsNullOrEmpty(this.DatabaseName))
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
		}

		private static string GetServerNameFromServerShortNameOrFqdn(string serverName)
		{
			string text = serverName;
			if (text.Contains("."))
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 412, "GetServerNameFromServerShortNameOrFqdn", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\DatabaseCopyIdParameter.cs");
				topologyConfigurationSession.UseConfigNC = false;
				topologyConfigurationSession.UseGlobalCatalog = true;
				ADComputer adcomputer = topologyConfigurationSession.FindComputerByHostName(text);
				if (adcomputer == null)
				{
					throw new ArgumentException(Strings.ErrorInvalidServerName(text), "Identity");
				}
				text = ((ADObjectId)adcomputer.Identity).Name;
			}
			return text;
		}

		private string databaseName;

		private string serverName;

		private bool allowLegacy;

		private QueryFilter additionalQueryFilter;
	}
}
