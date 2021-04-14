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
	internal class NoEndpointSupportForMigrationTypeException : MigrationPermanentException
	{
		public NoEndpointSupportForMigrationTypeException(MigrationType migrationType) : base(Strings.ErrorNoEndpointSupportForMigrationType(migrationType))
		{
			this.migrationType = migrationType;
		}

		public NoEndpointSupportForMigrationTypeException(MigrationType migrationType, Exception innerException) : base(Strings.ErrorNoEndpointSupportForMigrationType(migrationType), innerException)
		{
			this.migrationType = migrationType;
		}

		protected NoEndpointSupportForMigrationTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.migrationType = (MigrationType)info.GetValue("migrationType", typeof(MigrationType));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("migrationType", this.migrationType);
		}

		public MigrationType MigrationType
		{
			get
			{
				return this.migrationType;
			}
		}

		private readonly MigrationType migrationType;
	}
}
