using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UpdateDbcCatalogOnlyAndDatabaseOnlyAreMutuallyExclusiveException : LocalizedException
	{
		public UpdateDbcCatalogOnlyAndDatabaseOnlyAreMutuallyExclusiveException() : base(Strings.UpdateDbcCatalogOnlyAndDatabaseOnlyAreMutuallyExclusiveException)
		{
		}

		public UpdateDbcCatalogOnlyAndDatabaseOnlyAreMutuallyExclusiveException(Exception innerException) : base(Strings.UpdateDbcCatalogOnlyAndDatabaseOnlyAreMutuallyExclusiveException, innerException)
		{
		}

		protected UpdateDbcCatalogOnlyAndDatabaseOnlyAreMutuallyExclusiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
