using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmMultiNodeRoleFetcher : AmMultiNodeRpcMap
	{
		internal Dictionary<AmServerName, AmRole> RoleMap
		{
			get
			{
				return this.roleMap;
			}
		}

		internal AmMultiNodeRoleFetcher(List<AmServerName> serverList, TimeSpan timeout, bool isCompleteOnMajority) : base(serverList, "AmMultiNodeRoleFetcher")
		{
			this.timeout = timeout;
		}

		protected override Exception RunServerRpc(AmServerName node, out object result)
		{
			Exception exception = null;
			result = null;
			AmRole? role = null;
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					string errorMessage = null;
					exception = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
					{
						role = new AmRole?(Dependencies.AmRpcClientWrapper.GetActiveManagerRole(node.Fqdn, out errorMessage));
					});
				}, this.timeout);
			}
			catch (TimeoutException exception)
			{
				TimeoutException exception2;
				exception = exception2;
			}
			if (role != null)
			{
				Interlocked.Increment(ref this.successCount);
				result = role;
			}
			return exception;
		}

		protected override void UpdateStatus(AmServerName node, object result)
		{
			if (result != null)
			{
				this.roleMap[node] = (AmRole)result;
			}
		}

		internal bool IsMajoritySuccessfulRepliesReceived(out int totalServerCount, out int successServerCount)
		{
			bool result = false;
			totalServerCount = this.m_expectedCount;
			successServerCount = this.successCount;
			int num = totalServerCount / 2 + 1;
			if (successServerCount >= num)
			{
				result = true;
			}
			return result;
		}

		public void Run()
		{
			base.RunAllRpcs();
		}

		private readonly TimeSpan timeout;

		private int successCount;

		private Dictionary<AmServerName, AmRole> roleMap = new Dictionary<AmServerName, AmRole>();
	}
}
