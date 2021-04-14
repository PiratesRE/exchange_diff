using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationRpcRequestTransientException : MigrationTransientException
	{
		public MigrationRpcRequestTransientException(string requestType, string serverName) : base(Strings.RpcRequestFailed(requestType, serverName))
		{
			this.requestType = requestType;
			this.serverName = serverName;
		}

		public MigrationRpcRequestTransientException(string requestType, string serverName, Exception innerException) : base(Strings.RpcRequestFailed(requestType, serverName), innerException)
		{
			this.requestType = requestType;
			this.serverName = serverName;
		}

		protected MigrationRpcRequestTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.requestType = (string)info.GetValue("requestType", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("requestType", this.requestType);
			info.AddValue("serverName", this.serverName);
		}

		public string RequestType
		{
			get
			{
				return this.requestType;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string requestType;

		private readonly string serverName;
	}
}
