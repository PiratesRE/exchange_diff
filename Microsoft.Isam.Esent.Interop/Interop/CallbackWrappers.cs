using System;
using System.Collections.Generic;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class CallbackWrappers
	{
		public JetCallbackWrapper Add(JET_CALLBACK callback)
		{
			JetCallbackWrapper result;
			lock (this.lockObject)
			{
				JetCallbackWrapper jetCallbackWrapper;
				if (!this.TryFindWrapperFor(callback, out jetCallbackWrapper))
				{
					jetCallbackWrapper = new JetCallbackWrapper(callback);
					this.callbackWrappers.Add(jetCallbackWrapper);
				}
				result = jetCallbackWrapper;
			}
			return result;
		}

		public void Collect()
		{
			lock (this.lockObject)
			{
				this.callbackWrappers.RemoveAll((JetCallbackWrapper wrapper) => !wrapper.IsAlive);
			}
		}

		private bool TryFindWrapperFor(JET_CALLBACK callback, out JetCallbackWrapper wrapper)
		{
			foreach (JetCallbackWrapper jetCallbackWrapper in this.callbackWrappers)
			{
				if (jetCallbackWrapper.IsWrapping(callback))
				{
					wrapper = jetCallbackWrapper;
					return true;
				}
			}
			wrapper = null;
			return false;
		}

		private readonly object lockObject = new object();

		private readonly List<JetCallbackWrapper> callbackWrappers = new List<JetCallbackWrapper>();
	}
}
