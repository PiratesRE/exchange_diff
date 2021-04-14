using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArbitrationExceptionCommon : ThrottlingRejectedOperationException
	{
		public ArbitrationExceptionCommon(string arbitrationMsg) : base(StringsRecovery.ArbitrationExceptionCommon(arbitrationMsg))
		{
			this.arbitrationMsg = arbitrationMsg;
		}

		public ArbitrationExceptionCommon(string arbitrationMsg, Exception innerException) : base(StringsRecovery.ArbitrationExceptionCommon(arbitrationMsg), innerException)
		{
			this.arbitrationMsg = arbitrationMsg;
		}

		protected ArbitrationExceptionCommon(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.arbitrationMsg = (string)info.GetValue("arbitrationMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("arbitrationMsg", this.arbitrationMsg);
		}

		public string ArbitrationMsg
		{
			get
			{
				return this.arbitrationMsg;
			}
		}

		private readonly string arbitrationMsg;
	}
}
