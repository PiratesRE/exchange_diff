using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Security
{
	public sealed class SecurityContext : IDisposable
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal SecurityContext()
		{
		}

		internal static SecurityContext FullTrustSecurityContext
		{
			[SecurityCritical]
			get
			{
				if (SecurityContext._fullTrustSC == null)
				{
					SecurityContext._fullTrustSC = SecurityContext.CreateFullTrustSecurityContext();
				}
				return SecurityContext._fullTrustSC;
			}
		}

		internal ExecutionContext ExecutionContext
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			set
			{
				this._executionContext = value;
			}
		}

		internal WindowsIdentity WindowsIdentity
		{
			get
			{
				return this._windowsIdentity;
			}
			set
			{
				this._windowsIdentity = value;
			}
		}

		internal CompressedStack CompressedStack
		{
			get
			{
				return this._compressedStack;
			}
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			set
			{
				this._compressedStack = value;
			}
		}

		public void Dispose()
		{
			if (this._windowsIdentity != null)
			{
				this._windowsIdentity.Dispose();
			}
		}

		[SecurityCritical]
		public static AsyncFlowControl SuppressFlow()
		{
			return SecurityContext.SuppressFlow(SecurityContextDisableFlow.All);
		}

		[SecurityCritical]
		public static AsyncFlowControl SuppressFlowWindowsIdentity()
		{
			return SecurityContext.SuppressFlow(SecurityContextDisableFlow.WI);
		}

		[SecurityCritical]
		internal static AsyncFlowControl SuppressFlow(SecurityContextDisableFlow flags)
		{
			if (SecurityContext.IsFlowSuppressed(flags))
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotSupressFlowMultipleTimes"));
			}
			ExecutionContext mutableExecutionContext = Thread.CurrentThread.GetMutableExecutionContext();
			if (mutableExecutionContext.SecurityContext == null)
			{
				mutableExecutionContext.SecurityContext = new SecurityContext();
			}
			AsyncFlowControl result = default(AsyncFlowControl);
			result.Setup(flags);
			return result;
		}

		[SecuritySafeCritical]
		public static void RestoreFlow()
		{
			SecurityContext securityContext = Thread.CurrentThread.GetMutableExecutionContext().SecurityContext;
			if (securityContext == null || securityContext._disableFlow == SecurityContextDisableFlow.Nothing)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotRestoreUnsupressedFlow"));
			}
			securityContext._disableFlow = SecurityContextDisableFlow.Nothing;
		}

		public static bool IsFlowSuppressed()
		{
			return SecurityContext.IsFlowSuppressed(SecurityContextDisableFlow.All);
		}

		public static bool IsWindowsIdentityFlowSuppressed()
		{
			return SecurityContext._LegacyImpersonationPolicy || SecurityContext.IsFlowSuppressed(SecurityContextDisableFlow.WI);
		}

		[SecuritySafeCritical]
		internal static bool IsFlowSuppressed(SecurityContextDisableFlow flags)
		{
			return Thread.CurrentThread.GetExecutionContextReader().SecurityContext.IsFlowSuppressed(flags);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Run(SecurityContext securityContext, ContextCallback callback, object state)
		{
			if (securityContext == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullContext"));
			}
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMe;
			if (!securityContext.isNewCapture)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotNewCaptureContext"));
			}
			securityContext.isNewCapture = false;
			ExecutionContext.Reader executionContextReader = Thread.CurrentThread.GetExecutionContextReader();
			if (SecurityContext.CurrentlyInDefaultFTSecurityContext(executionContextReader) && securityContext.IsDefaultFTSecurityContext())
			{
				callback(state);
				if (SecurityContext.GetCurrentWI(Thread.CurrentThread.GetExecutionContextReader()) != null)
				{
					WindowsIdentity.SafeRevertToSelf(ref stackCrawlMark);
					return;
				}
			}
			else
			{
				SecurityContext.RunInternal(securityContext, callback, state);
			}
		}

		[SecurityCritical]
		internal static void RunInternal(SecurityContext securityContext, ContextCallback callBack, object state)
		{
			if (SecurityContext.cleanupCode == null)
			{
				SecurityContext.tryCode = new RuntimeHelpers.TryCode(SecurityContext.runTryCode);
				SecurityContext.cleanupCode = new RuntimeHelpers.CleanupCode(SecurityContext.runFinallyCode);
			}
			SecurityContext.SecurityContextRunData userData = new SecurityContext.SecurityContextRunData(securityContext, callBack, state);
			RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(SecurityContext.tryCode, SecurityContext.cleanupCode, userData);
		}

		[SecurityCritical]
		internal static void runTryCode(object userData)
		{
			SecurityContext.SecurityContextRunData securityContextRunData = (SecurityContext.SecurityContextRunData)userData;
			securityContextRunData.scsw = SecurityContext.SetSecurityContext(securityContextRunData.sc, Thread.CurrentThread.GetExecutionContextReader().SecurityContext, true);
			securityContextRunData.callBack(securityContextRunData.state);
		}

		[SecurityCritical]
		[PrePrepareMethod]
		internal static void runFinallyCode(object userData, bool exceptionThrown)
		{
			SecurityContext.SecurityContextRunData securityContextRunData = (SecurityContext.SecurityContextRunData)userData;
			securityContextRunData.scsw.Undo();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static SecurityContextSwitcher SetSecurityContext(SecurityContext sc, SecurityContext.Reader prevSecurityContext, bool modifyCurrentExecutionContext)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return SecurityContext.SetSecurityContext(sc, prevSecurityContext, modifyCurrentExecutionContext, ref stackCrawlMark);
		}

		[SecurityCritical]
		[HandleProcessCorruptedStateExceptions]
		internal static SecurityContextSwitcher SetSecurityContext(SecurityContext sc, SecurityContext.Reader prevSecurityContext, bool modifyCurrentExecutionContext, ref StackCrawlMark stackMark)
		{
			SecurityContextDisableFlow disableFlow = sc._disableFlow;
			sc._disableFlow = SecurityContextDisableFlow.Nothing;
			SecurityContextSwitcher result = default(SecurityContextSwitcher);
			result.currSC = sc;
			result.prevSC = prevSecurityContext;
			if (modifyCurrentExecutionContext)
			{
				ExecutionContext mutableExecutionContext = Thread.CurrentThread.GetMutableExecutionContext();
				result.currEC = mutableExecutionContext;
				mutableExecutionContext.SecurityContext = sc;
			}
			if (sc != null)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					result.wic = null;
					if (!SecurityContext._LegacyImpersonationPolicy)
					{
						if (sc.WindowsIdentity != null)
						{
							result.wic = sc.WindowsIdentity.Impersonate(ref stackMark);
						}
						else if ((disableFlow & SecurityContextDisableFlow.WI) == SecurityContextDisableFlow.Nothing && prevSecurityContext.WindowsIdentity != null)
						{
							result.wic = WindowsIdentity.SafeRevertToSelf(ref stackMark);
						}
					}
					result.cssw = CompressedStack.SetCompressedStack(sc.CompressedStack, prevSecurityContext.CompressedStack);
				}
				catch
				{
					result.UndoNoThrow();
					throw;
				}
			}
			return result;
		}

		[SecuritySafeCritical]
		public SecurityContext CreateCopy()
		{
			if (!this.isNewCapture)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotNewCaptureContext"));
			}
			SecurityContext securityContext = new SecurityContext();
			securityContext.isNewCapture = true;
			securityContext._disableFlow = this._disableFlow;
			if (this.WindowsIdentity != null)
			{
				securityContext._windowsIdentity = new WindowsIdentity(this.WindowsIdentity.AccessToken);
			}
			if (this._compressedStack != null)
			{
				securityContext._compressedStack = this._compressedStack.CreateCopy();
			}
			return securityContext;
		}

		[SecuritySafeCritical]
		internal SecurityContext CreateMutableCopy()
		{
			SecurityContext securityContext = new SecurityContext();
			securityContext._disableFlow = this._disableFlow;
			if (this.WindowsIdentity != null)
			{
				securityContext._windowsIdentity = new WindowsIdentity(this.WindowsIdentity.AccessToken);
			}
			if (this._compressedStack != null)
			{
				securityContext._compressedStack = this._compressedStack.CreateCopy();
			}
			return securityContext;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static SecurityContext Capture()
		{
			if (SecurityContext.IsFlowSuppressed())
			{
				return null;
			}
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			SecurityContext securityContext = SecurityContext.Capture(Thread.CurrentThread.GetExecutionContextReader(), ref stackCrawlMark);
			if (securityContext == null)
			{
				securityContext = SecurityContext.CreateFullTrustSecurityContext();
			}
			return securityContext;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static SecurityContext Capture(ExecutionContext.Reader currThreadEC, ref StackCrawlMark stackMark)
		{
			if (currThreadEC.SecurityContext.IsFlowSuppressed(SecurityContextDisableFlow.All))
			{
				return null;
			}
			if (SecurityContext.CurrentlyInDefaultFTSecurityContext(currThreadEC))
			{
				return null;
			}
			return SecurityContext.CaptureCore(currThreadEC, ref stackMark);
		}

		[SecurityCritical]
		private static SecurityContext CaptureCore(ExecutionContext.Reader currThreadEC, ref StackCrawlMark stackMark)
		{
			SecurityContext securityContext = new SecurityContext();
			securityContext.isNewCapture = true;
			if (!SecurityContext.IsWindowsIdentityFlowSuppressed())
			{
				WindowsIdentity currentWI = SecurityContext.GetCurrentWI(currThreadEC);
				if (currentWI != null)
				{
					securityContext._windowsIdentity = new WindowsIdentity(currentWI.AccessToken);
				}
			}
			else
			{
				securityContext._disableFlow = SecurityContextDisableFlow.WI;
			}
			securityContext.CompressedStack = CompressedStack.GetCompressedStack(ref stackMark);
			return securityContext;
		}

		[SecurityCritical]
		internal static SecurityContext CreateFullTrustSecurityContext()
		{
			SecurityContext securityContext = new SecurityContext();
			securityContext.isNewCapture = true;
			if (SecurityContext.IsWindowsIdentityFlowSuppressed())
			{
				securityContext._disableFlow = SecurityContextDisableFlow.WI;
			}
			securityContext.CompressedStack = new CompressedStack(null);
			return securityContext;
		}

		internal static bool AlwaysFlowImpersonationPolicy
		{
			get
			{
				return SecurityContext._alwaysFlowImpersonationPolicy;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static WindowsIdentity GetCurrentWI(ExecutionContext.Reader threadEC)
		{
			return SecurityContext.GetCurrentWI(threadEC, SecurityContext._alwaysFlowImpersonationPolicy);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static WindowsIdentity GetCurrentWI(ExecutionContext.Reader threadEC, bool cachedAlwaysFlowImpersonationPolicy)
		{
			if (cachedAlwaysFlowImpersonationPolicy)
			{
				return WindowsIdentity.GetCurrentInternal(TokenAccessLevels.MaximumAllowed, true);
			}
			return threadEC.SecurityContext.WindowsIdentity;
		}

		[SecurityCritical]
		internal static void RestoreCurrentWI(ExecutionContext.Reader currentEC, ExecutionContext.Reader prevEC, WindowsIdentity targetWI, bool cachedAlwaysFlowImpersonationPolicy)
		{
			if (cachedAlwaysFlowImpersonationPolicy || prevEC.SecurityContext.WindowsIdentity != targetWI)
			{
				SecurityContext.RestoreCurrentWIInternal(targetWI);
			}
		}

		[SecurityCritical]
		private static void RestoreCurrentWIInternal(WindowsIdentity targetWI)
		{
			int num = Win32.RevertToSelf();
			if (num < 0)
			{
				Environment.FailFast(Win32Native.GetMessage(num));
			}
			if (targetWI != null)
			{
				SafeAccessTokenHandle accessToken = targetWI.AccessToken;
				if (accessToken != null && !accessToken.IsInvalid)
				{
					num = Win32.ImpersonateLoggedOnUser(accessToken);
					if (num < 0)
					{
						Environment.FailFast(Win32Native.GetMessage(num));
					}
				}
			}
		}

		[SecurityCritical]
		internal bool IsDefaultFTSecurityContext()
		{
			return this.WindowsIdentity == null && (this.CompressedStack == null || this.CompressedStack.CompressedStackHandle == null);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool CurrentlyInDefaultFTSecurityContext(ExecutionContext.Reader threadEC)
		{
			return SecurityContext.IsDefaultThreadSecurityInfo() && SecurityContext.GetCurrentWI(threadEC) == null;
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern WindowsImpersonationFlowMode GetImpersonationFlowMode();

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsDefaultThreadSecurityInfo();

		private static bool _LegacyImpersonationPolicy = SecurityContext.GetImpersonationFlowMode() == WindowsImpersonationFlowMode.IMP_NOFLOW;

		private static bool _alwaysFlowImpersonationPolicy = SecurityContext.GetImpersonationFlowMode() == WindowsImpersonationFlowMode.IMP_ALWAYSFLOW;

		private ExecutionContext _executionContext;

		private volatile WindowsIdentity _windowsIdentity;

		private volatile CompressedStack _compressedStack;

		private static volatile SecurityContext _fullTrustSC;

		internal volatile bool isNewCapture;

		internal volatile SecurityContextDisableFlow _disableFlow;

		internal static volatile RuntimeHelpers.TryCode tryCode;

		internal static volatile RuntimeHelpers.CleanupCode cleanupCode;

		internal struct Reader
		{
			public Reader(SecurityContext sc)
			{
				this.m_sc = sc;
			}

			public SecurityContext DangerousGetRawSecurityContext()
			{
				return this.m_sc;
			}

			public bool IsNull
			{
				get
				{
					return this.m_sc == null;
				}
			}

			public bool IsSame(SecurityContext sc)
			{
				return this.m_sc == sc;
			}

			public bool IsSame(SecurityContext.Reader sc)
			{
				return this.m_sc == sc.m_sc;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsFlowSuppressed(SecurityContextDisableFlow flags)
			{
				return this.m_sc != null && (this.m_sc._disableFlow & flags) == flags;
			}

			public CompressedStack CompressedStack
			{
				get
				{
					if (!this.IsNull)
					{
						return this.m_sc.CompressedStack;
					}
					return null;
				}
			}

			public WindowsIdentity WindowsIdentity
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					if (!this.IsNull)
					{
						return this.m_sc.WindowsIdentity;
					}
					return null;
				}
			}

			private SecurityContext m_sc;
		}

		internal class SecurityContextRunData
		{
			internal SecurityContextRunData(SecurityContext securityContext, ContextCallback cb, object state)
			{
				this.sc = securityContext;
				this.callBack = cb;
				this.state = state;
				this.scsw = default(SecurityContextSwitcher);
			}

			internal SecurityContext sc;

			internal ContextCallback callBack;

			internal object state;

			internal SecurityContextSwitcher scsw;
		}
	}
}
