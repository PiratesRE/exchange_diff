using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JobStalledTransientException : MailboxReplicationTransientException
	{
		public JobStalledTransientException(string jobId, string mdbId, LocalizedString failureReason, string agentName, int agentId) : base(MrsStrings.JobIsStalled(jobId, mdbId, failureReason, agentName, agentId))
		{
			this.jobId = jobId;
			this.mdbId = mdbId;
			this.failureReason = failureReason;
			this.agentName = agentName;
			this.agentId = agentId;
		}

		public JobStalledTransientException(string jobId, string mdbId, LocalizedString failureReason, string agentName, int agentId, Exception innerException) : base(MrsStrings.JobIsStalled(jobId, mdbId, failureReason, agentName, agentId), innerException)
		{
			this.jobId = jobId;
			this.mdbId = mdbId;
			this.failureReason = failureReason;
			this.agentName = agentName;
			this.agentId = agentId;
		}

		protected JobStalledTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.jobId = (string)info.GetValue("jobId", typeof(string));
			this.mdbId = (string)info.GetValue("mdbId", typeof(string));
			this.failureReason = (LocalizedString)info.GetValue("failureReason", typeof(LocalizedString));
			this.agentName = (string)info.GetValue("agentName", typeof(string));
			this.agentId = (int)info.GetValue("agentId", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("jobId", this.jobId);
			info.AddValue("mdbId", this.mdbId);
			info.AddValue("failureReason", this.failureReason);
			info.AddValue("agentName", this.agentName);
			info.AddValue("agentId", this.agentId);
		}

		public string JobId
		{
			get
			{
				return this.jobId;
			}
		}

		public string MdbId
		{
			get
			{
				return this.mdbId;
			}
		}

		public LocalizedString FailureReason
		{
			get
			{
				return this.failureReason;
			}
		}

		public string AgentName
		{
			get
			{
				return this.agentName;
			}
		}

		public int AgentId
		{
			get
			{
				return this.agentId;
			}
		}

		private readonly string jobId;

		private readonly string mdbId;

		private readonly LocalizedString failureReason;

		private readonly string agentName;

		private readonly int agentId;
	}
}
