using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestTypeNotUnderstoodPermanentException : MailboxReplicationPermanentException
	{
		public RequestTypeNotUnderstoodPermanentException(string serverName, string serverVersion, int requestType) : base(MrsStrings.RequestTypeNotUnderstoodOnThisServer(serverName, serverVersion, requestType))
		{
			this.serverName = serverName;
			this.serverVersion = serverVersion;
			this.requestType = requestType;
		}

		public RequestTypeNotUnderstoodPermanentException(string serverName, string serverVersion, int requestType, Exception innerException) : base(MrsStrings.RequestTypeNotUnderstoodOnThisServer(serverName, serverVersion, requestType), innerException)
		{
			this.serverName = serverName;
			this.serverVersion = serverVersion;
			this.requestType = requestType;
		}

		protected RequestTypeNotUnderstoodPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.serverVersion = (string)info.GetValue("serverVersion", typeof(string));
			this.requestType = (int)info.GetValue("requestType", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("serverVersion", this.serverVersion);
			info.AddValue("requestType", this.requestType);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public int RequestType
		{
			get
			{
				return this.requestType;
			}
		}

		private readonly string serverName;

		private readonly string serverVersion;

		private readonly int requestType;
	}
}
