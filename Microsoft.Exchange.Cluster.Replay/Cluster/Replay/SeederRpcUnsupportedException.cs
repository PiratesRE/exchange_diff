using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederRpcUnsupportedException : TaskServerException
	{
		public SeederRpcUnsupportedException(string serverName, string serverVersion, string supportedVersion) : base(ReplayStrings.SeederRpcUnsupportedException(serverName, serverVersion, supportedVersion))
		{
			this.serverName = serverName;
			this.serverVersion = serverVersion;
			this.supportedVersion = supportedVersion;
		}

		public SeederRpcUnsupportedException(string serverName, string serverVersion, string supportedVersion, Exception innerException) : base(ReplayStrings.SeederRpcUnsupportedException(serverName, serverVersion, supportedVersion), innerException)
		{
			this.serverName = serverName;
			this.serverVersion = serverVersion;
			this.supportedVersion = supportedVersion;
		}

		protected SeederRpcUnsupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.serverVersion = (string)info.GetValue("serverVersion", typeof(string));
			this.supportedVersion = (string)info.GetValue("supportedVersion", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("serverVersion", this.serverVersion);
			info.AddValue("supportedVersion", this.supportedVersion);
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

		public string SupportedVersion
		{
			get
			{
				return this.supportedVersion;
			}
		}

		private readonly string serverName;

		private readonly string serverVersion;

		private readonly string supportedVersion;
	}
}
