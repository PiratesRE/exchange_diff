using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MailboxCrossSiteFailoverException : ConnectionFailedPermanentException
	{
		public MailboxCrossSiteFailoverException(LocalizedString message, DatabaseLocationInfo dbLocationInfo) : base(message)
		{
			this.dbLocationInfo = dbLocationInfo;
		}

		public MailboxCrossSiteFailoverException(LocalizedString message, Exception innerException, DatabaseLocationInfo dbLocationInfo) : base(message, innerException)
		{
			this.dbLocationInfo = dbLocationInfo;
		}

		protected MailboxCrossSiteFailoverException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbLocationInfo = (DatabaseLocationInfo)info.GetValue("dbLocationInfo", typeof(DatabaseLocationInfo));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbLocationInfo", this.dbLocationInfo);
		}

		public DatabaseLocationInfo DatabaseLocationInfo
		{
			get
			{
				return this.dbLocationInfo;
			}
		}

		private const string DbLocationInfoLabel = "dbLocationInfo";

		private DatabaseLocationInfo dbLocationInfo;
	}
}
