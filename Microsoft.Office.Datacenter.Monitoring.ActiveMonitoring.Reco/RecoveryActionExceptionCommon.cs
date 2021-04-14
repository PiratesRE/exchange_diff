using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecoveryActionExceptionCommon : LocalizedException
	{
		public RecoveryActionExceptionCommon(string recoveryActionMsg) : base(StringsRecovery.RecoveryActionExceptionCommon(recoveryActionMsg))
		{
			this.recoveryActionMsg = recoveryActionMsg;
		}

		public RecoveryActionExceptionCommon(string recoveryActionMsg, Exception innerException) : base(StringsRecovery.RecoveryActionExceptionCommon(recoveryActionMsg), innerException)
		{
			this.recoveryActionMsg = recoveryActionMsg;
		}

		protected RecoveryActionExceptionCommon(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recoveryActionMsg = (string)info.GetValue("recoveryActionMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recoveryActionMsg", this.recoveryActionMsg);
		}

		public string RecoveryActionMsg
		{
			get
			{
				return this.recoveryActionMsg;
			}
		}

		private readonly string recoveryActionMsg;
	}
}
