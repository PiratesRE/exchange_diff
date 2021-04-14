using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationReportId : ObjectId
	{
		internal MigrationReportId(string reportId)
		{
			reportId = reportId.Replace('-', '+').Replace('_', '/');
			this.Id = StoreObjectId.Deserialize(reportId);
		}

		internal MigrationReportId(StoreObjectId reportId)
		{
			this.Id = reportId;
		}

		internal StoreObjectId Id { get; private set; }

		public override byte[] GetBytes()
		{
			return this.Id.GetBytes();
		}

		public override string ToString()
		{
			string text = this.Id.ToBase64String();
			return text.Replace('+', '-').Replace('/', '_');
		}

		public override bool Equals(object obj)
		{
			MigrationReportId migrationReportId = obj as MigrationReportId;
			return migrationReportId != null && this.Id.Equals(migrationReportId.Id);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}
	}
}
