using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConnectorMappingConflictException : LocalizedException
	{
		public ConnectorMappingConflictException(string receiveConnectorId) : base(Strings.ReceiveConnectorMappingConflict(receiveConnectorId))
		{
			this.receiveConnectorId = receiveConnectorId;
		}

		public ConnectorMappingConflictException(string receiveConnectorId, Exception innerException) : base(Strings.ReceiveConnectorMappingConflict(receiveConnectorId), innerException)
		{
			this.receiveConnectorId = receiveConnectorId;
		}

		protected ConnectorMappingConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.receiveConnectorId = (string)info.GetValue("receiveConnectorId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("receiveConnectorId", this.receiveConnectorId);
		}

		public string ReceiveConnectorId
		{
			get
			{
				return this.receiveConnectorId;
			}
		}

		private readonly string receiveConnectorId;
	}
}
