using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedClientVersionPermanentException : MailboxReplicationPermanentException
	{
		public UnsupportedClientVersionPermanentException(string clientName, string clientVersion, string operationName) : base(MrsStrings.UnsupportedClientVersionWithOperation(clientName, clientVersion, operationName))
		{
			this.clientName = clientName;
			this.clientVersion = clientVersion;
			this.operationName = operationName;
		}

		public UnsupportedClientVersionPermanentException(string clientName, string clientVersion, string operationName, Exception innerException) : base(MrsStrings.UnsupportedClientVersionWithOperation(clientName, clientVersion, operationName), innerException)
		{
			this.clientName = clientName;
			this.clientVersion = clientVersion;
			this.operationName = operationName;
		}

		protected UnsupportedClientVersionPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.clientName = (string)info.GetValue("clientName", typeof(string));
			this.clientVersion = (string)info.GetValue("clientVersion", typeof(string));
			this.operationName = (string)info.GetValue("operationName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("clientName", this.clientName);
			info.AddValue("clientVersion", this.clientVersion);
			info.AddValue("operationName", this.operationName);
		}

		public string ClientName
		{
			get
			{
				return this.clientName;
			}
		}

		public string ClientVersion
		{
			get
			{
				return this.clientVersion;
			}
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		private readonly string clientName;

		private readonly string clientVersion;

		private readonly string operationName;
	}
}
