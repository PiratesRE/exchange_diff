using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BugCheckActionFailedException : RecoveryActionExceptionCommon
	{
		public BugCheckActionFailedException(string errMsg) : base(Strings.BugCheckActionFailed(errMsg))
		{
			this.errMsg = errMsg;
		}

		public BugCheckActionFailedException(string errMsg, Exception innerException) : base(Strings.BugCheckActionFailed(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected BugCheckActionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg", this.errMsg);
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string errMsg;
	}
}
