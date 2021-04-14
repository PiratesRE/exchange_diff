using System;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.HA
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvokeWithTimeout
	{
		private InvokeWithTimeout()
		{
		}

		private static bool UseSyncCall()
		{
			if (!InvokeWithTimeout.s_useSyncCallDetermined)
			{
				Exception ex = null;
				try
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HA\\InvokeWithTimeout"))
					{
						if (registryKey != null)
						{
							int value = RegistryReader.Instance.GetValue<int>(registryKey, null, "UseSyncCall", 0);
							if (value != 0)
							{
								InvokeWithTimeout.s_useSyncCall = true;
							}
						}
					}
				}
				catch (SecurityException ex2)
				{
					ex = ex2;
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					InvokeWithTimeout.Tracer.TraceError<Exception>(0L, "UseSyncCall failed to read regkey: {0}", ex);
				}
				InvokeWithTimeout.s_useSyncCallDetermined = true;
			}
			return InvokeWithTimeout.s_useSyncCall;
		}

		public static void Invoke(Action invokableAction, TimeSpan invokeTimeout)
		{
			InvokeWithTimeout.Invoke(invokableAction, null, invokeTimeout, false, null);
		}

		public static void Invoke(Action invokableAction, TimeSpan invokeTimeout, object cancelEvent)
		{
			InvokeWithTimeout.Invoke(invokableAction, null, invokeTimeout, false, cancelEvent);
		}

		public static void Invoke(Action invokableAction, Action foregroundAction, TimeSpan invokeTimeout, bool sendWatsonReportNoThrow, object cancelEvent)
		{
			if (foregroundAction == null && (invokeTimeout == InvokeWithTimeout.InfiniteTimeSpan || InvokeWithTimeout.UseSyncCall()))
			{
				invokableAction();
				return;
			}
			InvokeWithTimeout invokeWithTimeout = new InvokeWithTimeout();
			invokeWithTimeout.InternalInvoke(invokableAction, foregroundAction, invokeTimeout, sendWatsonReportNoThrow, cancelEvent);
		}

		private void InternalInvoke(Action invokableAction, Action foregroundAction, TimeSpan invokeTimeout, bool sendWatsonReportNoThrow, object cancelEvent)
		{
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "InternalInvoke calling BeginInvoke");
			IAsyncResult result = invokableAction.BeginInvoke(new AsyncCallback(this.CompletionCallback), invokableAction);
			DateTime utcNow = DateTime.UtcNow;
			WaitHandle asyncWaitHandle = result.AsyncWaitHandle;
			bool flag = true;
			bool flag2 = false;
			try
			{
				if (foregroundAction != null)
				{
					foregroundAction();
				}
				flag2 = true;
				TimeSpan timeSpan = DateTime.UtcNow.Subtract(utcNow);
				TimeSpan timeout;
				if (invokeTimeout == InvokeWithTimeout.InfiniteTimeSpan)
				{
					timeout = invokeTimeout;
				}
				else if (timeSpan < invokeTimeout)
				{
					timeout = invokeTimeout.Subtract(timeSpan);
				}
				else
				{
					timeout = TimeSpan.Zero;
				}
				int num = 1;
				if (cancelEvent != null)
				{
					num = 2;
				}
				object[] array = new object[num];
				array[0] = asyncWaitHandle;
				if (cancelEvent != null)
				{
					array[1] = cancelEvent;
				}
				int num2 = ManualOneShotEvent.WaitAny(array, timeout);
				bool flag3 = false;
				bool flag4 = false;
				if (num2 == 258)
				{
					flag3 = true;
				}
				else if (num2 == 1)
				{
					flag4 = true;
				}
				if (flag3 || flag4)
				{
					if (flag3 && sendWatsonReportNoThrow)
					{
						if (this.m_asyncRefCount == 1)
						{
							flag3 = false;
						}
					}
					else if (Interlocked.Decrement(ref this.m_asyncRefCount) == 0)
					{
						flag3 = false;
						flag4 = false;
					}
				}
				if (flag3)
				{
					TimeoutException ex = new TimeoutException(Strings.OperationTimedOut(invokeTimeout.ToString()));
					if (!sendWatsonReportNoThrow)
					{
						flag = false;
						throw ex;
					}
					this.SendWatsonReport<TimeoutException>(ex);
					invokableAction.EndInvoke(result);
				}
				else
				{
					if (flag4)
					{
						OperationAbortedException ex2 = new OperationAbortedException();
						flag = false;
						throw ex2;
					}
					invokableAction.EndInvoke(result);
				}
			}
			finally
			{
				if (!flag2)
				{
					if (Interlocked.Decrement(ref this.m_asyncRefCount) > 0)
					{
						flag = false;
					}
					else
					{
						Exception ex3 = this.RunOperation(delegate
						{
							invokableAction.EndInvoke(result);
						});
						if (ex3 != null)
						{
							ExTraceGlobals.ClusterTracer.TraceError<Exception>((long)this.GetHashCode(), "EndInvoke() has thrown an exception after the foreground thread threw an exception. Exception: {0}", ex3);
						}
					}
				}
				if (flag && asyncWaitHandle != null)
				{
					asyncWaitHandle.Close();
				}
			}
		}

		private void SendWatsonReport<TException>(TException exception) where TException : Exception
		{
			try
			{
				throw exception;
			}
			catch (TException ex)
			{
				TException ex2 = (TException)((object)ex);
				ExWatson.SendReport(ex2, ReportOptions.None, ex2.Message);
			}
		}

		private void CompletionCallback(IAsyncResult ar)
		{
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "CompletionCallback entered");
			if (Interlocked.Decrement(ref this.m_asyncRefCount) > 0)
			{
				return;
			}
			Exception ex = this.RunOperation(delegate
			{
				((Action)ar.AsyncState).EndInvoke(ar);
			});
			if (ex != null)
			{
				InvokeWithTimeout.Tracer.TraceError<Exception>((long)this.GetHashCode(), "EndInvoke() has thrown an exception after async task exited on timeout. Exception: {0}", ex);
			}
			if (ar.AsyncWaitHandle != null)
			{
				ar.AsyncWaitHandle.Close();
			}
		}

		private Exception RunOperation(Action operation)
		{
			Exception ex = null;
			try
			{
				operation();
			}
			catch (Win32Exception ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (TimeoutException ex4)
			{
				ex = ex4;
			}
			catch (ObjectDisposedException ex5)
			{
				ex = ex5;
			}
			catch (InvalidOperationException ex6)
			{
				ex = ex6;
			}
			catch (LocalizedException ex7)
			{
				ex = ex7;
			}
			catch (RpcException ex8)
			{
				ex = ex8;
			}
			if (ex != null)
			{
				InvokeWithTimeout.Tracer.TraceError<Exception>((long)this.GetHashCode(), "InvokeWithTimeout.RunOperation() caught {0}", ex);
			}
			return ex;
		}

		private const string RegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HA\\InvokeWithTimeout";

		private static readonly Trace Tracer = ExTraceGlobals.ClusterTracer;

		public static readonly TimeSpan InfiniteTimeSpan = TimeSpan.FromMilliseconds(-1.0);

		private static bool s_useSyncCallDetermined;

		private static bool s_useSyncCall;

		private int m_asyncRefCount = 2;
	}
}
