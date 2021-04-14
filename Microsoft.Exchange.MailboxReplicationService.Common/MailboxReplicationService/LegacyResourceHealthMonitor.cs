using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class LegacyResourceHealthMonitor : CacheableResourceHealthMonitor, ILegacyResourceHealthProvider
	{
		internal LegacyResourceHealthMonitor(ResourceKey key) : base(key)
		{
		}

		protected override int InternalMetricValue
		{
			get
			{
				return 0;
			}
		}

		public override ResourceHealthMonitorWrapper CreateWrapper()
		{
			return new LegacyResourceHealthMonitorWrapper(this);
		}

		public override ResourceLoad GetResourceLoad(WorkloadClassification classification, bool raw = false, object optionalData = null)
		{
			this.LastAccessUtc = DateTime.UtcNow;
			ResourceLoad resourceLoad;
			switch (this.constraintResult)
			{
			case ConstraintCheckResultType.Retry:
				resourceLoad = ResourceLoad.Unknown;
				goto IL_48;
			case ConstraintCheckResultType.Satisfied:
				resourceLoad = ResourceLoad.Zero;
				goto IL_48;
			case ConstraintCheckResultType.NotSatisfied:
				resourceLoad = ResourceLoad.Full;
				goto IL_48;
			}
			resourceLoad = ResourceLoad.Critical;
			IL_48:
			Guid databaseGuid = (base.Key as LegacyResourceHealthMonitorKey).DatabaseGuid;
			MrsTracer.ResourceHealth.Debug("LegacyResourceHealthMonitor.GetResourceLoad(): Mdb: '{0}', Constraint result: '{1}', Load: '{2}'", new object[]
			{
				databaseGuid,
				this.constraintResult,
				resourceLoad
			});
			return resourceLoad;
		}

		public void Update(ConstraintCheckResultType constraintResult, ConstraintCheckAgent agent, LocalizedString failureReason)
		{
			this.LastUpdateUtc = DateTime.UtcNow;
			this.constraintResult = constraintResult;
			this.agent = agent;
			this.failureReason = failureReason;
			Guid databaseGuid = (base.Key as LegacyResourceHealthMonitorKey).DatabaseGuid;
			MrsTracer.ResourceHealth.Debug("ILegacyResourceHealthProvider.Update(): Updating health for Mdb '{0}': Result: '{1}', Agent: '{2}', Reason: '{3}'", new object[]
			{
				databaseGuid,
				constraintResult,
				this.agent,
				this.failureReason
			});
		}

		public ConstraintCheckResultType ConstraintResult
		{
			get
			{
				return this.constraintResult;
			}
		}

		public ConstraintCheckAgent Agent
		{
			get
			{
				return this.agent;
			}
		}

		public LocalizedString FailureReason
		{
			get
			{
				return this.failureReason;
			}
		}

		private ConstraintCheckResultType constraintResult;

		private ConstraintCheckAgent agent;

		private LocalizedString failureReason;
	}
}
