using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Management.Migration
{
	[Serializable]
	public class MigrationBatchIdParameter : IIdentityParameter
	{
		public MigrationBatchIdParameter() : this(MigrationBatchId.Any)
		{
		}

		public MigrationBatchIdParameter(INamedIdentity namedIdentity)
		{
			if (namedIdentity == null)
			{
				throw new ArgumentNullException("namedIdentity");
			}
			this.MigrationBatchId = MigrationBatchIdParameter.MigrationBatchIdFromString(namedIdentity.Identity);
			this.RawIdentity = namedIdentity.DisplayName;
		}

		public MigrationBatchIdParameter(MigrationBatchId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationBatchId = identity;
			this.RawIdentity = this.MigrationBatchId.ToString();
		}

		public MigrationBatchIdParameter(MigrationBatch batch) : this(batch.Identity)
		{
		}

		public MigrationBatchIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationBatchId = MigrationBatchIdParameter.MigrationBatchIdFromString(identity);
			this.RawIdentity = identity;
		}

		public MigrationBatchIdParameter(Guid jobId) : this(new MigrationBatchId(jobId))
		{
		}

		public MigrationBatchId MigrationBatchId
		{
			get
			{
				return this.migrationBatchId;
			}
			private set
			{
				this.migrationBatchId = value;
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

		public static MigrationBatchIdParameter Parse(string identity)
		{
			return new MigrationBatchIdParameter(identity);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			IConfigurable[] array = session.Find<T>(null, this.MigrationBatchId, false, null);
			for (int i = 0; i < array.Length; i++)
			{
				T instance = (T)((object)array[i]);
				yield return instance;
			}
			yield break;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = new LocalizedString?(Strings.MigrationJobNotFound(this.RawIdentity));
			return this.GetObjects<T>(rootId, session);
		}

		public void Initialize(ObjectId objectId)
		{
			MigrationBatchId migrationBatchId = objectId as MigrationBatchId;
			if (migrationBatchId == null)
			{
				throw new ArgumentException("objectId");
			}
			this.MigrationBatchId = migrationBatchId;
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}

		private static MigrationBatchId MigrationBatchIdFromString(string identity)
		{
			Guid jobId;
			if (!GuidHelper.TryParseGuid(identity, out jobId))
			{
				return new MigrationBatchId(identity);
			}
			return new MigrationBatchId(jobId);
		}

		public const string MigrationMailboxName = "Migration.8f3e7716-2011-43e4-96b1-aba62d229136";

		private MigrationBatchId migrationBatchId;

		private string rawIdentity;
	}
}
