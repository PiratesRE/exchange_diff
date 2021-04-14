using System;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class RemoteRegistryBaseKey : IDisposable
	{
		public RegistryKey Key
		{
			get
			{
				return this.m_remoteKey;
			}
		}

		public void Open(RegistryHive hive, string serverName)
		{
			this.m_openCalled = true;
			int remoteRegistryTimeoutInMsec = RegistryParameters.RemoteRegistryTimeoutInMsec;
			RemoteRegistryBaseKey.AsyncOpenTask asyncOpenTask = new RemoteRegistryBaseKey.AsyncOpenTask(this.DoOpen);
			this.m_asyncRefCount = 2;
			IAsyncResult asyncResult = asyncOpenTask.BeginInvoke(hive, serverName, new AsyncCallback(RemoteRegistryBaseKey.OpenCompletion), asyncOpenTask);
			WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
			if (!asyncWaitHandle.WaitOne(remoteRegistryTimeoutInMsec, false) && Interlocked.Decrement(ref this.m_asyncRefCount) > 0)
			{
				throw new RemoteRegistryTimedOutException(serverName, remoteRegistryTimeoutInMsec / 1000);
			}
			asyncOpenTask.EndInvoke(asyncResult);
		}

		private static void OpenCompletion(IAsyncResult ar)
		{
			RemoteRegistryBaseKey.AsyncOpenTask asyncOpenTask = (RemoteRegistryBaseKey.AsyncOpenTask)ar.AsyncState;
			RemoteRegistryBaseKey remoteRegistryBaseKey = (RemoteRegistryBaseKey)asyncOpenTask.Target;
			if (Interlocked.Decrement(ref remoteRegistryBaseKey.m_asyncRefCount) > 0)
			{
				return;
			}
			Exception ex = null;
			try
			{
				asyncOpenTask.EndInvoke(ar);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.StateTracer.TraceDebug<string>((long)remoteRegistryBaseKey.GetHashCode(), "RemoteRegistryBaseKey hit exception after being abandoned: {0}", ex.Message);
				}
				remoteRegistryBaseKey.Dispose();
			}
		}

		private void DoOpen(RegistryHive hive, string serverName)
		{
			RegistryKey remoteKey = RegistryKey.OpenRemoteBaseKey(hive, serverName);
			lock (this)
			{
				this.m_remoteKey = remoteKey;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{
					if (this.m_remoteKey != null)
					{
						this.m_remoteKey.Close();
					}
					this.m_remoteKey = null;
				}
			}
		}

		private RegistryKey m_remoteKey;

		private int m_asyncRefCount = 2;

		private bool m_openCalled;

		private delegate void AsyncOpenTask(RegistryHive hive, string serverName);
	}
}
