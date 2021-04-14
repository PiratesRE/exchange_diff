using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreationDiagnostics
	{
		public CreationDiagnostics()
		{
			this.stopWatch = new Stopwatch();
		}

		public TimeSpan? MailboxCreationTime { get; private set; }

		public TimeSpan? AADIdentityCreationTime { get; private set; }

		public TimeSpan? AADCompleteCallbackTime { get; private set; }

		public TimeSpan? SharePointNotificationTime { get; private set; }

		public TimeSpan? GroupCreationTime { get; private set; }

		public bool MailboxCreatedSuccessfully { get; set; }

		public Guid CmdletLogCorrelationId { get; set; }

		public void Start()
		{
			this.stopWatch.Start();
		}

		public void Stop()
		{
			this.GroupCreationTime = new TimeSpan?(this.stopWatch.Elapsed);
			this.stopWatch.Stop();
		}

		public void RecordAADTime()
		{
			this.AADIdentityCreationTime = new TimeSpan?(this.stopWatch.Elapsed);
		}

		public void RecordAADCompleteCallbackTime()
		{
			this.AADCompleteCallbackTime = new TimeSpan?(this.stopWatch.Elapsed.Subtract(this.AADIdentityCreationTime.Value));
		}

		public void RecordSharePointNotificationTime()
		{
			this.SharePointNotificationTime = new TimeSpan?(this.stopWatch.Elapsed.Subtract(this.AADCompleteCallbackTime.Value));
		}

		public void RecordMailboxTime()
		{
			this.MailboxCreationTime = new TimeSpan?(this.stopWatch.Elapsed.Subtract(this.SharePointNotificationTime.Value));
		}

		private readonly Stopwatch stopWatch;
	}
}
