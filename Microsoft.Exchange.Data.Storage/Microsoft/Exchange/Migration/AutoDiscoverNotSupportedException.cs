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
	internal class AutoDiscoverNotSupportedException : MigrationPermanentException
	{
		public AutoDiscoverNotSupportedException(MigrationType type) : base(Strings.ErrorAutoDiscoveryNotSupported(type))
		{
			this.type = type;
		}

		public AutoDiscoverNotSupportedException(MigrationType type, Exception innerException) : base(Strings.ErrorAutoDiscoveryNotSupported(type), innerException)
		{
			this.type = type;
		}

		protected AutoDiscoverNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = (MigrationType)info.GetValue("type", typeof(MigrationType));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.type);
		}

		public MigrationType Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly MigrationType type;
	}
}
