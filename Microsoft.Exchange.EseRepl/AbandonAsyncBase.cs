using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Authentication;
using System.Threading;

namespace Microsoft.Exchange.EseRepl
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
				TcpChannel.Tracer.TraceDebug<string>(0L, "AbandonCompletionCallback ignoring exception {0}", ex.Message);
				return;
			}
			TcpChannel.Tracer.TraceDebug(0L, "AbandonCompletionCallback exits.");
		}

		public void Start()
		{
			TcpChannel.Tracer.TraceDebug<int>((long)this.GetHashCode(), "RemoteLogSource:Notification timeout after {0}msec", 10);
			this.m_timeActive.Reset();
			this.m_timeActive.Start();
		}

		public void Complete()
		{
			this.m_timeActive.Stop();
		}

		public Stopwatch TimeActive
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

		protected Stopwatch m_timeActive = new Stopwatch();
	}
}
