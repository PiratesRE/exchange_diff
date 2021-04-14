using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoProvisionFailedException : LocalizedException
	{
		public AutoProvisionFailedException() : base(Strings.AutoProvisionFailedException)
		{
		}

		public AutoProvisionFailedException(Exception innerException) : base(Strings.AutoProvisionFailedException, innerException)
		{
		}

		protected AutoProvisionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
