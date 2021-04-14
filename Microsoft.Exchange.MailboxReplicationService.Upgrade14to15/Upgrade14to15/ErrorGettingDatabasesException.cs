using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorGettingDatabasesException : MigrationTransientException
	{
		public ErrorGettingDatabasesException(string server) : base(UpgradeHandlerStrings.ErrorGettingDatabases(server))
		{
			this.server = server;
		}

		public ErrorGettingDatabasesException(string server, Exception innerException) : base(UpgradeHandlerStrings.ErrorGettingDatabases(server), innerException)
		{
			this.server = server;
		}

		protected ErrorGettingDatabasesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string server;
	}
}
