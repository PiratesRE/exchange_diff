using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmServerNotFoundToVerifyRpcVersion : AmServerTransientException
	{
		public AmServerNotFoundToVerifyRpcVersion(string serverName) : base(ServerStrings.AmServerNotFoundToVerifyRpcVersion(serverName))
		{
			this.serverName = serverName;
		}

		public AmServerNotFoundToVerifyRpcVersion(string serverName, Exception innerException) : base(ServerStrings.AmServerNotFoundToVerifyRpcVersion(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected AmServerNotFoundToVerifyRpcVersion(SerializationInfo info, StreamingContext context) : base(info, context)
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
