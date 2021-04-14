using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Logging;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxStatisticsLogger : MailboxProcessor
	{
		public MailboxStatisticsLogger(ILogger logger, ObjectLogCollector logCollector) : base(logger)
		{
			AnchorUtil.ThrowOnNullArgument(logCollector, "logCollector");
			this.logCollector = logCollector;
		}

		public override bool RequiresRunspace
		{
			get
			{
				return false;
			}
		}

		public override void ProcessMailbox(DirectoryMailbox mailbox, IAnchorRunspaceProxy runspace)
		{
			mailbox.EmitLogEntry(this.logCollector);
		}

		private readonly ObjectLogCollector logCollector;
	}
}
