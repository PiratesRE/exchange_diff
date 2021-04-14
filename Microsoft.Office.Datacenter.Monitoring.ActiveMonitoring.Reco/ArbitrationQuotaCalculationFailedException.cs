using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArbitrationQuotaCalculationFailedException : ArbitrationExceptionCommon
	{
		public ArbitrationQuotaCalculationFailedException(int exhaustedQuota, int allowedQuota, bool isConcluded, bool isInvokedTooSoon) : base(StringsRecovery.ArbitrationQuotaCalculationFailed(exhaustedQuota, allowedQuota, isConcluded, isInvokedTooSoon))
		{
			this.exhaustedQuota = exhaustedQuota;
			this.allowedQuota = allowedQuota;
			this.isConcluded = isConcluded;
			this.isInvokedTooSoon = isInvokedTooSoon;
		}

		public ArbitrationQuotaCalculationFailedException(int exhaustedQuota, int allowedQuota, bool isConcluded, bool isInvokedTooSoon, Exception innerException) : base(StringsRecovery.ArbitrationQuotaCalculationFailed(exhaustedQuota, allowedQuota, isConcluded, isInvokedTooSoon), innerException)
		{
			this.exhaustedQuota = exhaustedQuota;
			this.allowedQuota = allowedQuota;
			this.isConcluded = isConcluded;
			this.isInvokedTooSoon = isInvokedTooSoon;
		}

		protected ArbitrationQuotaCalculationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exhaustedQuota = (int)info.GetValue("exhaustedQuota", typeof(int));
			this.allowedQuota = (int)info.GetValue("allowedQuota", typeof(int));
			this.isConcluded = (bool)info.GetValue("isConcluded", typeof(bool));
			this.isInvokedTooSoon = (bool)info.GetValue("isInvokedTooSoon", typeof(bool));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exhaustedQuota", this.exhaustedQuota);
			info.AddValue("allowedQuota", this.allowedQuota);
			info.AddValue("isConcluded", this.isConcluded);
			info.AddValue("isInvokedTooSoon", this.isInvokedTooSoon);
		}

		public int ExhaustedQuota
		{
			get
			{
				return this.exhaustedQuota;
			}
		}

		public int AllowedQuota
		{
			get
			{
				return this.allowedQuota;
			}
		}

		public bool IsConcluded
		{
			get
			{
				return this.isConcluded;
			}
		}

		public bool IsInvokedTooSoon
		{
			get
			{
				return this.isInvokedTooSoon;
			}
		}

		private readonly int exhaustedQuota;

		private readonly int allowedQuota;

		private readonly bool isConcluded;

		private readonly bool isInvokedTooSoon;
	}
}
