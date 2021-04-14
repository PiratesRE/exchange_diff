using System;
using System.Text;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationBatchId : ObjectId
	{
		internal MigrationBatchId(string batchName, Guid jobId)
		{
			if (jobId == Guid.Empty && string.IsNullOrEmpty(batchName))
			{
				throw new InvalidMigrationBatchIdException();
			}
			this.name = batchName;
			this.jobId = jobId;
		}

		internal MigrationBatchId(string batchName) : this(batchName, Guid.Empty)
		{
		}

		internal MigrationBatchId(Guid jobId) : this(jobId.ToString(), jobId)
		{
		}

		public string Id
		{
			get
			{
				if (!(this.JobId != Guid.Empty))
				{
					return this.Name;
				}
				return this.JobId.ToString();
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Guid JobId
		{
			get
			{
				return this.jobId;
			}
		}

		internal static MigrationBatchId Any
		{
			get
			{
				return new MigrationBatchId("90DA65B9-2154-4338-A690-602DF1FD5BAC", MigrationBatchId.AnyBatchGuid);
			}
		}

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.Name);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return this.JobId.ToString();
		}

		public override bool Equals(object obj)
		{
			MigrationBatchId migrationBatchId = obj as MigrationBatchId;
			return migrationBatchId != null && (string.Equals(this.Name, migrationBatchId.Name, StringComparison.OrdinalIgnoreCase) || this.JobId.Equals(migrationBatchId.JobId));
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public const string MigrationMailboxName = "Migration.8f3e7716-2011-43e4-96b1-aba62d229136";

		private const string AnyBatchId = "90DA65B9-2154-4338-A690-602DF1FD5BAC";

		private static readonly Guid AnyBatchGuid = Guid.Parse("90DA65B9-2154-4338-A690-602DF1FD5BAC");

		private readonly string name;

		private readonly Guid jobId;
	}
}
