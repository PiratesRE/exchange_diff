using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxProcessorDispatcher : IDirectoryListener
	{
		public MailboxProcessorDispatcher(LoadBalanceAnchorContext context, Func<IRequestQueueManager, IList<MailboxProcessor>> processors)
		{
			AnchorUtil.ThrowOnNullArgument(context, "context");
			AnchorUtil.ThrowOnNullArgument(processors, "processors");
			this.context = context;
			this.processors = processors;
		}

		void IDirectoryListener.ObjectLoaded(DirectoryObject directoryObject)
		{
			if (!LoadBalanceADSettings.Instance.Value.MailboxProcessorsEnabled)
			{
				this.context.Logger.LogVerbose("Mailbox processors are not enabled, ignoring loaded AD item.", new object[0]);
				return;
			}
			DirectoryMailbox directoryMailbox = directoryObject as DirectoryMailbox;
			if (directoryMailbox != null)
			{
				this.BeginProcessingMailbox(directoryMailbox);
			}
		}

		private void BeginProcessingMailbox(DirectoryMailbox mailbox)
		{
			IRequestQueue requestQueue = (mailbox.Parent != null) ? this.context.QueueManager.GetProcessingQueue(mailbox.Parent) : this.context.QueueManager.MainProcessingQueue;
			string[] source = LoadBalanceADSettings.Instance.Value.ExcludedMailboxProcessors.Split(new char[]
			{
				','
			});
			IList<MailboxProcessor> list = this.processors(this.context.QueueManager) ?? ((IList<MailboxProcessor>)Array<MailboxProcessor>.Empty);
			foreach (MailboxProcessor mailboxProcessor in list)
			{
				if (source.Contains(mailboxProcessor.Name))
				{
					this.context.Logger.LogVerbose("Skipping processor {0} because it is disabled.", new object[]
					{
						mailboxProcessor.Name
					});
				}
				else if (!mailboxProcessor.ShouldProcess(mailbox))
				{
					this.context.Logger.LogVerbose("Processor {0} doesn't want to process the mailbox, ignored.", new object[]
					{
						mailboxProcessor.Name
					});
				}
				else if (mailboxProcessor.RequiresRunspace)
				{
					requestQueue.EnqueueRequest(new ProcessMailboxRequest(mailbox, mailboxProcessor, this.context.Logger, this.context.CmdletPool));
				}
				else
				{
					using (OperationTracker.Create(this.context.Logger, "Applying processor {0} to mailbox {1}.", new object[]
					{
						mailboxProcessor.GetType().FullName,
						mailbox.Identity
					}))
					{
						mailboxProcessor.ProcessMailbox(mailbox, null);
					}
				}
			}
		}

		private readonly LoadBalanceAnchorContext context;

		private readonly Func<IRequestQueueManager, IList<MailboxProcessor>> processors;
	}
}
