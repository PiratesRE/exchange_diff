using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmServerNotFoundException : AmServerException
	{
		public AmServerNotFoundException(string server) : base(ServerStrings.AmServerNotFoundException(server))
		{
			this.server = server;
		}

		public AmServerNotFoundException(string server, Exception innerException) : base(ServerStrings.AmServerNotFoundException(server), innerException)
		{
			this.server = server;
		}

		protected AmServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
