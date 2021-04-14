using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorSourceAndTargetServersInOneRgException : LocalizedException
	{
		public SendConnectorSourceAndTargetServersInOneRgException(string routingGroupName) : base(Strings.SendConnectorSourceAndTargetServersInOneRg(routingGroupName))
		{
			this.routingGroupName = routingGroupName;
		}

		public SendConnectorSourceAndTargetServersInOneRgException(string routingGroupName, Exception innerException) : base(Strings.SendConnectorSourceAndTargetServersInOneRg(routingGroupName), innerException)
		{
			this.routingGroupName = routingGroupName;
		}

		protected SendConnectorSourceAndTargetServersInOneRgException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.routingGroupName = (string)info.GetValue("routingGroupName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("routingGroupName", this.routingGroupName);
		}

		public string RoutingGroupName
		{
			get
			{
				return this.routingGroupName;
			}
		}

		private readonly string routingGroupName;
	}
}
