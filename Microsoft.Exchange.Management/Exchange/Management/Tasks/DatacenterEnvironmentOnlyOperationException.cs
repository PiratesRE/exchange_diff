using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatacenterEnvironmentOnlyOperationException : LocalizedException
	{
		public DatacenterEnvironmentOnlyOperationException() : base(Strings.DatacenterEnvironmentOnlyOperationException)
		{
		}

		public DatacenterEnvironmentOnlyOperationException(Exception innerException) : base(Strings.DatacenterEnvironmentOnlyOperationException, innerException)
		{
		}

		protected DatacenterEnvironmentOnlyOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
