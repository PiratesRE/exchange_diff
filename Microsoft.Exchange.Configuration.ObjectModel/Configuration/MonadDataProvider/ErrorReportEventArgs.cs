using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class ErrorReportEventArgs : RunGuidEventArgs
	{
		public ErrorReportEventArgs(Guid guid, ErrorRecord errorRecord, int objectIndex, MonadCommand command) : base(guid)
		{
			if (errorRecord == null)
			{
				throw new ArgumentNullException("errorRecord");
			}
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			this.errorRecord = errorRecord;
			this.objectIndex = objectIndex;
			this.command = command;
		}

		public ErrorRecord ErrorRecord
		{
			get
			{
				return this.errorRecord;
			}
		}

		public int ObjectIndex
		{
			get
			{
				return this.objectIndex;
			}
		}

		public MonadCommand Command
		{
			get
			{
				return this.command;
			}
		}

		private ErrorRecord errorRecord;

		private int objectIndex;

		private MonadCommand command;
	}
}
