using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.ThirdPartyReplication;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmActiveThirdPartyMove
	{
		public AmActiveThirdPartyMove(IADDatabase db, string activeNodeName, bool mountDesired)
		{
			this.m_db = db;
			this.m_currentActiveNodeName = activeNodeName;
			this.m_mountDesired = mountDesired;
			this.Response = NotificationResponse.Incomplete;
		}

		public NotificationResponse Response { get; set; }

		internal void Notify(WaitCallback compRtn)
		{
			this.m_completionCallback = compRtn;
			AmTrace.Debug("AMTPR: Queuing notification for database {0}", new object[]
			{
				this.m_db.Name
			});
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.NotifyProcessing));
		}

		private void NotifyProcessing(object dummy)
		{
			try
			{
				this.Response = ThirdPartyManager.Instance.DatabaseMoveNeeded(this.m_db.Guid, this.m_currentActiveNodeName, this.m_mountDesired);
			}
			finally
			{
				AmTrace.Debug("AMTPR: Invoking completion callback for database {0}. Response={1}", new object[]
				{
					this.m_db.Name,
					this.Response
				});
				this.m_completionCallback(this);
			}
		}

		private IADDatabase m_db;

		private string m_currentActiveNodeName;

		private bool m_mountDesired;

		private WaitCallback m_completionCallback;
	}
}
