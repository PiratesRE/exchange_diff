using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArbitrationMinimumRequiredReadyNotSatisfiedException : ArbitrationExceptionCommon
	{
		public ArbitrationMinimumRequiredReadyNotSatisfiedException(int totalReady, int minimumRequired) : base(StringsRecovery.ArbitrationMinimumRequiredReadyNotSatisfied(totalReady, minimumRequired))
		{
			this.totalReady = totalReady;
			this.minimumRequired = minimumRequired;
		}

		public ArbitrationMinimumRequiredReadyNotSatisfiedException(int totalReady, int minimumRequired, Exception innerException) : base(StringsRecovery.ArbitrationMinimumRequiredReadyNotSatisfied(totalReady, minimumRequired), innerException)
		{
			this.totalReady = totalReady;
			this.minimumRequired = minimumRequired;
		}

		protected ArbitrationMinimumRequiredReadyNotSatisfiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.totalReady = (int)info.GetValue("totalReady", typeof(int));
			this.minimumRequired = (int)info.GetValue("minimumRequired", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("totalReady", this.totalReady);
			info.AddValue("minimumRequired", this.minimumRequired);
		}

		public int TotalReady
		{
			get
			{
				return this.totalReady;
			}
		}

		public int MinimumRequired
		{
			get
			{
				return this.minimumRequired;
			}
		}

		private readonly int totalReady;

		private readonly int minimumRequired;
	}
}
