using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[Serializable]
	public class MigrationReportIdParameter : IIdentityParameter
	{
		public MigrationReportIdParameter() : this(StoreObjectId.DummyId.ToString())
		{
		}

		public MigrationReportIdParameter(INamedIdentity namedIdentity)
		{
			if (namedIdentity == null)
			{
				throw new ArgumentNullException("namedIdentity");
			}
			this.MigrationReportId = new MigrationReportId(namedIdentity.Identity);
			this.RawIdentity = namedIdentity.DisplayName;
		}

		public MigrationReportIdParameter(MigrationReportId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationReportId = identity;
			this.RawIdentity = identity.ToString();
		}

		public MigrationReportIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationReportId = new MigrationReportId(identity);
			this.RawIdentity = identity;
		}

		public MigrationReportId MigrationReportId
		{
			get
			{
				return this.migrationReportId;
			}
			private set
			{
				this.migrationReportId = value;
			}
		}

		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
			private set
			{
				this.rawIdentity = value;
			}
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			IConfigurable[] array = session.Find<T>(null, this.MigrationReportId, false, null);
			for (int i = 0; i < array.Length; i++)
			{
				T instance = (T)((object)array[i]);
				yield return instance;
			}
			yield break;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = new LocalizedString?(Strings.MigrationReportNotFound);
			return this.GetObjects<T>(rootId, session);
		}

		public void Initialize(ObjectId objectId)
		{
			MigrationReportId migrationReportId = objectId as MigrationReportId;
			if (migrationReportId == null)
			{
				throw new ArgumentException("objectId");
			}
			this.MigrationReportId = migrationReportId;
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}

		private MigrationReportId migrationReportId;

		private string rawIdentity;
	}
}
