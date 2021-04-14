using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ServerHealthStatus
	{
		public ServerHealthStatus()
		{
		}

		[DataMember(Name = "HealthState")]
		public ServerHealthState HealthState { get; set; }

		[DataMember(Name = "FailureReason", EmitDefaultValue = false)]
		public byte[] FailureReasonData { get; set; }

		[DataMember(Name = "Agent")]
		public int AgentInt { get; set; }

		public ServerHealthStatus(ServerHealthState healthState)
		{
			this.HealthState = healthState;
			this.Agent = ConstraintCheckAgent.None;
			this.FailureReason = LocalizedString.Empty;
		}

		public LocalizedString FailureReason
		{
			get
			{
				return CommonUtils.ByteDeserialize(this.FailureReasonData);
			}
			set
			{
				this.FailureReasonData = CommonUtils.ByteSerialize(value);
			}
		}

		public ConstraintCheckAgent Agent
		{
			get
			{
				return (ConstraintCheckAgent)this.AgentInt;
			}
			set
			{
				this.AgentInt = (int)value;
			}
		}

		public static ServerHealthStatus Healthy = new ServerHealthStatus(ServerHealthState.Healthy);
	}
}
