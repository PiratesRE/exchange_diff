using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetIISVersionException : DataSourceOperationException
	{
		public GetIISVersionException(string server) : base(Strings.GetIISVersionException(server))
		{
			this.server = server;
		}

		public GetIISVersionException(string server, Exception innerException) : base(Strings.GetIISVersionException(server), innerException)
		{
			this.server = server;
		}

		protected GetIISVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
