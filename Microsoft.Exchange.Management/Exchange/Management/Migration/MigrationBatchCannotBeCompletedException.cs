using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationBatchCannotBeCompletedException : LocalizedException
	{
		public MigrationBatchCannotBeCompletedException(LocalizedString message) : base(message)
		{
		}

		public MigrationBatchCannotBeCompletedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MigrationBatchCannotBeCompletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
