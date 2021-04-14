using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetHubTransportServerOnAdamException : LocalizedException
	{
		public CannotSetHubTransportServerOnAdamException() : base(Strings.CannotSetHubTransportServerOnAdam)
		{
		}

		public CannotSetHubTransportServerOnAdamException(Exception innerException) : base(Strings.CannotSetHubTransportServerOnAdam, innerException)
		{
		}

		protected CannotSetHubTransportServerOnAdamException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
