using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorNonE12SourceAndTargetServersException : LocalizedException
	{
		public SendConnectorNonE12SourceAndTargetServersException() : base(Strings.SendConnectorNonE12SourceAndTargetServers)
		{
		}

		public SendConnectorNonE12SourceAndTargetServersException(Exception innerException) : base(Strings.SendConnectorNonE12SourceAndTargetServers, innerException)
		{
		}

		protected SendConnectorNonE12SourceAndTargetServersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
