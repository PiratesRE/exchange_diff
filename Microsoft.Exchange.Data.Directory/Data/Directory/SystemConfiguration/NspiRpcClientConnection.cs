using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Nspi;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class NspiRpcClientConnection : IDisposeTrackable, IDisposable
	{
		private static NspiRpcClientConnectionPerformanceCountersInstance PerfCounterInstance
		{
			get
			{
				if (NspiRpcClientConnection.perfCounterInstance == null)
				{
					lock (NspiRpcClientConnection.perfCounterInstanceInitLock)
					{
						if (NspiRpcClientConnection.perfCounterInstance == null)
						{
							using (Process currentProcess = Process.GetCurrentProcess())
							{
								string instanceName = string.Format("{0} ({1})", currentProcess.MainModule.ModuleName, currentProcess.Id);
								NspiRpcClientConnection.perfCounterInstance = NspiRpcClientConnectionPerformanceCounters.GetInstance(instanceName);
							}
						}
					}
				}
				return NspiRpcClientConnection.perfCounterInstance;
			}
		}

		public NspiRpcClient RpcClient
		{
			get
			{
				return this.nspiRpcClient;
			}
		}

		private NspiRpcClientConnection()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public static NspiRpcClientConnection GetNspiRpcClientConnection(string domainController)
		{
			NspiRpcClientConnection nspiRpcClientConnection = new NspiRpcClientConnection();
			int hashCode = nspiRpcClientConnection.GetHashCode();
			int num = 0;
			while (!nspiRpcClientConnection.bound)
			{
				try
				{
					NspiRpcClientConnection.TraceDebug(hashCode, "Creating NspiRpcClient and attempting bind to domain controller {0}", new object[]
					{
						domainController
					});
					nspiRpcClientConnection.nspiRpcClient = new NspiRpcClient(domainController, "ncacn_ip_tcp", null);
					using (SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(36))
					{
						using (SafeRpcMemoryHandle safeRpcMemoryHandle2 = new SafeRpcMemoryHandle(16))
						{
							IntPtr ptr = safeRpcMemoryHandle.DangerousGetHandle();
							Marshal.WriteInt32(ptr, 0, 0);
							Marshal.WriteInt32(ptr, 4, 0);
							Marshal.WriteInt32(ptr, 8, 0);
							Marshal.WriteInt32(ptr, 12, 0);
							Marshal.WriteInt32(ptr, 16, 0);
							Marshal.WriteInt32(ptr, 20, 0);
							Marshal.WriteInt32(ptr, 24, 1252);
							Marshal.WriteInt32(ptr, 28, 1033);
							Marshal.WriteInt32(ptr, 32, 1033);
							int num2 = nspiRpcClientConnection.nspiRpcClient.Bind(0, safeRpcMemoryHandle.DangerousGetHandle(), safeRpcMemoryHandle2.DangerousGetHandle());
							if (num2 != 0)
							{
								NspiRpcClientConnection.TraceError(hashCode, "Bind returned non-zero SCODE {0}", new object[]
								{
									num2
								});
								throw new NspiFailureException(num2);
							}
							NspiRpcClientConnection.TraceDebug(hashCode, "Bind to domain controller {0} succeeded", new object[]
							{
								domainController
							});
							nspiRpcClientConnection.bound = true;
							NspiRpcClientConnection.PerfCounterInstance.NumberOfOpenConnections.Increment();
						}
					}
				}
				catch (RpcException ex)
				{
					num++;
					if (ex.ErrorCode != 1753 && ex.ErrorCode != 1727)
					{
						NspiRpcClientConnection.TraceError(hashCode, "Caught RpcException \"{0}\" with error code {1}.  We will not retry the bind.", new object[]
						{
							ex.Message,
							ex.ErrorCode
						});
						throw new ADOperationException(DirectoryStrings.NspiRpcError(ex.Message), ex);
					}
					if (num >= 3)
					{
						NspiRpcClientConnection.TraceError(hashCode, "Caught RpcException \"{0}\" with error code {1}.  Out of retries; giving up.", new object[]
						{
							ex.Message,
							ex.ErrorCode
						});
						throw new ADTransientException(DirectoryStrings.NspiRpcError(ex.Message), ex);
					}
					NspiRpcClientConnection.TraceWarning(hashCode, "Caught RpcException \"{0}\" with error code {1}.  We will retry the bind; this is retry {2}.", new object[]
					{
						ex.Message,
						ex.ErrorCode,
						num
					});
					Thread.Sleep(1000);
				}
				finally
				{
					if (nspiRpcClientConnection != null && !nspiRpcClientConnection.bound)
					{
						NspiRpcClientConnection.TraceDebug(hashCode, "Disposing the NspiRpcClient because we did not successfully bind", new object[0]);
						nspiRpcClientConnection.nspiRpcClient.Dispose();
						nspiRpcClientConnection.nspiRpcClient = null;
					}
				}
			}
			return nspiRpcClientConnection;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiRpcClientConnection>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public virtual void Dispose()
		{
			NspiRpcClientConnection.TraceDebug(this.GetHashCode(), "Disposing by calling Dispose()", new object[0]);
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
					if (this.nspiRpcClient != null && this.bound)
					{
						try
						{
							this.nspiRpcClient.Unbind();
							this.bound = false;
							NspiRpcClientConnection.PerfCounterInstance.NumberOfOpenConnections.Decrement();
						}
						catch (RpcException)
						{
						}
						this.nspiRpcClient.Dispose();
						this.nspiRpcClient = null;
					}
				}
				this.disposed = true;
			}
		}

		private static void TraceDebug(int hashcode, string trace, params object[] args)
		{
			ExTraceGlobals.NspiRpcClientConnectionTracer.TraceDebug((long)hashcode, trace, args);
		}

		private static void TraceWarning(int hashcode, string trace, params object[] args)
		{
			ExTraceGlobals.NspiRpcClientConnectionTracer.TraceWarning((long)hashcode, trace, args);
		}

		private static void TraceError(int hashcode, string trace, params object[] args)
		{
			ExTraceGlobals.NspiRpcClientConnectionTracer.TraceError((long)hashcode, trace, args);
		}

		public const string DefaultProtocolSequence = "ncacn_ip_tcp";

		public const int DefaultCodePage = 1252;

		public const int DefaultTemplateLocale = 1033;

		public const int DefaultSortLocale = 1033;

		private const int RPC_S_CALL_FAILED_DNE = 1727;

		private const int EPT_S_NOT_REGISTERED = 1753;

		private const int MaxConnectionRetries = 3;

		private const int ConnectionRetryInterval = 1000;

		private static NspiRpcClientConnectionPerformanceCountersInstance perfCounterInstance;

		private static object perfCounterInstanceInitLock = new object();

		private DisposeTracker disposeTracker;

		private bool disposed;

		private bool bound;

		private NspiRpcClient nspiRpcClient;
	}
}
