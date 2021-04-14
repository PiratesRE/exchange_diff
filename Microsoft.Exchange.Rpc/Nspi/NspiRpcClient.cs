using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Exchange.Rpc.Nspi
{
	internal class NspiRpcClient : RpcClientBase
	{
		private unsafe _RPC_ASYNC_STATE* AsyncState
		{
			get
			{
				return (_RPC_ASYNC_STATE*)this.asyncState.DangerousGetHandle().ToPointer();
			}
		}

		public NspiRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, string instanceName, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate, string certificateSubjectName, [MarshalAs(UnmanagedType.U1)] bool useEncryption)
		{
			string servicePrincipalName;
			if (string.IsNullOrEmpty(certificateSubjectName))
			{
				servicePrincipalName = string.Format("exchangeAB/{0}", instanceName);
			}
			else
			{
				servicePrincipalName = string.Format("msstd:{0}", instanceName);
			}
			base..ctor(machineName, proxyServer, protocolSequence, servicePrincipalName, useEncryption, nc, httpAuthenticationScheme, authenticationService, true, ignoreInvalidServerCertificate, certificateSubjectName);
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

		public NspiRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, string instanceName, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate, string certificateSubjectName)
		{
			string servicePrincipalName;
			if (string.IsNullOrEmpty(certificateSubjectName))
			{
				servicePrincipalName = string.Format("exchangeAB/{0}", instanceName);
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

		public NspiRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, string instanceName, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate) : base(machineName, proxyServer, protocolSequence, string.Format("exchangeAB/{0}", instanceName), true, nc, httpAuthenticationScheme, authenticationService, true, ignoreInvalidServerCertificate, false)
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

		public NspiRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, string instanceName) : base(machineName, proxyServer, protocolSequence, string.Format("exchangeAB/{0}", instanceName), true, nc, httpAuthenticationScheme, authenticationService, true)
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

		public NspiRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate) : base(machineName, proxyServer, protocolSequence, true, nc, httpAuthenticationScheme, authenticationService, true, ignoreInvalidServerCertificate, false)
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

		public NspiRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService) : base(machineName, proxyServer, protocolSequence, nc, httpAuthenticationScheme, authenticationService, true)
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

		public NspiRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc) : base(machineName, proxyServer, protocolSequence, nc, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, true)
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

		public NspiRpcClient(string machineName, string protocolSequence, NetworkCredential nc) : base(machineName, protocolSequence, nc, AuthenticationService.Negotiate)
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

		public NspiRpcClient(string machineName, NetworkCredential nc) : base(machineName, nc)
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

		public NspiRpcClient(string machineName) : base(machineName)
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
		public unsafe int Bind(uint flags, IntPtr stat, IntPtr guid)
		{
			int num = -2147467259;
			void* ptr = null;
			ref void* void*& = ref ptr;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					try
					{
						<Module>.cli_NspiBind(this.AsyncState, base.BindingHandle, flags, (__MIDL_nspi_0002*)stat.ToPointer(), (__MIDL_nspi_0001*)guid.ToPointer(), ref void*&);
						num = this.WaitForCompletion<int>();
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
						<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiBind");
					}
					if (num == null)
					{
						this.contextHandle = ptr;
					}
				}
				finally
				{
				}
				break;
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int Unbind()
		{
			int result = 2;
			ref void* void*& = ref this.contextHandle;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiUnbind(this.AsyncState, ref void*&, 0);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiUnbind");
				}
				finally
				{
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int GetHierarchyInfo(uint ulFlags, IntPtr stat, ref uint lpVersion, out SafeRpcMemoryHandle pHierTabRows)
		{
			int result = -2147467259;
			uint num = lpVersion;
			_SRowSet_r* ptr = null;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiGetHierarchyInfo(this.AsyncState, this.contextHandle, ulFlags, (__MIDL_nspi_0002*)stat.ToPointer(), &num, &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiGetHierarchyInfo");
				}
				finally
				{
				}
				break;
			}
			lpVersion = num;
			if (ptr == null)
			{
				pHierTabRows = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				pHierTabRows = new SafeSRowSetHandle(handle);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int GetMatches(IntPtr stat, IntPtr restriction, IntPtr propnames, int maxRows, int[] propTags, out int[] mids, out SafeRpcMemoryHandle rowset)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			_SPropTagArray_r* ptr2 = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(propTags, false);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
					<Module>.cli_NspiGetMatches(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), null, 0, (_SRestriction_r*)restriction.ToPointer(), null, maxRows, &ptr2, (_SPropTagArray_r*)intPtr.ToPointer(), &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiGetMatches");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			if (ptr == null)
			{
				rowset = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				rowset = new SafeSRowSetHandle(handle);
			}
			if (ptr2 == null)
			{
				mids = null;
			}
			else
			{
				mids = NspiHelper.ConvertPropTagArrayToIntArray(ptr2);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int QueryRows(uint flags, IntPtr stat, int[] mids, int count, int[] propTags, out SafeRpcMemoryHandle rowset)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(propTags, false);
			SafeRpcMemoryHandle safeRpcMemoryHandle2 = new SafeRpcMemoryHandle();
			int num = 0;
			if (mids != null)
			{
				int num2 = mids.Length;
				if (num2 != 0)
				{
					num = num2;
					safeRpcMemoryHandle2.Allocate((ulong)((long)num * 4L));
					int* value = (int*)safeRpcMemoryHandle2.DangerousGetHandle().ToPointer();
					IntPtr destination = new IntPtr((void*)value);
					Marshal.Copy(mids, 0, destination, num);
				}
			}
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
					IntPtr intPtr2 = safeRpcMemoryHandle2.DangerousGetHandle();
					<Module>.cli_NspiQueryRows(this.AsyncState, this.contextHandle, flags, (__MIDL_nspi_0002*)stat.ToPointer(), num, (uint*)intPtr2.ToPointer(), count, (_SPropTagArray_r*)intPtr.ToPointer(), &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiQueryRows");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			if (safeRpcMemoryHandle2 != null)
			{
				((IDisposable)safeRpcMemoryHandle2).Dispose();
			}
			if (ptr == null)
			{
				rowset = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				rowset = new SafeSRowSetHandle(handle);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int DNToEph(string[] DNs, out int[] mids)
		{
			int result = -2147467259;
			SafeStringArrayHandle safeStringArrayHandle = new SafeStringArrayHandle(DNs, true);
			_SPropTagArray_r* ptr = null;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeStringArrayHandle.DangerousGetHandle();
					<Module>.cli_NspiDNToEph(this.AsyncState, this.contextHandle, 0, (_StringsArray*)intPtr.ToPointer(), &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiDNToEph");
				}
				finally
				{
				}
				break;
			}
			if (ptr == null)
			{
				mids = null;
			}
			else
			{
				mids = NspiHelper.ConvertPropTagArrayToIntArray(ptr);
			}
			if (safeStringArrayHandle != null)
			{
				((IDisposable)safeStringArrayHandle).Dispose();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int ResolveNames(IntPtr stat, int[] propTags, byte[][] names, out int[] results, out SafeRpcMemoryHandle rowset)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			_SPropTagArray_r* ptr2 = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(propTags, false);
			SafeStringArrayHandle safeStringArrayHandle = new SafeStringArrayHandle(names);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeStringArrayHandle.DangerousGetHandle();
					IntPtr intPtr2 = safeRpcMemoryHandle.DangerousGetHandle();
					<Module>.cli_NspiResolveNames(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), (_SPropTagArray_r*)intPtr2.ToPointer(), (_StringsArray*)intPtr.ToPointer(), &ptr2, &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiResolveNames");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			if (safeStringArrayHandle != null)
			{
				((IDisposable)safeStringArrayHandle).Dispose();
			}
			if (ptr == null)
			{
				rowset = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				rowset = new SafeSRowSetHandle(handle);
			}
			if (ptr2 == null)
			{
				results = null;
			}
			else
			{
				results = NspiHelper.ConvertPropTagArrayToIntArray(ptr2);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int ResolveNames(IntPtr stat, int[] propTags, string[] names, out int[] results, out SafeRpcMemoryHandle rowset)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			_SPropTagArray_r* ptr2 = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(propTags, false);
			SafeStringArrayHandle safeStringArrayHandle = new SafeStringArrayHandle(names, false);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeStringArrayHandle.DangerousGetHandle();
					IntPtr intPtr2 = safeRpcMemoryHandle.DangerousGetHandle();
					<Module>.cli_NspiResolveNamesW(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), (_SPropTagArray_r*)intPtr2.ToPointer(), (_WStringsArray*)intPtr.ToPointer(), &ptr2, &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiResolveNamesW");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			if (safeStringArrayHandle != null)
			{
				((IDisposable)safeStringArrayHandle).Dispose();
			}
			if (ptr == null)
			{
				rowset = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				rowset = new SafeSRowSetHandle(handle);
			}
			if (ptr2 == null)
			{
				results = null;
			}
			else
			{
				results = NspiHelper.ConvertPropTagArrayToIntArray(ptr2);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int GetProps(uint flags, IntPtr stat, int[] propTags, out SafeRpcMemoryHandle row)
		{
			int result = -2147467259;
			_SRow_r* ptr = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(propTags, false);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
					<Module>.cli_NspiGetProps(this.AsyncState, this.contextHandle, flags, (__MIDL_nspi_0002*)stat.ToPointer(), (_SPropTagArray_r*)intPtr.ToPointer(), &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiGetProps");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			if (ptr == null)
			{
				row = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				row = new SafeSRowHandle(handle);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int ResortRestriction(IntPtr stat, int[] inMidlist, out int[] outMidlist)
		{
			int result = -2147467259;
			_SPropTagArray_r* ptr = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(inMidlist, false);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
					<Module>.cli_NspiResortRestriction(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), (_SPropTagArray_r*)intPtr.ToPointer(), &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiResortRestriction");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			if (ptr == null)
			{
				outMidlist = null;
			}
			else
			{
				outMidlist = NspiHelper.ConvertPropTagArrayToIntArray(ptr);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int UpdateStat(IntPtr stat)
		{
			int result = -2147467259;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiUpdateStat(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), null);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiUpdateStat");
				}
				finally
				{
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int UpdateStat(IntPtr stat, out int returnedDelta)
		{
			int result = -2147467259;
			int num = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiUpdateStat(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), &num);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiUpdateStat");
				}
				finally
				{
				}
				break;
			}
			returnedDelta = num;
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int GetPropList(uint flags, int mid, int codepage, out int[] propTags)
		{
			int result = -2147467259;
			_SPropTagArray_r* ptr = null;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiGetPropList(this.AsyncState, this.contextHandle, flags, mid, codepage, &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_GetPropList");
				}
				finally
				{
				}
				break;
			}
			if (ptr == null)
			{
				propTags = null;
			}
			else
			{
				propTags = NspiHelper.ConvertPropTagArrayToIntArray(ptr);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int CompareMids(IntPtr stat, int mid1, int mid2, out int result)
		{
			int result2 = -2147467259;
			int num = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiCompareDNTs(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), mid1, mid2, &num);
					result2 = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiCompareDNTs");
				}
				finally
				{
				}
				break;
			}
			result = num;
			return result2;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int ModProps(IntPtr stat, int[] propTags, IntPtr row)
		{
			int result = -2147467259;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(propTags, false);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
					<Module>.cli_NspiModProps(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), (_SPropTagArray_r*)intPtr.ToPointer(), (_SRow_r*)row.ToPointer());
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiModProps");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int GetTemplateInfo(uint flags, uint type, string dn, uint codePage, uint localeId, out SafeRpcMemoryHandle row)
		{
			int result = -2147467259;
			_SRow_r* ptr = null;
			IntPtr hglobal = Marshal.StringToHGlobalAnsi(dn);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiGetTemplateInfo(this.AsyncState, this.contextHandle, flags, type, (sbyte*)hglobal.ToPointer(), codePage, localeId, &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiGetTemplateInfo");
				}
				finally
				{
					Marshal.FreeHGlobal(hglobal);
				}
				break;
			}
			if (ptr == null)
			{
				row = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				row = new SafeSRowHandle(handle);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int QueryColumns(int clientFlags, out int[] columns)
		{
			int result = -2147467259;
			_SPropTagArray_r* ptr = null;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					<Module>.cli_NspiQueryColumns(this.AsyncState, this.contextHandle, 0, clientFlags, &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiQueryColumns");
				}
				finally
				{
				}
				break;
			}
			if (ptr == null)
			{
				columns = null;
			}
			else
			{
				columns = NspiHelper.ConvertPropTagArrayToIntArray(ptr);
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int ModLinkAtt(uint flags, int propTag, int mid, byte[][] entryIDs)
		{
			int result = -2147467259;
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeByteArraysHandle(entryIDs);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
					<Module>.cli_NspiModLinkAtt(this.AsyncState, this.contextHandle, flags, propTag, mid, (_SBinaryArray_r*)intPtr.ToPointer());
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiModLinkAtt");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int SeekEntries(IntPtr stat, IntPtr propValue, int[] mids, int[] propTags, out SafeRpcMemoryHandle rowset)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			SafeRpcMemoryHandle safeRpcMemoryHandle = NspiHelper.ConvertIntArrayToPropTagArray(propTags, false);
			SafeRpcMemoryHandle safeRpcMemoryHandle2 = NspiHelper.ConvertIntArrayToPropTagArray(mids, false);
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					IntPtr intPtr = safeRpcMemoryHandle.DangerousGetHandle();
					IntPtr intPtr2 = safeRpcMemoryHandle2.DangerousGetHandle();
					<Module>.cli_NspiSeekEntries(this.AsyncState, this.contextHandle, 0, (__MIDL_nspi_0002*)stat.ToPointer(), (_SPropValue_r*)propValue.ToPointer(), (_SPropTagArray_r*)intPtr2.ToPointer(), (_SPropTagArray_r*)intPtr.ToPointer(), &ptr);
					result = this.WaitForCompletion<int>();
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
					if (base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "cli_NspiSeekEntries");
				}
				finally
				{
				}
				break;
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			if (safeRpcMemoryHandle2 != null)
			{
				((IDisposable)safeRpcMemoryHandle2).Dispose();
			}
			if (ptr == null)
			{
				rowset = null;
			}
			else
			{
				IntPtr handle = new IntPtr((void*)ptr);
				rowset = new SafeSRowSetHandle(handle);
			}
			return result;
		}

		private void ~NspiRpcClient()
		{
			IDisposable disposable = this.asyncState;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			IDisposable disposable2 = this.rpcCompleteEvent;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}

		private unsafe void Initialize()
		{
			this.totalRpcCounter = 0;
			this.totalRpcTime = TimeSpan.Zero;
			this.contextHandle = null;
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

		private unsafe type WaitForCompletion<type>()
		{
			bool flag = false;
			if (!this.rpcCompleteEvent.WaitOne(this.timeout))
			{
				<Module>.RpcAsyncCancelCall((_RPC_ASYNC_STATE*)this.asyncState.DangerousGetHandle().ToPointer(), 1);
				this.rpcCompleteEvent.WaitOne();
				flag = true;
			}
			type result;
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
					this.~NspiRpcClient();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private unsafe void* contextHandle;

		private AutoResetEvent rpcCompleteEvent;

		private SafeRpcMemoryHandle asyncState;

		private TimeSpan timeout;

		private int totalRpcCounter;

		private TimeSpan totalRpcTime;
	}
}
