using System;
using System.Text;

namespace Microsoft.Exchange.Data.Storage.Management.Migration
{
	[Serializable]
	public sealed class MigrationEndpointId : ObjectId
	{
		public MigrationEndpointId(string id, Guid guid)
		{
			this.Id = id;
			this.Guid = guid;
		}

		public string Id { get; private set; }

		public Guid Guid { get; private set; }

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.Id);
		}

		public override string ToString()
		{
			return this.Id ?? this.Guid.ToString();
		}

		public override bool Equals(object obj)
		{
			MigrationEndpointId migrationEndpointId = obj as MigrationEndpointId;
			return migrationEndpointId != null && string.Equals(this.Id, migrationEndpointId.Id, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public QueryFilter GetFilter()
		{
			if (this.Guid == MigrationEndpointId.Any.Guid)
			{
				return new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.MS-Exchange.MigrationEndpoint");
			}
			if (this.Guid != Guid.Empty)
			{
				return new ComparisonFilter(ComparisonOperator.Equal, MigrationEndpointMessageSchema.MigrationEndpointGuid, this.Guid);
			}
			return new TextFilter(MigrationEndpointMessageSchema.MigrationEndpointName, this.Id, MatchOptions.FullString, MatchFlags.IgnoreCase);
		}

		public static readonly MigrationEndpointId Any = new MigrationEndpointId("13CA1A28-C866-4CF7-9D81-22B5C0E03AD2", new Guid("13CA1A28-C866-4CF7-9D81-22B5C0E03AD2"));
	}
}
