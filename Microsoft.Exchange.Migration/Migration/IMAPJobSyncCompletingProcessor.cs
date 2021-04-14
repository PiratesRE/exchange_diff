using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal class IMAPJobSyncCompletingProcessor : MigrationJobSyncCompletingProcessor
	{
		protected override string LicensingHelpUrl
		{
			get
			{
				return null;
			}
		}

		protected override string GetEmailSubject(bool errorsPresent)
		{
			if (base.Job.IsCancelled)
			{
				switch (base.Job.JobCancellationStatus)
				{
				case JobCancellationStatus.NotCancelled:
					break;
				case JobCancellationStatus.CancelledByUserRequest:
					return Strings.BatchCancelledByUser(base.Job.JobName);
				case JobCancellationStatus.CancelledDueToHighFailureCount:
					return Strings.BatchCancelledBySystem(base.Job.JobName);
				default:
					throw new InvalidOperationException("Unsupported job cancellation status " + base.Job.JobCancellationStatus);
				}
			}
			if (errorsPresent)
			{
				return Strings.BatchCompletionReportMailErrorHeader(base.Job.JobName);
			}
			return Strings.BatchCompletionReportMailHeader(base.Job.JobName);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IMAPJobSyncCompletingProcessor>(this);
		}
	}
}
