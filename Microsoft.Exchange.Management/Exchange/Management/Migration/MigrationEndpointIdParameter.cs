using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[Serializable]
	public class MigrationEndpointIdParameter : IIdentityParameter
	{
		public MigrationEndpointIdParameter() : this(MigrationEndpointId.Any)
		{
		}

		public MigrationEndpointIdParameter(string name) : this(new MigrationEndpointId(name, Guid.Empty))
		{
		}

		public MigrationEndpointIdParameter(INamedIdentity namedId) : this(new MigrationEndpointId(namedId.Identity, Guid.Empty))
		{
			this.RawIdentity = namedId.DisplayName;
		}

		public MigrationEndpointIdParameter(MigrationEndpointId id)
		{
			this.MigrationEndpointId = id;
			this.RawIdentity = id.ToString();
		}

		public MigrationEndpointIdParameter(Guid guid) : this(new MigrationEndpointId(string.Empty, guid))
		{
		}

		public MigrationEndpointIdParameter(MigrationEndpoint connector) : this(connector.Identity)
		{
		}

		public string RawIdentity { get; private set; }

		public MigrationEndpointId MigrationEndpointId { get; private set; }

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			IConfigurable[] array = session.Find<T>(null, this.MigrationEndpointId, false, null);
			for (int i = 0; i < array.Length; i++)
			{
				T entry = (T)((object)array[i]);
				yield return entry;
			}
			yield break;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = new LocalizedString?(Strings.ErrorCouldNotFindMigrationEndpoint);
			return this.GetObjects<T>(rootId, session);
		}

		public void Initialize(ObjectId objectId)
		{
			MigrationEndpointId migrationEndpointId = objectId as MigrationEndpointId;
			if (migrationEndpointId == null)
			{
				throw new ArgumentException("Only MigrationEndpointId is supported.", "objectId");
			}
			this.MigrationEndpointId = migrationEndpointId;
			this.RawIdentity = this.MigrationEndpointId.ToString();
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}
	}
}
