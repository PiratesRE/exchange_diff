using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArchiveDatabaseDifferentFromRawValuePermanentException : MailboxReplicationPermanentException
	{
		public ArchiveDatabaseDifferentFromRawValuePermanentException(string archiveDb, string archiveDbRaw) : base(Strings.ArchiveDatabaseDifferentFromRawValueError(archiveDb, archiveDbRaw))
		{
			this.archiveDb = archiveDb;
			this.archiveDbRaw = archiveDbRaw;
		}

		public ArchiveDatabaseDifferentFromRawValuePermanentException(string archiveDb, string archiveDbRaw, Exception innerException) : base(Strings.ArchiveDatabaseDifferentFromRawValueError(archiveDb, archiveDbRaw), innerException)
		{
			this.archiveDb = archiveDb;
			this.archiveDbRaw = archiveDbRaw;
		}

		protected ArchiveDatabaseDifferentFromRawValuePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.archiveDb = (string)info.GetValue("archiveDb", typeof(string));
			this.archiveDbRaw = (string)info.GetValue("archiveDbRaw", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("archiveDb", this.archiveDb);
			info.AddValue("archiveDbRaw", this.archiveDbRaw);
		}

		public string ArchiveDb
		{
			get
			{
				return this.archiveDb;
			}
		}

		public string ArchiveDbRaw
		{
			get
			{
				return this.archiveDbRaw;
			}
		}

		private readonly string archiveDb;

		private readonly string archiveDbRaw;
	}
}
