using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LocalThrottlingRejectedOperationException : ThrottlingRejectedOperationException
	{
		public LocalThrottlingRejectedOperationException(string actionId, string resourceName, string requester, string failedChecks) : base(StringsRecovery.LocalThrottlingRejectedOperation(actionId, resourceName, requester, failedChecks))
		{
			this.actionId = actionId;
			this.resourceName = resourceName;
			this.requester = requester;
			this.failedChecks = failedChecks;
		}

		public LocalThrottlingRejectedOperationException(string actionId, string resourceName, string requester, string failedChecks, Exception innerException) : base(StringsRecovery.LocalThrottlingRejectedOperation(actionId, resourceName, requester, failedChecks), innerException)
		{
			this.actionId = actionId;
			this.resourceName = resourceName;
			this.requester = requester;
			this.failedChecks = failedChecks;
		}

		protected LocalThrottlingRejectedOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.actionId = (string)info.GetValue("actionId", typeof(string));
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
			this.requester = (string)info.GetValue("requester", typeof(string));
			this.failedChecks = (string)info.GetValue("failedChecks", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("actionId", this.actionId);
			info.AddValue("resourceName", this.resourceName);
			info.AddValue("requester", this.requester);
			info.AddValue("failedChecks", this.failedChecks);
		}

		public string ActionId
		{
			get
			{
				return this.actionId;
			}
		}

		public string ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		public string Requester
		{
			get
			{
				return this.requester;
			}
		}

		public string FailedChecks
		{
			get
			{
				return this.failedChecks;
			}
		}

		private readonly string actionId;

		private readonly string resourceName;

		private readonly string requester;

		private readonly string failedChecks;
	}
}
