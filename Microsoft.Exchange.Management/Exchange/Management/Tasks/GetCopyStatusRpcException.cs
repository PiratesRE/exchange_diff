using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetCopyStatusRpcException : LocalizedException
	{
		public GetCopyStatusRpcException(string server, string databaseName, string errorMessage) : base(Strings.GetCopyStatusRpcException(server, databaseName, errorMessage))
		{
			this.server = server;
			this.databaseName = databaseName;
			this.errorMessage = errorMessage;
		}

		public GetCopyStatusRpcException(string server, string databaseName, string errorMessage, Exception innerException) : base(Strings.GetCopyStatusRpcException(server, databaseName, errorMessage), innerException)
		{
			this.server = server;
			this.databaseName = databaseName;
			this.errorMessage = errorMessage;
		}

		protected GetCopyStatusRpcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string server;

		private readonly string databaseName;

		private readonly string errorMessage;
	}
}
