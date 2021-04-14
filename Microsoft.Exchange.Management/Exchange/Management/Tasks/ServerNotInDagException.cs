using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerNotInDagException : LocalizedException
	{
		public ServerNotInDagException(string serverName) : base(Strings.ServerNotInDagError(serverName))
		{
			this.serverName = serverName;
		}

		public ServerNotInDagException(string serverName, Exception innerException) : base(Strings.ServerNotInDagError(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected ServerNotInDagException(SerializationInfo info, StreamingContext context) : base(info, context)
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
