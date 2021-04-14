using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DatabaseIdParameter : ADIdParameter
	{
		public DatabaseIdParameter(Database database) : base(database.Id)
		{
		}

		public DatabaseIdParameter(DatabaseCopyIdParameter databaseCopy) : this(databaseCopy.DatabaseName)
		{
		}

		public DatabaseIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public DatabaseIdParameter()
		{
		}

		public DatabaseIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected DatabaseIdParameter(string identity) : base(identity)
		{
			this.Initialize(identity);
		}

		internal bool AllowLegacy
		{
			get
			{
				return this.allowLegacy;
			}
			set
			{
				this.allowLegacy = value;
				if (this.legacyParameter != null)
				{
					this.legacyParameter.AllowLegacy = value;
				}
			}
		}

		internal bool AllowInvalid { get; set; }

		public static DatabaseIdParameter Parse(string identity)
		{
			return new DatabaseIdParameter(identity);
		}

		public override string ToString()
		{
			if (base.InternalADObjectId == null && this.legacyParameter != null)
			{
				return this.legacyParameter.ToString();
			}
			return base.ToString();
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (!typeof(Database).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			new List<T>();
			notFoundReason = null;
			if (rootId != null)
			{
				Server server = (Server)((IConfigDataProvider)session).Read<Server>(rootId);
				if (server != null)
				{
					if (optionalData != null && optionalData.AdditionalFilter != null)
					{
						throw new NotSupportedException("Supplying Additional Filters and a RootId is not currently supported by this IdParameter.");
					}
					return server.GetDatabases<T>(this.AllowInvalid);
				}
			}
			IEnumerable<T> objects = base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			IEnumerable<T> enumerable = from tmpDb in objects
			where tmpDb.IsValid || this.AllowInvalid
			select tmpDb;
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(enumerable);
			if (!wrapper.HasElements() && this.legacyParameter != null)
			{
				wrapper = EnumerableWrapper<T>.GetWrapper(this.legacyParameter.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			}
			return wrapper;
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
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
			if (array.Length > 2)
			{
				this.legacyParameter = LegacyDatabaseIdParameter.Parse(identity);
			}
		}

		private LegacyDatabaseIdParameter legacyParameter;

		private bool allowLegacy;
	}
}
