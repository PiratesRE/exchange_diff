using System;
using System.ComponentModel;
using System.IO;
using System.Security.Authentication;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class AbandonAsyncBase
	{
		protected static void AbandonCompletionCallback(object state)
		{
			IAsyncResult asyncResult = (IAsyncResult)state;
			AbandonAsyncBase abandonAsyncBase = (AbandonAsyncBase)asyncResult.AsyncState;
			Exception ex = null;
			try
			{
				abandonAsyncBase.CompletionProcessing(asyncResult);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (Win32Exception ex3)
			{
				ex = ex3;
			}
			catch (AuthenticationException ex4)
			{
				ex = ex4;
			}
			catch (ObjectDisposedException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				ExTraceGlobals.TcpChannelTracer.TraceDebug<string>(0L, "AbandonCompletionCallback ignoring exception {0}", ex.Message);
				return;
			}
			ExTraceGlobals.TcpChannelTracer.TraceDebug(0L, "AbandonCompletionCallback exits.");
		}

		public void Start()
		{
			ExTraceGlobals.TcpChannelTracer.TraceDebug<int>((long)this.GetHashCode(), "RemoteLogSource:Notification timeout after {0}msec", 10);
			this.m_timeActive.Reset();
			this.m_timeActive.Start();
		}

		public void Complete()
		{
			this.m_timeActive.Stop();
		}

		public ReplayStopwatch TimeActive
		{
			get
			{
				return this.m_timeActive;
			}
		}

		protected abstract void EndProcessing(IAsyncResult ar);

		private void CompletionProcessing(IAsyncResult ar)
		{
			this.m_timeActive.Stop();
			this.EndProcessing(ar);
		}

		public void Abandon(IAsyncResult ar)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(AbandonAsyncBase.AbandonCompletionCallback), ar);
		}

		protected ReplayStopwatch m_timeActive = new ReplayStopwatch();
	}
}
