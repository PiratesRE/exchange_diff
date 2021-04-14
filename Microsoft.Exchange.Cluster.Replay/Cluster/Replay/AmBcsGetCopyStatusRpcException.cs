using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmBcsGetCopyStatusRpcException : AmBcsSingleCopyValidationException
	{
		public AmBcsGetCopyStatusRpcException(string server, string database, string rpcError) : base(ReplayStrings.AmBcsGetCopyStatusRpcException(server, database, rpcError))
		{
			this.server = server;
			this.database = database;
			this.rpcError = rpcError;
		}

		public AmBcsGetCopyStatusRpcException(string server, string database, string rpcError, Exception innerException) : base(ReplayStrings.AmBcsGetCopyStatusRpcException(server, database, rpcError), innerException)
		{
			this.server = server;
			this.database = database;
			this.rpcError = rpcError;
		}

		protected AmBcsGetCopyStatusRpcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.database = (string)info.GetValue("database", typeof(string));
			this.rpcError = (string)info.GetValue("rpcError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("database", this.database);
			info.AddValue("rpcError", this.rpcError);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string RpcError
		{
			get
			{
				return this.rpcError;
			}
		}

		private readonly string server;

		private readonly string database;

		private readonly string rpcError;
	}
}
