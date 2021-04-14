using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationJobNotFoundException : MigrationPermanentException
	{
		public MigrationJobNotFoundException(Guid identity) : base(Strings.ErrorMigrationJobNotFound(identity))
		{
			this.identity = identity;
		}

		public MigrationJobNotFoundException(Guid identity, Exception innerException) : base(Strings.ErrorMigrationJobNotFound(identity), innerException)
		{
			this.identity = identity;
		}

		protected MigrationJobNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (Guid)info.GetValue("identity", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public Guid Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly Guid identity;
	}
}
