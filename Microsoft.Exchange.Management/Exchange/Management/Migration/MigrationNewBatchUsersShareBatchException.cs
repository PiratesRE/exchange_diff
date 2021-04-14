using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationNewBatchUsersShareBatchException : LocalizedException
	{
		public MigrationNewBatchUsersShareBatchException() : base(Strings.MigrationNewBatchUsersShareBatch)
		{
		}

		public MigrationNewBatchUsersShareBatchException(Exception innerException) : base(Strings.MigrationNewBatchUsersShareBatch, innerException)
		{
		}

		protected MigrationNewBatchUsersShareBatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
