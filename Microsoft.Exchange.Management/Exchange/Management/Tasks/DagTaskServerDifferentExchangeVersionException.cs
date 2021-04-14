using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerDifferentExchangeVersionException : LocalizedException
	{
		public DagTaskServerDifferentExchangeVersionException(string dagName, string existingServer, object existingVersion, string serverName, object serverVersion) : base(Strings.DagTaskServerDifferentExchangeVersion(dagName, existingServer, existingVersion, serverName, serverVersion))
		{
			this.dagName = dagName;
			this.existingServer = existingServer;
			this.existingVersion = existingVersion;
			this.serverName = serverName;
			this.serverVersion = serverVersion;
		}

		public DagTaskServerDifferentExchangeVersionException(string dagName, string existingServer, object existingVersion, string serverName, object serverVersion, Exception innerException) : base(Strings.DagTaskServerDifferentExchangeVersion(dagName, existingServer, existingVersion, serverName, serverVersion), innerException)
		{
			this.dagName = dagName;
			this.existingServer = existingServer;
			this.existingVersion = existingVersion;
			this.serverName = serverName;
			this.serverVersion = serverVersion;
		}

		protected DagTaskServerDifferentExchangeVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
			this.existingServer = (string)info.GetValue("existingServer", typeof(string));
			this.existingVersion = info.GetValue("existingVersion", typeof(object));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.serverVersion = info.GetValue("serverVersion", typeof(object));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
			info.AddValue("existingServer", this.existingServer);
			info.AddValue("existingVersion", this.existingVersion);
			info.AddValue("serverName", this.serverName);
			info.AddValue("serverVersion", this.serverVersion);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		public string ExistingServer
		{
			get
			{
				return this.existingServer;
			}
		}

		public object ExistingVersion
		{
			get
			{
				return this.existingVersion;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public object ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		private readonly string dagName;

		private readonly string existingServer;

		private readonly object existingVersion;

		private readonly string serverName;

		private readonly object serverVersion;
	}
}
