using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ChangeActiveServerException : ThirdPartyReplicationException
	{
		public ChangeActiveServerException(Guid dbId, string newServer, string reason) : base(ThirdPartyReplication.ChangeActiveServerFailed(dbId, newServer, reason))
		{
			this.dbId = dbId;
			this.newServer = newServer;
			this.reason = reason;
		}

		public ChangeActiveServerException(Guid dbId, string newServer, string reason, Exception innerException) : base(ThirdPartyReplication.ChangeActiveServerFailed(dbId, newServer, reason), innerException)
		{
			this.dbId = dbId;
			this.newServer = newServer;
			this.reason = reason;
		}

		protected ChangeActiveServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbId = (Guid)info.GetValue("dbId", typeof(Guid));
			this.newServer = (string)info.GetValue("newServer", typeof(string));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbId", this.dbId);
			info.AddValue("newServer", this.newServer);
			info.AddValue("reason", this.reason);
		}

		public Guid DbId
		{
			get
			{
				return this.dbId;
			}
		}

		public string NewServer
		{
			get
			{
				return this.newServer;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly Guid dbId;

		private readonly string newServer;

		private readonly string reason;
	}
}
