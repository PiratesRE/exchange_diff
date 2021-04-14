using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JobStalledPermanentException : MailboxReplicationPermanentException
	{
		public JobStalledPermanentException(LocalizedString failureReason, int agentId) : base(MrsStrings.JobIsStalledAndFailed(failureReason, agentId))
		{
			this.failureReason = failureReason;
			this.agentId = agentId;
		}

		public JobStalledPermanentException(LocalizedString failureReason, int agentId, Exception innerException) : base(MrsStrings.JobIsStalledAndFailed(failureReason, agentId), innerException)
		{
			this.failureReason = failureReason;
			this.agentId = agentId;
		}

		protected JobStalledPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failureReason = (LocalizedString)info.GetValue("failureReason", typeof(LocalizedString));
			this.agentId = (int)info.GetValue("agentId", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failureReason", this.failureReason);
			info.AddValue("agentId", this.agentId);
		}

		public LocalizedString FailureReason
		{
			get
			{
				return this.failureReason;
			}
		}

		public int AgentId
		{
			get
			{
				return this.agentId;
			}
		}

		private readonly LocalizedString failureReason;

		private readonly int agentId;
	}
}
