using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OperationOnOldServerException : LocalizedException
	{
		public OperationOnOldServerException(string serverName) : base(Strings.ErrorOperationOnOldServer(serverName))
		{
			this.serverName = serverName;
		}

		public OperationOnOldServerException(string serverName, Exception innerException) : base(Strings.ErrorOperationOnOldServer(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected OperationOnOldServerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
