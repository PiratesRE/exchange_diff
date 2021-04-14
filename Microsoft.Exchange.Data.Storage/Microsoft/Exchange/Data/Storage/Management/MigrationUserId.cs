using System;
using System.Text;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationUserId : ObjectId
	{
		internal MigrationUserId(string userId, Guid jobItemGuid)
		{
			this.Id = userId;
			this.JobItemGuid = jobItemGuid;
		}

		public string Id { get; private set; }

		public Guid JobItemGuid { get; private set; }

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.Id);
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Id) && this.JobItemGuid != Guid.Empty)
			{
				return this.JobItemGuid.ToString();
			}
			return this.Id;
		}

		public override bool Equals(object obj)
		{
			MigrationUserId migrationUserId = obj as MigrationUserId;
			if (migrationUserId == null)
			{
				return false;
			}
			if (this.JobItemGuid == Guid.Empty && migrationUserId.JobItemGuid == Guid.Empty)
			{
				return string.Equals(this.Id, migrationUserId.Id, StringComparison.OrdinalIgnoreCase);
			}
			return this.JobItemGuid == migrationUserId.JobItemGuid;
		}

		public override int GetHashCode()
		{
			if (this.JobItemGuid != Guid.Empty)
			{
				return this.JobItemGuid.GetHashCode();
			}
			return this.Id.GetHashCode();
		}
	}
}
