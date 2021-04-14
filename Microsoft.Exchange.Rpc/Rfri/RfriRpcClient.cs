using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Exchange.Rpc.Rfri
{
	internal class RfriRpcClient : RpcClientBase
	{
		private unsafe _RPC_ASYNC_STATE* AsyncState
		{
			get
			{
				return (_RPC_ASYNC_STATE*)this.asyncState.DangerousGetHandle().ToPointer();
			}
		}

		public RfriRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, string instanceName, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate, string certificateSubjectName)
		{
			string servicePrincipalName;
			if (string.IsNullOrEmpty(certificateSubjectName))
			{
				servicePrincipalName = string.Format("exchangeRFR/{0}", instanceName);
			}
			else
			{
				servicePrincipalName = string.Format("msstd:{0}", instanceName);
			}
			base..ctor(machineName, proxyServer, protocolSequence, servicePrincipalName, true, nc, httpAuthenticationScheme, authenticationService, true, ignoreInvalidServerCertificate, certificateSubjectName);
			try
			{
				this.Initialize();
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public RfriRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, string instanceName, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate) : base(machineName, proxyServer, protocolSequence, string.Format("exchangeRFR/{0}", instanceName), true, nc, httpAuthenticationScheme, authenticationService, true, ignoreInvalidServerCertificate, false)
		{
			try
			{
				this.Initialize();
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public RfriRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate) : base(machineName, proxyServer, protocolSequence, true, nc, httpAuthenticationScheme, authenticationService, true, ignoreInvalidServerCertificate, false)
		{
			try
			{
				this.Initialize();
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public RfriRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, string instanceName) : base(machineName, proxyServer, protocolSequence, string.Format("exchangeRFR/{0}", instanceName), true, nc, httpAuthenticationScheme, authenticationService, true, false, false)
		{
			try
			{
				this.Initialize();
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public RfriRpcClient(string machineName, string proxyServer, string protocolSequence, [MarshalAs(UnmanagedType.U1)] bool useEncryption, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService) : base(machineName, proxyServer, protocolSequence, string.Format("exchangeRFR/{0}", machineName), useEncryption, nc, httpAuthenticationScheme, authenticationService, true, false, false)
		{
			try
			{
				this.Initialize();
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public RfriRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc) : base(machineName, proxyServer, protocolSequence, nc, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, true)
		{
			try
			{
				this.Initialize();
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public RfriRpcClient(string machineName, string protocolSequence, NetworkCredential nc) : base(machineName, protocolSequence, nc, AuthenticationService.Negotiate)
		{
			try
			{
				this.Initialize();
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		private void ~RfriRpcClient()
		{
			IDisposable disposable = this.asyncState;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			this.asyncState = null;
			IDisposable disposable2 = this.rpcCompleteEvent;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
			this.rpcCompleteEvent = null;
		}

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		public int TotalRpcCounter
		{
			get
			{
				return this.totalRpcCounter;
			}
		}

		public TimeSpan TotalRpcTime
		{
			get
			{
				return this.totalRpcTime;
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int GetNewDSA(string userDN, out string server)
		{
			byte* ptr = null;
			SafeMarshalHGlobalHandle safeMarshalHGlobalHandle = new SafeMarshalHGlobalHandle(Marshal.StringToHGlobalAnsi(userDN));
			base.ResetRetryCounter();
			int result;
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeMarshalHGlobalHandle.DangerousGetHandle();
					<Module>.cli_RfrGetNewDSA(this.AsyncState, base.BindingHandle, 0, (byte*)intPtr.ToPointer(), null, &ptr);
					result = this.WaitForCompletion();
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					DateTime utcNow3 = DateTime.UtcNow;
					TimeSpan ts2 = utcNow3 - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.CallCancelled | RpcRetryType.ServerBusy | RpcRetryType.ServerUnavailable | RpcRetryType.AccessDenied) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_RfrGetNewDSA");
				}
				finally
				{
					if (ptr == null)
					{
						server = null;
					}
					else
					{
						server = new string((sbyte*)ptr);
						<Module>.MIDL_user_free((void*)ptr);
					}
				}
				break;
			}
			if (safeMarshalHGlobalHandle != null)
			{
				((IDisposable)safeMarshalHGlobalHandle).Dispose();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int GetFQDNFromLegacyDN(string serverDN, out string serverFQDN)
		{
			byte* ptr = null;
			SafeMarshalHGlobalHandle safeMarshalHGlobalHandle = new SafeMarshalHGlobalHandle(Marshal.StringToHGlobalAnsi(serverDN));
			void* ptr2 = safeMarshalHGlobalHandle.DangerousGetHandle().ToPointer();
			void* ptr3 = ptr2;
			if (*(sbyte*)ptr2 != 0)
			{
				do
				{
					ptr3 = (void*)((byte*)ptr3 + 1L);
				}
				while (*(sbyte*)ptr3 != 0);
			}
			int num = (int)(ptr3 - ptr2 + 1);
			base.ResetRetryCounter();
			int result;
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeMarshalHGlobalHandle.DangerousGetHandle();
					<Module>.cli_RfrGetFQDNFromLegacyDN(this.AsyncState, base.BindingHandle, 0, num, (byte*)intPtr.ToPointer(), &ptr);
					result = this.WaitForCompletion();
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					DateTime utcNow3 = DateTime.UtcNow;
					TimeSpan ts2 = utcNow3 - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.CallCancelled | RpcRetryType.ServerBusy | RpcRetryType.ServerUnavailable | RpcRetryType.AccessDenied) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_RfrGetFQDNFromLegacyDN");
				}
				finally
				{
					if (ptr == null)
					{
						serverFQDN = null;
					}
					else
					{
						serverFQDN = new string((sbyte*)ptr);
						<Module>.MIDL_user_free((void*)ptr);
					}
				}
				break;
			}
			if (safeMarshalHGlobalHandle != null)
			{
				((IDisposable)safeMarshalHGlobalHandle).Dispose();
			}
			return result;
		}

		private unsafe void Initialize()
		{
			this.totalRpcCounter = 0;
			this.totalRpcTime = TimeSpan.Zero;
			TimeSpan timeSpan = TimeSpan.FromMinutes(5.0);
			this.timeout = timeSpan;
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(112);
			this.asyncState = safeRpcMemoryHandle;
			int num = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer(), 112U);
			if (num != null)
			{
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "RpcAsyncInitializeHandle");
			}
			*(long*)((byte*)this.asyncState.DangerousGetHandle().ToPointer() + 24L) = 0L;
			*(int*)((byte*)this.asyncState.DangerousGetHandle().ToPointer() + 44L) = 1;
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			this.rpcCompleteEvent = autoResetEvent;
			IntPtr value = autoResetEvent.SafeWaitHandle.DangerousGetHandle();
			*(long*)((byte*)this.asyncState.DangerousGetHandle().ToPointer() + 48L) = (void*)value;
		}

		private unsafe int WaitForCompletion()
		{
			bool flag = false;
			if (!this.rpcCompleteEvent.WaitOne(this.timeout))
			{
				<Module>.RpcAsyncCancelCall((_RPC_ASYNC_STATE*)this.asyncState.DangerousGetHandle().ToPointer(), 1);
				this.rpcCompleteEvent.WaitOne();
				flag = true;
			}
			int result;
			int num = <Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)this.asyncState.DangerousGetHandle().ToPointer(), (void*)(&result));
			if (num != null)
			{
				<Module>.RpcRaiseException(flag ? 1460 : num);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					this.~RfriRpcClient();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private AutoResetEvent rpcCompleteEvent;

		private SafeRpcMemoryHandle asyncState;

		private TimeSpan timeout;

		private int totalRpcCounter;

		private TimeSpan totalRpcTime;
	}
}
