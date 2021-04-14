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
	internal class AnchorServerNotFoundTransientException : MigrationTransientException
	{
		public AnchorServerNotFoundTransientException(string mdbGuid) : base(ServerStrings.AnchorServerNotFound(mdbGuid))
		{
			this.mdbGuid = mdbGuid;
		}

		public AnchorServerNotFoundTransientException(string mdbGuid, Exception innerException) : base(ServerStrings.AnchorServerNotFound(mdbGuid), innerException)
		{
			this.mdbGuid = mdbGuid;
		}

		protected AnchorServerNotFoundTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
