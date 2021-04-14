using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmMultiNodeRpcMap
	{
		internal Tuple<AmServerName, Exception>[] ServersStatusWithException
		{
			get
			{
				lock (this.m_locker)
				{
					if (this.m_rpcAttemptMap != null && this.m_rpcAttemptMap.Count > 0)
					{
						return (from x in this.m_rpcAttemptMap
						select new Tuple<AmServerName, Exception>(x.Key, x.Value)).ToArray<Tuple<AmServerName, Exception>>();
					}
				}
				return null;
			}
		}

		protected Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerTracer;
			}
		}

		protected AmMultiNodeRpcMap(string traceName)
		{
			this.m_name = traceName;
		}

		protected AmMultiNodeRpcMap(List<AmServerName> nodeList, string traceName)
		{
			this.m_name = traceName;
			this.Initialize(nodeList);
		}

		public List<AmServerName> ServersToContact
		{
			get
			{
				return this.m_nodeList;
			}
		}

		protected bool IsTimedout
		{
			get
			{
				return this.m_isTimedout;
			}
		}

		public bool WaitForCompletion(TimeSpan timeout)
		{
			ManualOneShotEvent.Result result = this.m_completionEvent.WaitOne(timeout);
			if (result != ManualOneShotEvent.Result.Success)
			{
				this.Tracer.TraceError<string, string, ManualOneShotEvent.Result>((long)this.GetHashCode(), "{0}: Multinode rpc timedout after {1}. WaitOne Result: {2}", this.m_name, timeout.ToString(), result);
				this.m_isTimedout = true;
				return false;
			}
			return true;
		}

		public Exception GetPossibleExceptionForServer(AmServerName node)
		{
			Exception result = null;
			lock (this.m_locker)
			{
				this.m_rpcAttemptMap.TryGetValue(node, out result);
			}
			return result;
		}

		internal virtual void TestInitialState()
		{
			DiagCore.RetailAssert(this.m_completionCount == 0, "m_completionCount should be 0 at the start.", new object[0]);
			DiagCore.RetailAssert(this.m_nodeList == null || this.m_expectedCount == this.m_nodeList.Count, "m_expectedCount should be same as m_nodeList.Count", new object[0]);
			DiagCore.RetailAssert(this.m_rpcAttemptMap != null, "m_rpcAttemptMap should not be null.", new object[0]);
			DiagCore.RetailAssert(this.m_rpcAttemptMap.Count == 0, "m_rpcAttemptMap should have 0 entries.", new object[0]);
		}

		internal virtual void TestFinalState()
		{
			DiagCore.RetailAssert(this.IsTimedout || this.m_completionCount == this.m_expectedCount, "m_completionCount should be equal to m_expectedCount.", new object[0]);
			DiagCore.RetailAssert(this.IsTimedout || this.m_rpcAttemptMap.Count == this.m_expectedCount, "m_rpcAttemptMap should have m_expectedCount entries.", new object[0]);
		}

		protected void Initialize(List<AmServerName> nodeList)
		{
			if (nodeList == null)
			{
				throw new ArgumentNullException("nodeList", "nodeList should not be null!");
			}
			this.m_nodeList = nodeList;
			this.m_expectedCount = nodeList.Count;
			this.m_rpcAttemptMap = new Dictionary<AmServerName, Exception>(nodeList.Count);
			this.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "{0}: Initializing with m_expectedCount={1}", this.m_name, this.m_expectedCount);
		}

		protected void RunAllRpcs()
		{
			this.RunAllRpcs(InvokeWithTimeout.InfiniteTimeSpan);
		}

		protected void RunAllRpcs(TimeSpan timeout)
		{
			if (this.m_nodeList != null && this.m_nodeList.Count > 0)
			{
				ReplayStopwatch replayStopwatch = new ReplayStopwatch();
				replayStopwatch.Start();
				try
				{
					bool flag = false;
					lock (this.m_locker)
					{
						flag = (this.m_rpcAttemptMap.Count == 0);
					}
					if (flag)
					{
						this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: RunAllRpcs(): Beginning to issue parallel RPCs.", this.m_name);
						List<AmServerName> list = new List<AmServerName>(this.m_nodeList.Count);
						foreach (AmServerName amServerName in this.m_nodeList)
						{
							if (this.TryStartRpc(amServerName))
							{
								list.Add(amServerName);
							}
							else
							{
								this.SkipSingleRpc(amServerName);
							}
						}
						foreach (AmServerName state in list)
						{
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunSingleRpc), state);
						}
					}
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: RunAllRpcs(): Waiting for RPCs to complete...", this.m_name);
					ManualOneShotEvent.Result result = this.m_completionEvent.WaitOne(timeout);
					if (!this.m_isTimedout)
					{
						this.m_isTimedout = (result == ManualOneShotEvent.Result.WaitTimedOut);
					}
					this.Tracer.TraceDebug<string, ManualOneShotEvent.Result>((long)this.GetHashCode(), "{0}: RunAllRpcs(): Waiting for RPCs returned: {1}", this.m_name, result);
					if (timeout == InvokeWithTimeout.InfiniteTimeSpan)
					{
						DiagCore.AssertOrWatson(result == ManualOneShotEvent.Result.Success, "waitResult cannot be anything other than Success! Actual: {0}", new object[]
						{
							result
						});
					}
					return;
				}
				finally
				{
					replayStopwatch.Stop();
					this.m_elapsedTime = replayStopwatch.Elapsed;
					this.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: RunAllRpcs(): Completed in {1}.", this.m_name, replayStopwatch.ToString());
				}
			}
			this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: RunAllRpcs(): No servers to contact since m_nodeList is null/empty. Setting completion event.", this.m_name);
			this.m_completionEvent.Set();
			this.m_completionEvent.Close();
		}

		protected abstract Exception RunServerRpc(AmServerName node, out object result);

		protected abstract void UpdateStatus(AmServerName node, object result);

		protected virtual bool TryStartRpc(AmServerName server)
		{
			return true;
		}

		protected virtual void RecordRpcCompleted(AmServerName server)
		{
		}

		protected virtual void Cleanup()
		{
			this.m_completionEvent.Close();
		}

		private void SkipSingleRpc(AmServerName node)
		{
			this.Tracer.TraceError<string, AmServerName>((long)this.GetHashCode(), "{0}: RunAllRpcs(): Skipping starting another thread to server '{1}' because an RPC thread is already running/hung.", this.m_name, node);
			lock (this.m_locker)
			{
				this.m_skippedCount++;
				this.m_isTimedout = true;
			}
		}

		private void RunSingleRpc(object obj)
		{
			AmServerName amServerName = obj as AmServerName;
			lock (this.m_locker)
			{
				this.m_rpcAttemptMap[amServerName] = null;
			}
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			Exception ex = null;
			object result;
			try
			{
				this.Tracer.TraceDebug<string, AmServerName>((long)this.GetHashCode(), "{0}: RunSingleRpc: Issuing RPC to server {1}...", this.m_name, amServerName);
				replayStopwatch.Start();
				ex = this.RunServerRpc(amServerName, out result);
			}
			finally
			{
				replayStopwatch.Stop();
				if (ex == null)
				{
					this.Tracer.TraceDebug<string, AmServerName, string>((long)this.GetHashCode(), "{0}: RunSingleRpc: RPC to server {1} completed successfully in {2}.", this.m_name, amServerName, replayStopwatch.ToString());
				}
				else
				{
					this.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: RunSingleRpc: RPC to server {1} completed with error in {2}. Exception: {3}", new object[]
					{
						this.m_name,
						amServerName,
						replayStopwatch.ToString(),
						ex
					});
				}
				this.RecordRpcCompleted(amServerName);
			}
			this.UpdateMap(amServerName, ex, result);
		}

		private void UpdateMap(AmServerName node, Exception ex, object result)
		{
			lock (this.m_locker)
			{
				this.m_rpcAttemptMap[node] = ex;
				this.UpdateStatus(node, result);
				this.m_completionCount++;
				if (this.m_completionCount == this.m_expectedCount - this.m_skippedCount)
				{
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: All RPCs completed. Signalling completion event.", this.m_name);
					this.m_completionEvent.Set();
					this.m_completionEvent.Close();
				}
			}
		}

		private const string RpcsCompletedEventName = "RpcsCompletedEvent";

		protected ManualOneShotEvent m_completionEvent = new ManualOneShotEvent("RpcsCompletedEvent");

		protected Dictionary<AmServerName, Exception> m_rpcAttemptMap;

		protected int m_expectedCount;

		protected TimeSpan m_elapsedTime;

		protected string m_name;

		private object m_locker = new object();

		private int m_completionCount;

		private int m_skippedCount;

		private List<AmServerName> m_nodeList;

		private bool m_isTimedout;
	}
}
