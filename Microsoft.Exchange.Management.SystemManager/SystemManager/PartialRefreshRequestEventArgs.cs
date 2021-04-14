using System;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class PartialRefreshRequestEventArgs : RefreshRequestEventArgs
	{
		public PartialRefreshRequestEventArgs(IProgress progress, object argument, object[] ids, RefreshRequestPriority priority) : base(false, progress, argument, priority)
		{
			this.Identities = ids;
		}

		public PartialRefreshRequestEventArgs(IProgress progress, object argument, object[] ids) : this(progress, argument, ids, 0)
		{
		}

		public object[] Identities { get; private set; }
	}
}
