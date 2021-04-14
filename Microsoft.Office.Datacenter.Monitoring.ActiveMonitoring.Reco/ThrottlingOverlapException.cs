using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ThrottlingOverlapException : ThrottlingRejectedOperationException
	{
		public ThrottlingOverlapException(long currentInstanceId, long overlapInstanceId, string currentRequester, string overlapRequester, DateTime currentStartTime, DateTime overlapStartTime) : base(StringsRecovery.ThrottlingOverlapException(currentInstanceId, overlapInstanceId, currentRequester, overlapRequester, currentStartTime, overlapStartTime))
		{
			this.currentInstanceId = currentInstanceId;
			this.overlapInstanceId = overlapInstanceId;
			this.currentRequester = currentRequester;
			this.overlapRequester = overlapRequester;
			this.currentStartTime = currentStartTime;
			this.overlapStartTime = overlapStartTime;
		}

		public ThrottlingOverlapException(long currentInstanceId, long overlapInstanceId, string currentRequester, string overlapRequester, DateTime currentStartTime, DateTime overlapStartTime, Exception innerException) : base(StringsRecovery.ThrottlingOverlapException(currentInstanceId, overlapInstanceId, currentRequester, overlapRequester, currentStartTime, overlapStartTime), innerException)
		{
			this.currentInstanceId = currentInstanceId;
			this.overlapInstanceId = overlapInstanceId;
			this.currentRequester = currentRequester;
			this.overlapRequester = overlapRequester;
			this.currentStartTime = currentStartTime;
			this.overlapStartTime = overlapStartTime;
		}

		protected ThrottlingOverlapException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.currentInstanceId = (long)info.GetValue("currentInstanceId", typeof(long));
			this.overlapInstanceId = (long)info.GetValue("overlapInstanceId", typeof(long));
			this.currentRequester = (string)info.GetValue("currentRequester", typeof(string));
			this.overlapRequester = (string)info.GetValue("overlapRequester", typeof(string));
			this.currentStartTime = (DateTime)info.GetValue("currentStartTime", typeof(DateTime));
			this.overlapStartTime = (DateTime)info.GetValue("overlapStartTime", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("currentInstanceId", this.currentInstanceId);
			info.AddValue("overlapInstanceId", this.overlapInstanceId);
			info.AddValue("currentRequester", this.currentRequester);
			info.AddValue("overlapRequester", this.overlapRequester);
			info.AddValue("currentStartTime", this.currentStartTime);
			info.AddValue("overlapStartTime", this.overlapStartTime);
		}

		public long CurrentInstanceId
		{
			get
			{
				return this.currentInstanceId;
			}
		}

		public long OverlapInstanceId
		{
			get
			{
				return this.overlapInstanceId;
			}
		}

		public string CurrentRequester
		{
			get
			{
				return this.currentRequester;
			}
		}

		public string OverlapRequester
		{
			get
			{
				return this.overlapRequester;
			}
		}

		public DateTime CurrentStartTime
		{
			get
			{
				return this.currentStartTime;
			}
		}

		public DateTime OverlapStartTime
		{
			get
			{
				return this.overlapStartTime;
			}
		}

		private readonly long currentInstanceId;

		private readonly long overlapInstanceId;

		private readonly string currentRequester;

		private readonly string overlapRequester;

		private readonly DateTime currentStartTime;

		private readonly DateTime overlapStartTime;
	}
}
