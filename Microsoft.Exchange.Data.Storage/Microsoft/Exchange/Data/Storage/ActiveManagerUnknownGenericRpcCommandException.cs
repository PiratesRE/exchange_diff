using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ActiveManagerUnknownGenericRpcCommandException : AmServerException
	{
		public ActiveManagerUnknownGenericRpcCommandException(int requestedServerVersion, int replyingServerVersion, int commandId) : base(ServerStrings.ActiveManagerUnknownGenericRpcCommand(requestedServerVersion, replyingServerVersion, commandId))
		{
			this.requestedServerVersion = requestedServerVersion;
			this.replyingServerVersion = replyingServerVersion;
			this.commandId = commandId;
		}

		public ActiveManagerUnknownGenericRpcCommandException(int requestedServerVersion, int replyingServerVersion, int commandId, Exception innerException) : base(ServerStrings.ActiveManagerUnknownGenericRpcCommand(requestedServerVersion, replyingServerVersion, commandId), innerException)
		{
			this.requestedServerVersion = requestedServerVersion;
			this.replyingServerVersion = replyingServerVersion;
			this.commandId = commandId;
		}

		protected ActiveManagerUnknownGenericRpcCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.requestedServerVersion = (int)info.GetValue("requestedServerVersion", typeof(int));
			this.replyingServerVersion = (int)info.GetValue("replyingServerVersion", typeof(int));
			this.commandId = (int)info.GetValue("commandId", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("requestedServerVersion", this.requestedServerVersion);
			info.AddValue("replyingServerVersion", this.replyingServerVersion);
			info.AddValue("commandId", this.commandId);
		}

		public int RequestedServerVersion
		{
			get
			{
				return this.requestedServerVersion;
			}
		}

		public int ReplyingServerVersion
		{
			get
			{
				return this.replyingServerVersion;
			}
		}

		public int CommandId
		{
			get
			{
				return this.commandId;
			}
		}

		private readonly int requestedServerVersion;

		private readonly int replyingServerVersion;

		private readonly int commandId;
	}
}
