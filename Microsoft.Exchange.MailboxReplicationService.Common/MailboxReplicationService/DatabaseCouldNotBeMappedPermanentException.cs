using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseCouldNotBeMappedPermanentException : MailboxReplicationPermanentException
	{
		public DatabaseCouldNotBeMappedPermanentException(string databaseName) : base(MrsStrings.DatabaseCouldNotBeMapped(databaseName))
		{
			this.databaseName = databaseName;
		}

		public DatabaseCouldNotBeMappedPermanentException(string databaseName, Exception innerException) : base(MrsStrings.DatabaseCouldNotBeMapped(databaseName), innerException)
		{
			this.databaseName = databaseName;
		}

		protected DatabaseCouldNotBeMappedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		private readonly string databaseName;
	}
}
