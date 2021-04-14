using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SystemMailboxNotFoundException : LocalizedException
	{
		public SystemMailboxNotFoundException(string systemMailbox, string database) : base(Strings.messageSystemMailboxNotFoundException(systemMailbox, database))
		{
			this.systemMailbox = systemMailbox;
			this.database = database;
		}

		public SystemMailboxNotFoundException(string systemMailbox, string database, Exception innerException) : base(Strings.messageSystemMailboxNotFoundException(systemMailbox, database), innerException)
		{
			this.systemMailbox = systemMailbox;
			this.database = database;
		}

		protected SystemMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.systemMailbox = (string)info.GetValue("systemMailbox", typeof(string));
			this.database = (string)info.GetValue("database", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("systemMailbox", this.systemMailbox);
			info.AddValue("database", this.database);
		}

		public string SystemMailbox
		{
			get
			{
				return this.systemMailbox;
			}
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		private readonly string systemMailbox;

		private readonly string database;
	}
}
