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
	internal class NspiNotSupportedForEndpointTypeException : MigrationPermanentException
	{
		public NspiNotSupportedForEndpointTypeException(MigrationType type) : base(Strings.ErrorNspiNotSupportedForEndpointType(type))
		{
			this.type = type;
		}

		public NspiNotSupportedForEndpointTypeException(MigrationType type, Exception innerException) : base(Strings.ErrorNspiNotSupportedForEndpointType(type), innerException)
		{
			this.type = type;
		}

		protected NspiNotSupportedForEndpointTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
