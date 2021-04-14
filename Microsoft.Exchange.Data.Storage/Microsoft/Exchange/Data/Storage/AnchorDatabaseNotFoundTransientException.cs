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
	internal class AnchorDatabaseNotFoundTransientException : MigrationTransientException
	{
		public AnchorDatabaseNotFoundTransientException(string mdbGuid) : base(ServerStrings.AnchorDatabaseNotFound(mdbGuid))
		{
			this.mdbGuid = mdbGuid;
		}

		public AnchorDatabaseNotFoundTransientException(string mdbGuid, Exception innerException) : base(ServerStrings.AnchorDatabaseNotFound(mdbGuid), innerException)
		{
			this.mdbGuid = mdbGuid;
		}

		protected AnchorDatabaseNotFoundTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbGuid = (string)info.GetValue("mdbGuid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbGuid", this.mdbGuid);
		}

		public string MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		private readonly string mdbGuid;
	}
}
