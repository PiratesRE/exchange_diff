using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class PrimaryConsistencyCheckResult : ConsistencyCheckResult
	{
		protected PrimaryConsistencyCheckResult(ConsistencyCheckType checkType, string checkDescription, bool terminateIfNotPassed) : base(checkType, checkDescription)
		{
			this.terminationOverride = false;
			this.terminateIfNotPassed = terminateIfNotPassed;
		}

		internal static PrimaryConsistencyCheckResult CreateInstance(ConsistencyCheckType checkType, string checkDescription, bool terminateIfNotPassed)
		{
			return new PrimaryConsistencyCheckResult(checkType, checkDescription, terminateIfNotPassed);
		}

		internal virtual bool ShouldTerminate
		{
			get
			{
				return this.terminationOverride || (this.terminateIfNotPassed && base.Status != CheckStatusType.Passed);
			}
		}

		internal void TerminateCheck(string error)
		{
			this.terminationOverride = true;
			if (error != null)
			{
				base.ErrorString = error;
			}
		}

		private readonly bool terminateIfNotPassed;

		private bool terminationOverride;
	}
}
