using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class WatsonOnUnhandledExceptionDispatch : BaseObject, IRpcDispatch, IDisposable
	{
		public WatsonOnUnhandledExceptionDispatch(IRpcDispatch innerDispatch) : this(innerDispatch, null, RpcDispatch.BuildDefaultConnectAuxOutBuffer())
		{
		}

		public WatsonOnUnhandledExceptionDispatch(IRpcDispatch innerDispatch, Action<bool, Exception> exceptionHandlerAction, ArraySegment<byte> defaultConnectAuxOutBuffer)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.innerDispatch = innerDispatch;
				this.exceptionHandlerAction = exceptionHandlerAction;
				this.defaultConnectAuxOutBuffer = defaultConnectAuxOutBuffer;
				disposeGuard.Success();
			}
		}

		public static bool TryHandleGreyExceptionGuardFilterNoCustomHandlerAction(object exceptionObject)
		{
			Exception ex = exceptionObject as Exception;
			if (!WatsonOnUnhandledExceptionDispatch.IsExceptionInteresting(ex))
			{
				return false;
			}
			bool flag = !GrayException.IsGrayException(ex) && !WatsonOnUnhandledExceptionDispatch.IsLocalGrayException(ex);
			WatsonOnUnhandledExceptionDispatch.HandleGreyExceptionTakeWatsonActions(flag, ex);
			return !flag;
		}

		public bool TryHandleGreyExceptionGuardFilter(object exceptionObject)
		{
			Exception ex = exceptionObject as Exception;
			if (!WatsonOnUnhandledExceptionDispatch.IsExceptionInteresting(ex))
			{
				return false;
			}
			bool flag = !GrayException.IsGrayException(ex) && !WatsonOnUnhandledExceptionDispatch.IsLocalGrayException(ex);
			this.exceptionHandlerAction(flag, ex);
			return !flag;
		}

		public static bool IsUnderWatsonSuiteTests
		{
			get
			{
				bool result = false;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(4083559741U, ref result);
				return result;
			}
		}

		public static bool IsUnderWatsonThrottlingTests
		{
			get
			{
				bool result = false;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3278253373U, ref result);
				return result;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<WatsonOnUnhandledExceptionDispatch>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.innerDispatch);
			base.InternalDispose();
		}

		private static void HandleGreyExceptionTakeWatsonActions(bool shouldTerminateProcess, Exception exception)
		{
			try
			{
				if (ExWatson.KillProcessAfterWatson || WatsonOnUnhandledExceptionDispatch.IsUnderWatsonSuiteTests)
				{
					ReportOptions reportOptions = ReportOptions.DoNotFreezeThreads;
					if (shouldTerminateProcess)
					{
						reportOptions |= ReportOptions.ReportTerminateAfterSend;
					}
					ProtocolLog.LogWatsonFailure(shouldTerminateProcess, exception);
					ExWatson.SendReport(exception, reportOptions, null);
				}
				else
				{
					ProtocolLog.LogWatsonFailure(shouldTerminateProcess, exception);
					ExTraceGlobals.UnhandledExceptionTracer.TraceInformation<Exception>(0, Activity.TraceId, "ExAEDbg will handle the crash after creation of a Watson report", exception);
					ExWatson.SendReportAndCrashOnAnotherThread(exception);
				}
			}
			finally
			{
				if (shouldTerminateProcess)
				{
					WatsonOnUnhandledExceptionDispatch.KillCurrentProcess();
				}
			}
		}

		private static void DoNothing(object unused)
		{
		}

		private static bool IsExceptionInteresting(object exGeneric)
		{
			Exception ex = exGeneric as Exception;
			if (ex == null)
			{
				return true;
			}
			bool flag = ex is RpcServiceException;
			return !flag;
		}

		private static void KillCurrentProcess()
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3546688829U);
			try
			{
				Process.GetCurrentProcess().Kill();
			}
			catch (Win32Exception)
			{
			}
			Environment.Exit(-559034355);
		}

		private static bool IsLocalGrayException(Exception exception)
		{
			return exception is OverflowException || (exception is MapiPermanentException || exception is MapiRetryableException) || (exception is DataValidationException || exception is DataSourceOperationException || exception is DataSourceTransientException);
		}

		private static void Guard(TryDelegate body, FilterDelegate exceptionFilter)
		{
			ILUtil.DoTryFilterCatch(body, exceptionFilter, new CatchDelegate(null, (UIntPtr)ldftn(DoNothing)));
		}

		private FilterDelegate GetExceptionFilter()
		{
			if (this.exceptionHandlerAction != null)
			{
				return new FilterDelegate(this, (UIntPtr)ldftn(TryHandleGreyExceptionGuardFilter));
			}
			return new FilterDelegate(null, (UIntPtr)ldftn(TryHandleGreyExceptionGuardFilterNoCustomHandlerAction));
		}

		int IRpcDispatch.MaximumConnections
		{
			get
			{
				WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass1 CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass1();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.localResult = 0;
				WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.get_MaximumConnections>b__0)), this.GetExceptionFilter());
				return CS$<>8__locals1.localResult;
			}
		}

		void IRpcDispatch.ReportBytesRead(long bytesRead, long uncompressedBytesRead)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass4 CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass4();
			CS$<>8__locals1.bytesRead = bytesRead;
			CS$<>8__locals1.uncompressedBytesRead = uncompressedBytesRead;
			CS$<>8__locals1.<>4__this = this;
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.ReportBytesRead>b__3)), this.GetExceptionFilter());
		}

		void IRpcDispatch.ReportBytesWritten(long bytesWritten, long uncompressedBytesWritten)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass7 CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass7();
			CS$<>8__locals1.bytesWritten = bytesWritten;
			CS$<>8__locals1.uncompressedBytesWritten = uncompressedBytesWritten;
			CS$<>8__locals1.<>4__this = this;
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.ReportBytesWritten>b__6)), this.GetExceptionFilter());
		}

		int IRpcDispatch.Connect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, out IntPtr contextHandle, string userDn, int flags, int connectionModulus, int codePage, int stringLocale, int sortLocale, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, IStandardBudget budget)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClassa CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClassa();
			CS$<>8__locals1.protocolRequestInfo = protocolRequestInfo;
			CS$<>8__locals1.clientBinding = clientBinding;
			CS$<>8__locals1.userDn = userDn;
			CS$<>8__locals1.flags = flags;
			CS$<>8__locals1.connectionModulus = connectionModulus;
			CS$<>8__locals1.codePage = codePage;
			CS$<>8__locals1.stringLocale = stringLocale;
			CS$<>8__locals1.sortLocale = sortLocale;
			CS$<>8__locals1.clientVersion = clientVersion;
			CS$<>8__locals1.auxIn = auxIn;
			CS$<>8__locals1.auxOut = auxOut;
			CS$<>8__locals1.budget = budget;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = null;
			CS$<>8__locals1.localContextHandle = IntPtr.Zero;
			CS$<>8__locals1.localDnPrefix = null;
			CS$<>8__locals1.localDisplayName = null;
			CS$<>8__locals1.localPollsMax = TimeSpan.Zero;
			CS$<>8__locals1.localRetryCount = 0;
			CS$<>8__locals1.localRetryDelay = TimeSpan.Zero;
			CS$<>8__locals1.localSizeAuxOut = 0;
			if (CS$<>8__locals1.auxIn.Array == null)
			{
				throw new InvalidOperationException("Invalid auxIn ArraySegment; Array can't be null");
			}
			if (CS$<>8__locals1.auxOut.Array == null)
			{
				throw new InvalidOperationException("Invalid auxOut ArraySegment; Array can't be null");
			}
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.Connect>b__9)), this.GetExceptionFilter());
			if ((CS$<>8__locals1.localErrorCode == null || CS$<>8__locals1.localSizeAuxOut == 0) && this.defaultConnectAuxOutBuffer.Count <= CS$<>8__locals1.auxOut.Count)
			{
				Buffer.BlockCopy(this.defaultConnectAuxOutBuffer.Array, this.defaultConnectAuxOutBuffer.Offset, CS$<>8__locals1.auxOut.Array, CS$<>8__locals1.auxOut.Offset, this.defaultConnectAuxOutBuffer.Count);
				CS$<>8__locals1.localSizeAuxOut = this.defaultConnectAuxOutBuffer.Count;
			}
			contextHandle = CS$<>8__locals1.localContextHandle;
			pollsMax = CS$<>8__locals1.localPollsMax;
			retryCount = CS$<>8__locals1.localRetryCount;
			retryDelay = CS$<>8__locals1.localRetryDelay;
			dnPrefix = CS$<>8__locals1.localDnPrefix;
			displayName = CS$<>8__locals1.localDisplayName;
			sizeAuxOut = CS$<>8__locals1.localSizeAuxOut;
			int? localErrorCode = CS$<>8__locals1.localErrorCode;
			if (localErrorCode == null)
			{
				return -2147467259;
			}
			return localErrorCode.GetValueOrDefault();
		}

		int IRpcDispatch.Disconnect(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, bool rundown)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClassd CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClassd();
			CS$<>8__locals1.protocolRequestInfo = protocolRequestInfo;
			CS$<>8__locals1.rundown = rundown;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.localContextHandle = contextHandle;
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.Disconnect>b__c)), this.GetExceptionFilter());
			contextHandle = CS$<>8__locals1.localContextHandle;
			return CS$<>8__locals1.localErrorCode;
		}

		int IRpcDispatch.Execute(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, IList<ArraySegment<byte>> ropInArray, ArraySegment<byte> ropOut, out int sizeRopOut, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, bool isFake, out byte[] fakeOut)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass10 CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass10();
			CS$<>8__locals1.protocolRequestInfo = protocolRequestInfo;
			CS$<>8__locals1.ropInArray = ropInArray;
			CS$<>8__locals1.ropOut = ropOut;
			CS$<>8__locals1.auxIn = auxIn;
			CS$<>8__locals1.auxOut = auxOut;
			CS$<>8__locals1.isFake = isFake;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.localContextHandle = contextHandle;
			CS$<>8__locals1.localSizeRopOut = 0;
			CS$<>8__locals1.localSizeAuxOut = 0;
			CS$<>8__locals1.localFakeOut = null;
			if (CS$<>8__locals1.ropOut.Array == null)
			{
				throw new InvalidOperationException("Invalid ropOut ArraySegment; Array can't be null");
			}
			if (CS$<>8__locals1.auxIn.Array == null)
			{
				throw new InvalidOperationException("Invalid auxIn ArraySegment; Array can't be null");
			}
			if (CS$<>8__locals1.auxOut.Array == null)
			{
				throw new InvalidOperationException("Invalid auxOut ArraySegment; Array can't be null");
			}
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.Execute>b__f)), this.GetExceptionFilter());
			contextHandle = CS$<>8__locals1.localContextHandle;
			sizeRopOut = CS$<>8__locals1.localSizeRopOut;
			sizeAuxOut = CS$<>8__locals1.localSizeAuxOut;
			fakeOut = CS$<>8__locals1.localFakeOut;
			return CS$<>8__locals1.localErrorCode;
		}

		int IRpcDispatch.NotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, out IntPtr asynchronousContextHandle)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass13 CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass13();
			CS$<>8__locals1.protocolRequestInfo = protocolRequestInfo;
			CS$<>8__locals1.contextHandle = contextHandle;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			CS$<>8__locals1.localAsynchronousContextHandle = IntPtr.Zero;
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.NotificationConnect>b__12)), this.GetExceptionFilter());
			asynchronousContextHandle = CS$<>8__locals1.localAsynchronousContextHandle;
			return CS$<>8__locals1.localErrorCode;
		}

		int IRpcDispatch.Dummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass16 CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass16();
			CS$<>8__locals1.protocolRequestInfo = protocolRequestInfo;
			CS$<>8__locals1.clientBinding = clientBinding;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = -2147467259;
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.Dummy>b__15)), this.GetExceptionFilter());
			return CS$<>8__locals1.localErrorCode;
		}

		void IRpcDispatch.NotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr asynchronousContextHandle, uint flags, Action<bool, int> completion)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass19 CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass19();
			CS$<>8__locals1.protocolRequestInfo = protocolRequestInfo;
			CS$<>8__locals1.asynchronousContextHandle = asynchronousContextHandle;
			CS$<>8__locals1.flags = flags;
			CS$<>8__locals1.completion = completion;
			CS$<>8__locals1.<>4__this = this;
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.NotificationWait>b__18)), this.GetExceptionFilter());
		}

		void IRpcDispatch.DroppedConnection(IntPtr asynchronousContextHandle)
		{
			WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass1c CS$<>8__locals1 = new WatsonOnUnhandledExceptionDispatch.<>c__DisplayClass1c();
			CS$<>8__locals1.asynchronousContextHandle = asynchronousContextHandle;
			CS$<>8__locals1.<>4__this = this;
			WatsonOnUnhandledExceptionDispatch.Guard(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<Microsoft.Exchange.RpcClientAccess.Server.IRpcDispatch.DroppedConnection>b__1b)), this.GetExceptionFilter());
		}

		private const uint WatsonSuiteSetWatsonTestInProgress = 4083559741U;

		private const uint WatsonSuiteSetWatsonThrottlingTestInProgress = 3278253373U;

		private const uint WatsonSuiteProcessKilled = 3546688829U;

		private readonly IRpcDispatch innerDispatch;

		private readonly Action<bool, Exception> exceptionHandlerAction;

		private readonly ArraySegment<byte> defaultConnectAuxOutBuffer;
	}
}
