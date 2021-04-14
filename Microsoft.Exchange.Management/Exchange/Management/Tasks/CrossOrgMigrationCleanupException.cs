using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CrossOrgMigrationCleanupException : LocalizedException
	{
		public CrossOrgMigrationCleanupException(LocalizedString message) : base(message)
		{
		}

		public CrossOrgMigrationCleanupException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CrossOrgMigrationCleanupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
