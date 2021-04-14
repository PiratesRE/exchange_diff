using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RegKeyNotExistMigrationException : MailboxReplicationTransientException
	{
		public RegKeyNotExistMigrationException(string keyPath) : base(MrsStrings.ErrorRegKeyNotExist(keyPath))
		{
			this.keyPath = keyPath;
		}

		public RegKeyNotExistMigrationException(string keyPath, Exception innerException) : base(MrsStrings.ErrorRegKeyNotExist(keyPath), innerException)
		{
			this.keyPath = keyPath;
		}

		protected RegKeyNotExistMigrationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyPath = (string)info.GetValue("keyPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyPath", this.keyPath);
		}

		public string KeyPath
		{
			get
			{
				return this.keyPath;
			}
		}

		private readonly string keyPath;
	}
}
