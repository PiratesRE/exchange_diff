using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorUndefinedServerRgException : LocalizedException
	{
		public SendConnectorUndefinedServerRgException(string serverName) : base(Strings.SendConnectorUndefinedServerRg(serverName))
		{
			this.serverName = serverName;
		}

		public SendConnectorUndefinedServerRgException(string serverName, Exception innerException) : base(Strings.SendConnectorUndefinedServerRg(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected SendConnectorUndefinedServerRgException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
