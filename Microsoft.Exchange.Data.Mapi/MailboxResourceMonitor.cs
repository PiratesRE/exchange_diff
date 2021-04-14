using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class MailboxResourceMonitor : MailboxResourceMonitorEntry
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxResourceMonitor.schema;
			}
		}

		public string DigestCategory
		{
			get
			{
				return (string)this[MailboxResourceMonitorSchema.DigestCategory];
			}
		}

		public uint? SampleId
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.SampleId];
			}
		}

		public DateTime? SampleTime
		{
			get
			{
				return (DateTime?)this[MailboxResourceMonitorSchema.SampleTime];
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[MailboxResourceMonitorSchema.DisplayName];
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)this[MailboxResourceMonitorSchema.MailboxGuid];
			}
		}

		public uint? TimeInServer
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.TimeInServer];
			}
		}

		public uint? TimeInCPU
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.TimeInCPU];
			}
		}

		public uint? ROPCount
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.ROPCount];
			}
		}

		public uint? PageRead
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.PageRead];
			}
		}

		public uint? PagePreread
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.PagePreread];
			}
		}

		public uint? LogRecordCount
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.LogRecordCount];
			}
		}

		public uint? LogRecordBytes
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.LogRecordBytes];
			}
		}

		public uint? LdapReads
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.LdapReads];
			}
		}

		public uint? LdapSearches
		{
			get
			{
				return (uint?)this[MailboxResourceMonitorSchema.LdapSearches];
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			internal set
			{
				this.serverName = value;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
			internal set
			{
				this.databaseName = value;
			}
		}

		public bool IsDatabaseCopyActive
		{
			get
			{
				return this.isDatabaseCopyActive;
			}
			internal set
			{
				this.isDatabaseCopyActive = value;
			}
		}

		public bool? IsQuarantined
		{
			get
			{
				return (bool?)this[MailboxResourceMonitorSchema.IsQuarantined];
			}
		}

		private string serverName;

		private string databaseName;

		private bool isDatabaseCopyActive;

		private static MapiObjectSchema schema = ObjectSchema.GetInstance<MailboxResourceMonitorSchema>();
	}
}
