using System;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal sealed class WindowUIInteractionHandler : UIInteractionHandler
	{
		public WindowUIInteractionHandler(Control window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			this.window = window;
		}

		public override void DoActionSynchronizely(Action<IWin32Window> action)
		{
			if (this.window.InvokeRequired)
			{
				this.window.Invoke(action, new object[]
				{
					this.window
				});
				return;
			}
			action(this.window);
		}

		private Control window;
	}
}
