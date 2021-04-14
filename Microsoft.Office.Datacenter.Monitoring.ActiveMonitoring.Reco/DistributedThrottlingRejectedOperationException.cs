using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DistributedThrottlingRejectedOperationException : ArbitrationExceptionCommon
	{
		public DistributedThrottlingRejectedOperationException(string actionId, string requester) : base(StringsRecovery.DistributedThrottlingRejectedOperation(actionId, requester))
		{
			this.actionId = actionId;
			this.requester = requester;
		}

		public DistributedThrottlingRejectedOperationException(string actionId, string requester, Exception innerException) : base(StringsRecovery.DistributedThrottlingRejectedOperation(actionId, requester), innerException)
		{
			this.actionId = actionId;
			this.requester = requester;
		}

		protected DistributedThrottlingRejectedOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.actionId = (string)info.GetValue("actionId", typeof(string));
			this.requester = (string)info.GetValue("requester", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("actionId", this.actionId);
			info.AddValue("requester", this.requester);
		}

		public string ActionId
		{
			get
			{
				return this.actionId;
			}
		}

		public string Requester
		{
			get
			{
				return this.requester;
			}
		}

		private readonly string actionId;

		private readonly string requester;
	}
}
