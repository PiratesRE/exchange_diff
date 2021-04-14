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
	internal class MigrationEndpointDuplicatedException : MigrationPermanentException
	{
		public MigrationEndpointDuplicatedException(string endpointIdentity) : base(Strings.MigrationEndpointAlreadyExistsError(endpointIdentity))
		{
			this.endpointIdentity = endpointIdentity;
		}

		public MigrationEndpointDuplicatedException(string endpointIdentity, Exception innerException) : base(Strings.MigrationEndpointAlreadyExistsError(endpointIdentity), innerException)
		{
			this.endpointIdentity = endpointIdentity;
		}

		protected MigrationEndpointDuplicatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpointIdentity = (string)info.GetValue("endpointIdentity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpointIdentity", this.endpointIdentity);
		}

		public string EndpointIdentity
		{
			get
			{
				return this.endpointIdentity;
			}
		}

		private readonly string endpointIdentity;
	}
}
