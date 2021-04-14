using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerConfigurationException : LocalizedException
	{
		public ServerConfigurationException(string serverName, string errorMessage) : base(Strings.ServerConfigurationException(serverName, errorMessage))
		{
			this.serverName = serverName;
			this.errorMessage = errorMessage;
		}

		public ServerConfigurationException(string serverName, string errorMessage, Exception innerException) : base(Strings.ServerConfigurationException(serverName, errorMessage), innerException)
		{
			this.serverName = serverName;
			this.errorMessage = errorMessage;
		}

		protected ServerConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string serverName;

		private readonly string errorMessage;
	}
}
