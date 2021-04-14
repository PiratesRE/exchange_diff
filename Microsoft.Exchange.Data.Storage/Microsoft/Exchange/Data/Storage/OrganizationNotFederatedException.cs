using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OrganizationNotFederatedException : StoragePermanentException
	{
		public OrganizationNotFederatedException() : base(ServerStrings.OrganizationNotFederatedException)
		{
		}

		public OrganizationNotFederatedException(Exception innerException) : base(ServerStrings.OrganizationNotFederatedException, innerException)
		{
		}

		protected OrganizationNotFederatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
