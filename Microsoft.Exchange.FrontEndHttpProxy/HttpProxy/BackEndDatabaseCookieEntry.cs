using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.HttpProxy
{
	internal class BackEndDatabaseCookieEntry : BackEndCookieEntryBase
	{
		public BackEndDatabaseCookieEntry(Guid database, string domain, ExDateTime expiryTime) : base(BackEndCookieEntryType.Database, expiryTime)
		{
			this.Database = database;
			this.Domain = domain;
		}

		public BackEndDatabaseCookieEntry(Guid database, string domain) : this(database, domain, ExDateTime.UtcNow + BackEndCookieEntryBase.LongLivedBackEndServerCookieLifeTime)
		{
		}

		public Guid Database { get; private set; }

		public string Domain { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				BackEndCookieEntryBase.ConvertBackEndCookieEntryTypeToString(base.EntryType),
				'~',
				this.Database.ToString(),
				'~',
				this.Domain,
				'~',
				base.ExpiryTime.ToString("s")
			});
		}
	}
}
