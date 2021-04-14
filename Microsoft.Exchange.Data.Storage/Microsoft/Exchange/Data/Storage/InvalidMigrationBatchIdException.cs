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
	internal class InvalidMigrationBatchIdException : MigrationPermanentException
	{
		public InvalidMigrationBatchIdException() : base(ServerStrings.InvalidMigrationBatchId)
		{
		}

		public InvalidMigrationBatchIdException(Exception innerException) : base(ServerStrings.InvalidMigrationBatchId, innerException)
		{
		}

		protected InvalidMigrationBatchIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
