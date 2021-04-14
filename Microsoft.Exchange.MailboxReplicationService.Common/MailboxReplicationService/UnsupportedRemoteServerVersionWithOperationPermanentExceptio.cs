using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedRemoteServerVersionWithOperationPermanentException : MailboxReplicationPermanentException
	{
		public UnsupportedRemoteServerVersionWithOperationPermanentException(string remoteServerAddress, string serverVersion, string operationName) : base(MrsStrings.UnsupportedRemoteServerVersionWithOperation(remoteServerAddress, serverVersion, operationName))
		{
			this.remoteServerAddress = remoteServerAddress;
			this.serverVersion = serverVersion;
			this.operationName = operationName;
		}

		public UnsupportedRemoteServerVersionWithOperationPermanentException(string remoteServerAddress, string serverVersion, string operationName, Exception innerException) : base(MrsStrings.UnsupportedRemoteServerVersionWithOperation(remoteServerAddress, serverVersion, operationName), innerException)
		{
			this.remoteServerAddress = remoteServerAddress;
			this.serverVersion = serverVersion;
			this.operationName = operationName;
		}

		protected UnsupportedRemoteServerVersionWithOperationPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.remoteServerAddress = (string)info.GetValue("remoteServerAddress", typeof(string));
			this.serverVersion = (string)info.GetValue("serverVersion", typeof(string));
			this.operationName = (string)info.GetValue("operationName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("remoteServerAddress", this.remoteServerAddress);
			info.AddValue("serverVersion", this.serverVersion);
			info.AddValue("operationName", this.operationName);
		}

		public string RemoteServerAddress
		{
			get
			{
				return this.remoteServerAddress;
			}
		}

		public string ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		private readonly string remoteServerAddress;

		private readonly string serverVersion;

		private readonly string operationName;
	}
}
