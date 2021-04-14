using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ProcessMailboxRequest : BaseRequest
	{
		public ProcessMailboxRequest(DirectoryMailbox mailbox, MailboxProcessor processor, ILogger logger, CmdletExecutionPool cmdletPool)
		{
			AnchorUtil.ThrowOnNullArgument(mailbox, "mailbox");
			AnchorUtil.ThrowOnNullArgument(processor, "processor");
			this.Mailbox = mailbox;
			this.Processor = processor;
			this.logger = logger;
			this.cmdletPool = cmdletPool;
		}

		public override bool IsBlocked
		{
			get
			{
				return !this.cmdletPool.HasRunspacesAvailable;
			}
		}

		internal MailboxProcessor Processor { get; private set; }

		internal DirectoryMailbox Mailbox { get; private set; }

		public override RequestDiagnosticData GetDiagnosticData(bool verbose)
		{
			RequestDiagnosticData diagnosticData = base.GetDiagnosticData(verbose);
			diagnosticData.RequestKind = string.Format("ProcessMailboxRequest/{0}", this.Processor.Name);
			return diagnosticData;
		}

		protected override void ProcessRequest()
		{
			using (OperationTracker.Create(this.logger, "Applying processor {0} to mailbox {1}.", new object[]
			{
				this.Processor.GetType().FullName,
				this.Mailbox.Identity
			}))
			{
				using (RunspaceReservation runspaceReservation = this.cmdletPool.AcquireRunspace())
				{
					this.Processor.ProcessMailbox(this.Mailbox, runspaceReservation.Runspace);
				}
			}
		}

		private readonly ILogger logger;

		private readonly CmdletExecutionPool cmdletPool;
	}
}
