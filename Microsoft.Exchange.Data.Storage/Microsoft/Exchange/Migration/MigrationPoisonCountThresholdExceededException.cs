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
	internal class MigrationPoisonCountThresholdExceededException : MigrationPermanentException
	{
		public MigrationPoisonCountThresholdExceededException() : base(Strings.UnknownMigrationBatchError)
		{
		}

		public MigrationPoisonCountThresholdExceededException(Exception innerException) : base(Strings.UnknownMigrationBatchError, innerException)
		{
		}

		protected MigrationPoisonCountThresholdExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
