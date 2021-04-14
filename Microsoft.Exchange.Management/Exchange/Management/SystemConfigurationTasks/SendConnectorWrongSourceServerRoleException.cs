using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorWrongSourceServerRoleException : LocalizedException
	{
		public SendConnectorWrongSourceServerRoleException(string serverName) : base(Strings.SendConnectorWrongSourceServerRole(serverName))
		{
			this.serverName = serverName;
		}

		public SendConnectorWrongSourceServerRoleException(string serverName, Exception innerException) : base(Strings.SendConnectorWrongSourceServerRole(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected SendConnectorWrongSourceServerRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
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
