using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationMailboxDatabaseInfoNotAvailableException : MigrationTransientException
	{
		public MigrationMailboxDatabaseInfoNotAvailableException(string mbxid) : base(ServerStrings.MigrationMailboxDatabaseInfoNotAvailable(mbxid))
		{
			this.mbxid = mbxid;
		}

		public MigrationMailboxDatabaseInfoNotAvailableException(string mbxid, Exception innerException) : base(ServerStrings.MigrationMailboxDatabaseInfoNotAvailable(mbxid), innerException)
		{
			this.mbxid = mbxid;
		}

		protected MigrationMailboxDatabaseInfoNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbxid = (string)info.GetValue("mbxid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbxid", this.mbxid);
		}

		public string Mbxid
		{
			get
			{
				return this.mbxid;
			}
		}

		private readonly string mbxid;
	}
}
