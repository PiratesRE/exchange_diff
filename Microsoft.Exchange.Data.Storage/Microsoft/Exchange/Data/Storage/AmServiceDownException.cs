using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmServiceDownException : AmServerException
	{
		public AmServiceDownException(string serverName) : base(ServerStrings.AmServiceDownException(serverName))
		{
			this.serverName = serverName;
		}

		public AmServiceDownException(string serverName, Exception innerException) : base(ServerStrings.AmServiceDownException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected AmServiceDownException(SerializationInfo info, StreamingContext context) : base(info, context)
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
