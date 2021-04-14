using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct MailboxComponentOperationFrame : IDisposable
	{
		public static MailboxComponentOperationFrame CreateWrite(Context context, LockableMailboxComponent lockableMailboxComponent)
		{
			return new MailboxComponentOperationFrame(context, lockableMailboxComponent, true);
		}

		public static MailboxComponentOperationFrame CreateRead(Context context, LockableMailboxComponent lockableMailboxComponent)
		{
			return new MailboxComponentOperationFrame(context, lockableMailboxComponent, false);
		}

		private MailboxComponentOperationFrame(Context context, LockableMailboxComponent lockableMailboxComponent, bool writeOperation)
		{
			this.context = context;
			this.lockableMailboxComponent = lockableMailboxComponent;
			this.writeOperation = writeOperation;
			this.success = false;
			if (writeOperation)
			{
				context.StartMailboxComponentWriteOperation(lockableMailboxComponent);
				this.locked = true;
				return;
			}
			context.StartMailboxComponentReadOperation(lockableMailboxComponent);
			this.locked = true;
		}

		public void Success()
		{
			this.success = true;
		}

		public void Dispose()
		{
			if (this.locked)
			{
				if (this.writeOperation)
				{
					this.context.EndMailboxComponentWriteOperation(this.lockableMailboxComponent, this.success);
					return;
				}
				this.context.EndMailboxComponentReadOperation(this.lockableMailboxComponent);
			}
		}

		private readonly LockableMailboxComponent lockableMailboxComponent;

		private readonly Context context;

		private readonly bool writeOperation;

		private readonly bool locked;

		private bool success;
	}
}
