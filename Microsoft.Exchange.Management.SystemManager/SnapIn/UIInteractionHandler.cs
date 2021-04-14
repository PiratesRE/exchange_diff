using System;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal abstract class UIInteractionHandler
	{
		public abstract void DoActionSynchronizely(Action<IWin32Window> action);
	}
}
