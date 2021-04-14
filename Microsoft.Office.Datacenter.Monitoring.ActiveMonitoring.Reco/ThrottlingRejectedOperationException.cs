using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ThrottlingRejectedOperationException : RecoveryActionExceptionCommon
	{
		public ThrottlingRejectedOperationException(string rejectedOperationMsg) : base(StringsRecovery.ThrottlingRejectedOperationException(rejectedOperationMsg))
		{
			this.rejectedOperationMsg = rejectedOperationMsg;
		}

		public ThrottlingRejectedOperationException(string rejectedOperationMsg, Exception innerException) : base(StringsRecovery.ThrottlingRejectedOperationException(rejectedOperationMsg), innerException)
		{
			this.rejectedOperationMsg = rejectedOperationMsg;
		}

		protected ThrottlingRejectedOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.rejectedOperationMsg = (string)info.GetValue("rejectedOperationMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rejectedOperationMsg", this.rejectedOperationMsg);
		}

		public string RejectedOperationMsg
		{
			get
			{
				return this.rejectedOperationMsg;
			}
		}

		private readonly string rejectedOperationMsg;
	}
}
