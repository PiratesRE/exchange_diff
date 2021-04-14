using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class MdbChangedEntry
	{
		internal MdbChangedEntry(MdbChangedType changedType, MdbInfo mailboxDatabaseInfo)
		{
			this.changedType = changedType;
			this.mailboxDatabaseInfo = mailboxDatabaseInfo;
		}

		internal MdbChangedType ChangedType
		{
			get
			{
				return this.changedType;
			}
		}

		internal MdbInfo MailboxDatabaseInfo
		{
			get
			{
				return this.mailboxDatabaseInfo;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} has changed to {1}.", this.MailboxDatabaseInfo, this.ChangedType);
		}

		private readonly MdbChangedType changedType;

		private readonly MdbInfo mailboxDatabaseInfo;
	}
}
