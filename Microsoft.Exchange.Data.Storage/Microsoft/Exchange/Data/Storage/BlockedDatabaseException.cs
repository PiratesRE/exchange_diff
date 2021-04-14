using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BlockedDatabaseException : StorageTransientException
	{
		public BlockedDatabaseException(Guid resourceIdentity) : base(ServerStrings.idResourceQuarantinedDueToBlackHole(resourceIdentity))
		{
			this.resourceIdentity = resourceIdentity;
		}

		public BlockedDatabaseException(Guid resourceIdentity, Exception innerException) : base(ServerStrings.idResourceQuarantinedDueToBlackHole(resourceIdentity), innerException)
		{
			this.resourceIdentity = resourceIdentity;
		}

		protected BlockedDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resourceIdentity = (Guid)info.GetValue("resourceIdentity", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resourceIdentity", this.resourceIdentity);
		}

		public Guid ResourceIdentity
		{
			get
			{
				return this.resourceIdentity;
			}
		}

		private readonly Guid resourceIdentity;
	}
}
