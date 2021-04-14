using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecoveryMailboxDatabaseNotMonitoredException : LocalizedException
	{
		public RecoveryMailboxDatabaseNotMonitoredException(string databaseId) : base(Strings.RecoveryMailboxDatabaseNotMonitored(databaseId))
		{
			this.databaseId = databaseId;
		}

		public RecoveryMailboxDatabaseNotMonitoredException(string databaseId, Exception innerException) : base(Strings.RecoveryMailboxDatabaseNotMonitored(databaseId), innerException)
		{
			this.databaseId = databaseId;
		}

		protected RecoveryMailboxDatabaseNotMonitoredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseId = (string)info.GetValue("databaseId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseId", this.databaseId);
		}

		public string DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		private readonly string databaseId;
	}
}
