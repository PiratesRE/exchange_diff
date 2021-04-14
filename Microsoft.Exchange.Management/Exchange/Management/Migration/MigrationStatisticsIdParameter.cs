using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[Serializable]
	public class MigrationStatisticsIdParameter : IIdentityParameter
	{
		public MigrationStatisticsIdParameter()
		{
		}

		public MigrationStatisticsIdParameter(INamedIdentity namedIdentity)
		{
			if (namedIdentity == null)
			{
				throw new ArgumentNullException("namedIdentity");
			}
			this.OrganizationIdentifier = new OrganizationIdParameter(namedIdentity.Identity);
			this.RawIdentity = namedIdentity.DisplayName;
		}

		public MigrationStatisticsIdParameter(MigrationStatisticsId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.Id = identity;
			this.RawIdentity = identity.ToString();
		}

		public MigrationStatisticsIdParameter(MigrationStatistics statistics) : this(statistics.Identity)
		{
		}

		public MigrationStatisticsIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.OrganizationIdentifier = new OrganizationIdParameter(identity);
			this.RawIdentity = identity;
		}

		public MigrationStatisticsId Id { get; internal set; }

		public OrganizationIdParameter OrganizationIdentifier { get; private set; }

		public string RawIdentity { get; private set; }

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			if (this.Id == null)
			{
				throw new ArgumentNullException("this.Id");
			}
			IConfigurable[] array = session.Find<T>(null, this.Id, false, null);
			for (int i = 0; i < array.Length; i++)
			{
				T instance = (T)((object)array[i]);
				yield return instance;
			}
			yield break;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = new LocalizedString?(Strings.MigrationNotFound(this.RawIdentity));
			return this.GetObjects<T>(rootId, session);
		}

		public void Initialize(ObjectId objectId)
		{
			MigrationStatisticsId migrationStatisticsId = objectId as MigrationStatisticsId;
			if (migrationStatisticsId == null)
			{
				throw new ArgumentException("objectId");
			}
			this.Id = migrationStatisticsId;
			this.RawIdentity = migrationStatisticsId.ToString();
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}
	}
}
