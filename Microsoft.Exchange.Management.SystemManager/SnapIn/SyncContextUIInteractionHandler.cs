using System;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal sealed class SyncContextUIInteractionHandler : UIInteractionHandler
	{
		public SyncContextUIInteractionHandler(SynchronizationContext uiSyncContext)
		{
			if (uiSyncContext == null)
			{
				throw new ArgumentNullException("uiSyncContext");
			}
			this.uiSyncContext = uiSyncContext;
		}

		public override void DoActionSynchronizely(Action<IWin32Window> action)
		{
			this.uiSyncContext.Send(delegate(object param0)
			{
				action(null);
			}, null);
		}

		private SynchronizationContext uiSyncContext;
	}
}
