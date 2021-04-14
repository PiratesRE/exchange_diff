using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OperationOnlyOnMailboxServerException : LocalizedException
	{
		public OperationOnlyOnMailboxServerException(string serverName) : base(Strings.ErrorOperationOnlyOnMailboxServer(serverName))
		{
			this.serverName = serverName;
		}

		public OperationOnlyOnMailboxServerException(string serverName, Exception innerException) : base(Strings.ErrorOperationOnlyOnMailboxServer(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected OperationOnlyOnMailboxServerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
