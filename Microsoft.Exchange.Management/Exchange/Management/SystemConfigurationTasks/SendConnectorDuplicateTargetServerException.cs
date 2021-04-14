using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SendConnectorDuplicateTargetServerException : LocalizedException
	{
		public SendConnectorDuplicateTargetServerException(string server) : base(Strings.SendConnectorDuplicateTargetServerException(server))
		{
			this.server = server;
		}

		public SendConnectorDuplicateTargetServerException(string server, Exception innerException) : base(Strings.SendConnectorDuplicateTargetServerException(server), innerException)
		{
			this.server = server;
		}

		protected SendConnectorDuplicateTargetServerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
