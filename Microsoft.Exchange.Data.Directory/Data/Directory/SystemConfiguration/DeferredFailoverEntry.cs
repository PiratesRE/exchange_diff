using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DeferredFailoverEntry
	{
		public DeferredFailoverEntry(ADObjectId server, DateTime recoveryDueAt)
		{
			this.m_server = server;
			this.m_recoveryDueAt = recoveryDueAt;
		}

		public ADObjectId Server
		{
			get
			{
				return this.m_server;
			}
			internal set
			{
				this.m_server = value;
			}
		}

		public DateTime RecoveryDueAt
		{
			get
			{
				return this.m_recoveryDueAt;
			}
			internal set
			{
				this.m_recoveryDueAt = value;
			}
		}

		public override string ToString()
		{
			return string.Format(DirectoryStrings.DeferredFailoverEntryString, this.Server.Name, this.RecoveryDueAt);
		}

		private ADObjectId m_server;

		private DateTime m_recoveryDueAt;
	}
}
