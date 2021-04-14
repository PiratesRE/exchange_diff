using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class MailboxProcessor
	{
		protected MailboxProcessor(ILogger logger)
		{
			this.Logger = logger;
		}

		public virtual string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		public abstract bool RequiresRunspace { get; }

		private protected ILogger Logger { protected get; private set; }

		public abstract void ProcessMailbox(DirectoryMailbox mailbox, IAnchorRunspaceProxy runspace);

		public virtual bool ShouldProcess(DirectoryMailbox mailbox)
		{
			return true;
		}
	}
}
