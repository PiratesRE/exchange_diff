using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorRgcNotFoundException : LocalizedException
	{
		public SendConnectorRgcNotFoundException(string connectorDn) : base(Strings.SendConnectorRgcNotFound(connectorDn))
		{
			this.connectorDn = connectorDn;
		}

		public SendConnectorRgcNotFoundException(string connectorDn, Exception innerException) : base(Strings.SendConnectorRgcNotFound(connectorDn), innerException)
		{
			this.connectorDn = connectorDn;
		}

		protected SendConnectorRgcNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.connectorDn = (string)info.GetValue("connectorDn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("connectorDn", this.connectorDn);
		}

		public string ConnectorDn
		{
			get
			{
				return this.connectorDn;
			}
		}

		private readonly string connectorDn;
	}
}
