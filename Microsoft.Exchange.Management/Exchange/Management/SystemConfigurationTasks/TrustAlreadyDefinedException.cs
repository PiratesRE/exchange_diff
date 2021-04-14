using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrustAlreadyDefinedException : FederationException
	{
		public TrustAlreadyDefinedException() : base(Strings.ErrorTrustAlreadyDefined)
		{
		}

		public TrustAlreadyDefinedException(Exception innerException) : base(Strings.ErrorTrustAlreadyDefined, innerException)
		{
		}

		protected TrustAlreadyDefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
