using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ThrottlingInProgressException : ThrottlingRejectedOperationException
	{
		public ThrottlingInProgressException(long instanceId, string actionId, string resourceName, string currentRequester, string inProgressRequester, DateTime operationStartTime, DateTime expectedEndTime) : base(StringsRecovery.ThrottlingInProgressException(instanceId, actionId, resourceName, currentRequester, inProgressRequester, operationStartTime, expectedEndTime))
		{
			this.instanceId = instanceId;
			this.actionId = actionId;
			this.resourceName = resourceName;
			this.currentRequester = currentRequester;
			this.inProgressRequester = inProgressRequester;
			this.operationStartTime = operationStartTime;
			this.expectedEndTime = expectedEndTime;
		}

		public ThrottlingInProgressException(long instanceId, string actionId, string resourceName, string currentRequester, string inProgressRequester, DateTime operationStartTime, DateTime expectedEndTime, Exception innerException) : base(StringsRecovery.ThrottlingInProgressException(instanceId, actionId, resourceName, currentRequester, inProgressRequester, operationStartTime, expectedEndTime), innerException)
		{
			this.instanceId = instanceId;
			this.actionId = actionId;
			this.resourceName = resourceName;
			this.currentRequester = currentRequester;
			this.inProgressRequester = inProgressRequester;
			this.operationStartTime = operationStartTime;
			this.expectedEndTime = expectedEndTime;
		}

		protected ThrottlingInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.instanceId = (long)info.GetValue("instanceId", typeof(long));
			this.actionId = (string)info.GetValue("actionId", typeof(string));
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
			this.currentRequester = (string)info.GetValue("currentRequester", typeof(string));
			this.inProgressRequester = (string)info.GetValue("inProgressRequester", typeof(string));
			this.operationStartTime = (DateTime)info.GetValue("operationStartTime", typeof(DateTime));
			this.expectedEndTime = (DateTime)info.GetValue("expectedEndTime", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("instanceId", this.instanceId);
			info.AddValue("actionId", this.actionId);
			info.AddValue("resourceName", this.resourceName);
			info.AddValue("currentRequester", this.currentRequester);
			info.AddValue("inProgressRequester", this.inProgressRequester);
			info.AddValue("operationStartTime", this.operationStartTime);
			info.AddValue("expectedEndTime", this.expectedEndTime);
		}

		public long InstanceId
		{
			get
			{
				return this.instanceId;
			}
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

		public string CurrentRequester
		{
			get
			{
				return this.currentRequester;
			}
		}

		public string InProgressRequester
		{
			get
			{
				return this.inProgressRequester;
			}
		}

		public DateTime OperationStartTime
		{
			get
			{
				return this.operationStartTime;
			}
		}

		public DateTime ExpectedEndTime
		{
			get
			{
				return this.expectedEndTime;
			}
		}

		private readonly long instanceId;

		private readonly string actionId;

		private readonly string resourceName;

		private readonly string currentRequester;

		private readonly string inProgressRequester;

		private readonly DateTime operationStartTime;

		private readonly DateTime expectedEndTime;
	}
}
