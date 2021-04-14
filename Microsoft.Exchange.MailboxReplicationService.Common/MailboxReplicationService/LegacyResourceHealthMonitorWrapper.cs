using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LegacyResourceHealthMonitorWrapper : ResourceHealthMonitorWrapper, ILegacyResourceHealthProvider
	{
		public LegacyResourceHealthMonitorWrapper(LegacyResourceHealthMonitor provider) : base(provider)
		{
		}

		public void Update(ConstraintCheckResultType constraintResult, ConstraintCheckAgent agent, LocalizedString failureReason)
		{
			base.CheckExpired();
			LegacyResourceHealthMonitor wrappedMonitor = base.GetWrappedMonitor<LegacyResourceHealthMonitor>();
			wrappedMonitor.Update(constraintResult, agent, failureReason);
		}

		public ConstraintCheckResultType ConstraintResult
		{
			get
			{
				return base.GetWrappedMonitor<LegacyResourceHealthMonitor>().ConstraintResult;
			}
		}

		public ConstraintCheckAgent Agent
		{
			get
			{
				return base.GetWrappedMonitor<LegacyResourceHealthMonitor>().Agent;
			}
		}

		public LocalizedString FailureReason
		{
			get
			{
				return base.GetWrappedMonitor<LegacyResourceHealthMonitor>().FailureReason;
			}
		}
	}
}
