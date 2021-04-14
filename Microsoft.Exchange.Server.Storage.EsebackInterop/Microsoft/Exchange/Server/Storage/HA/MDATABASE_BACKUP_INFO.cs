using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	public struct MDATABASE_BACKUP_INFO
	{
		public string Name
		{
			get
			{
				return this.<backing_store>Name;
			}
			set
			{
				this.<backing_store>Name = value;
			}
		}

		public string Path
		{
			get
			{
				return this.<backing_store>Path;
			}
			set
			{
				this.<backing_store>Path = value;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.<backing_store>Guid;
			}
			set
			{
				this.<backing_store>Guid = value;
			}
		}

		public DatabaseBackupInfoFlags Flags
		{
			get
			{
				return this.<backing_store>Flags;
			}
			set
			{
				this.<backing_store>Flags = value;
			}
		}

		private string <backing_store>Name;

		private string <backing_store>Path;

		private Guid <backing_store>Guid;

		private DatabaseBackupInfoFlags <backing_store>Flags;
	}
}
