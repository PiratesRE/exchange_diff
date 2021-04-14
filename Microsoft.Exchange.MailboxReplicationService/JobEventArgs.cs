using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class JobEventArgs : EventArgs
	{
		public JobEventArgs(IJob job, JobState state)
		{
			this.job = job;
			this.state = state;
		}

		public IJob Job
		{
			get
			{
				return this.job;
			}
		}

		public JobState State
		{
			get
			{
				return this.state;
			}
		}

		private IJob job;

		private JobState state;
	}
}
