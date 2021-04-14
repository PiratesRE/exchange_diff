using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmEvtServerSwitchoverBase : AmEvtBase
	{
		internal AmEvtServerSwitchoverBase(AmServerName nodeName)
		{
			this.NodeName = nodeName;
			this.m_completionEvent = new ManualResetEvent(false);
		}

		internal AmServerName NodeName { get; set; }

		internal bool IsDone { get; set; }

		internal List<AmDbOperation> OperationList { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}: Params: (Nodename={1})", base.GetType().Name, this.NodeName);
		}

		internal void SwitchoverCompletedCallback(List<AmDbOperation> operationList)
		{
			lock (this.m_locker)
			{
				this.OperationList = operationList;
				this.IsDone = true;
				if (this.m_completionEvent != null)
				{
					this.m_completionEvent.Set();
				}
			}
		}

		internal void WaitForSwitchoverComplete(TimeSpan timeout)
		{
			if (this.m_completionEvent != null)
			{
				this.m_completionEvent.WaitOne(timeout, false);
				lock (this.m_locker)
				{
					this.m_completionEvent.Close();
					this.m_completionEvent = null;
				}
			}
		}

		internal void WaitForSwitchoverComplete()
		{
			this.WaitForSwitchoverComplete(TimeSpan.FromMilliseconds(-1.0));
		}

		private object m_locker = new object();

		private ManualResetEvent m_completionEvent;
	}
}
