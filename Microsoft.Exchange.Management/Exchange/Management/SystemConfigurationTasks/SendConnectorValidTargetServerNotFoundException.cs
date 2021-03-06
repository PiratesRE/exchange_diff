using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorValidTargetServerNotFoundException : LocalizedException
	{
		public SendConnectorValidTargetServerNotFoundException() : base(Strings.SendConnectorValidTargetServerNotFound)
		{
		}

		public SendConnectorValidTargetServerNotFoundException(Exception innerException) : base(Strings.SendConnectorValidTargetServerNotFound, innerException)
		{
		}

		protected SendConnectorValidTargetServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
