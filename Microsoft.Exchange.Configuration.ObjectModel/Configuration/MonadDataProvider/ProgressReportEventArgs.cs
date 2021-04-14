using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class ProgressReportEventArgs : EventArgs
	{
		public ProgressReportEventArgs(ProgressRecord progressRecord, MonadCommand command)
		{
			if (progressRecord == null)
			{
				throw new ArgumentNullException("progressRecord");
			}
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			this.progressRecord = progressRecord;
			this.command = command;
		}

		public ProgressRecord ProgressRecord
		{
			get
			{
				return this.progressRecord;
			}
		}

		public int ObjectIndex
		{
			get
			{
				return this.ProgressRecord.ActivityId;
			}
		}

		public MonadCommand Command
		{
			get
			{
				return this.command;
			}
		}

		private ProgressRecord progressRecord;

		private MonadCommand command;
	}
}
