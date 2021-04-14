using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerIsNotCasServerException : LocalizedException
	{
		public ServerIsNotCasServerException(string server) : base(Strings.ErrorServerIsNotCasServer(server))
		{
			this.server = server;
		}

		public ServerIsNotCasServerException(string server, Exception innerException) : base(Strings.ErrorServerIsNotCasServer(server), innerException)
		{
			this.server = server;
		}

		protected ServerIsNotCasServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string server;
	}
}
