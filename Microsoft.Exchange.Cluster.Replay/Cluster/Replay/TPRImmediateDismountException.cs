using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TPRImmediateDismountException : TransientException
	{
		public TPRImmediateDismountException(Guid dbId, string reason) : base(ReplayStrings.TPRmmediateDismountFailed(dbId, reason))
		{
			this.dbId = dbId;
			this.reason = reason;
		}

		public TPRImmediateDismountException(Guid dbId, string reason, Exception innerException) : base(ReplayStrings.TPRmmediateDismountFailed(dbId, reason), innerException)
		{
			this.dbId = dbId;
			this.reason = reason;
		}

		protected TPRImmediateDismountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbId = (Guid)info.GetValue("dbId", typeof(Guid));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbId", this.dbId);
			info.AddValue("reason", this.reason);
		}

		public Guid DbId
		{
			get
			{
				return this.dbId;
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

		private readonly string reason;
	}
}
