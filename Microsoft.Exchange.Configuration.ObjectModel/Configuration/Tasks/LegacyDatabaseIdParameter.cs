using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class LegacyDatabaseIdParameter : ServerBasedIdParameter
	{
		public LegacyDatabaseIdParameter(Database database) : base(database.Id)
		{
		}

		public LegacyDatabaseIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public LegacyDatabaseIdParameter()
		{
		}

		public LegacyDatabaseIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected LegacyDatabaseIdParameter(string identity) : base(identity)
		{
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.Mailbox;
			}
		}

		public static LegacyDatabaseIdParameter Parse(string identity)
		{
			return new LegacyDatabaseIdParameter(identity);
		}

		public override string ToString()
		{
			if (base.InternalADObjectId == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!string.IsNullOrEmpty(base.ServerName))
				{
					stringBuilder.Append(base.ServerName);
					stringBuilder.Append('\\');
				}
				if (!string.IsNullOrEmpty(this.storageGroupName))
				{
					stringBuilder.Append(this.storageGroupName);
					stringBuilder.Append('\\');
				}
				stringBuilder.Append(base.CommonName);
				return stringBuilder.ToString();
			}
			return base.ToString();
		}

		internal override void Initialize(ObjectId objectId)
		{
			base.Initialize(objectId);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			IEnumerable<T> enumerable = null;
			EnumerableWrapper<T> enumerableWrapper = null;
			notFoundReason = null;
			if (string.IsNullOrEmpty(this.storageGroupName))
			{
				enumerableWrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			}
			if (!string.IsNullOrEmpty(base.CommonName) && (enumerableWrapper == null || !enumerableWrapper.HasElements()))
			{
				string serverName = this.storageGroupName;
				ServerIdParameter serverIdParameter = base.ServerId;
				if (string.IsNullOrEmpty(this.storageGroupName))
				{
					serverName = base.ServerName;
					serverIdParameter = new ServerIdParameter();
				}
				ADObjectId[] matchingIdentities = serverIdParameter.GetMatchingIdentities((IConfigDataProvider)session);
				for (int i = 0; i < matchingIdentities.Length; i++)
				{
					if (ServerIdParameter.HasRole(matchingIdentities[i], this.RoleRestriction, (IConfigDataProvider)session) || (base.AllowLegacy && !ServerIdParameter.HasRole(matchingIdentities[i], ServerRole.All, (IConfigDataProvider)session)))
					{
						if (string.IsNullOrEmpty(this.storageGroupName))
						{
							rootId = matchingIdentities[i].GetChildId("InformationStore").GetChildId(serverName);
							enumerable = base.PerformPrimarySearch<T>(base.CreateWildcardOrEqualFilter(ADObjectSchema.Name, base.CommonName), rootId, session, true, optionalData);
						}
						else
						{
							List<T> list = new List<T>();
							IEnumerable<StorageGroup> enumerable2 = base.PerformSearch<StorageGroup>(base.CreateWildcardOrEqualFilter(ADObjectSchema.Name, this.storageGroupName), matchingIdentities[i], session, true);
							foreach (StorageGroup storageGroup in enumerable2)
							{
								enumerable = base.PerformPrimarySearch<T>(base.CreateWildcardOrEqualFilter(ADObjectSchema.Name, base.CommonName), storageGroup.Id, session, true, optionalData);
								list.AddRange(enumerable);
							}
							enumerable = list;
						}
					}
				}
			}
			else
			{
				enumerable = enumerableWrapper;
			}
			if (enumerable == null)
			{
				enumerable = new List<T>();
			}
			return enumerable;
		}

		protected override void Initialize(string identity)
		{
			if (base.InternalADObjectId != null && base.InternalADObjectId.Rdn != null)
			{
				return;
			}
			string[] array = identity.Split(new char[]
			{
				'\\'
			});
			if (array.Length > 3)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
			if (array.Length == 3)
			{
				string identity2 = array[0] + '\\' + array[1];
				base.Initialize(identity2);
				this.storageGroupName = base.CommonName;
				base.CommonName = array[2];
			}
			else
			{
				base.Initialize(identity);
			}
			if (!string.IsNullOrEmpty(base.ServerName) && !string.IsNullOrEmpty(this.storageGroupName) && base.ServerId == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
		}

		private string storageGroupName;
	}
}
