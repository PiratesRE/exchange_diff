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
	internal class MigrationMailboxNotFoundException : MigrationPermanentException
	{
		public MigrationMailboxNotFoundException() : base(Strings.ErrorMigrationMailboxMissingOrInvalid)
		{
		}

		public MigrationMailboxNotFoundException(Exception innerException) : base(Strings.ErrorMigrationMailboxMissingOrInvalid, innerException)
		{
		}

		protected MigrationMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
