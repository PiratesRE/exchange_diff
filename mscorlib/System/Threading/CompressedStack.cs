using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Threading
{
	[Serializable]
	public sealed class CompressedStack : ISerializable
	{
		internal bool CanSkipEvaluation
		{
			get
			{
				return this.m_canSkipEvaluation;
			}
			private set
			{
				this.m_canSkipEvaluation = value;
			}
		}

		internal PermissionListSet PLS
		{
			get
			{
				return this.m_pls;
			}
		}

		[SecurityCritical]
		internal CompressedStack(SafeCompressedStackHandle csHandle)
		{
			this.m_csHandle = csHandle;
		}

		[SecurityCritical]
		private CompressedStack(SafeCompressedStackHandle csHandle, PermissionListSet pls)
		{
			this.m_csHandle = csHandle;
			this.m_pls = pls;
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.CompleteConstruction(null);
			info.AddValue("PLS", this.m_pls);
		}

		private CompressedStack(SerializationInfo info, StreamingContext context)
		{
			this.m_pls = (PermissionListSet)info.GetValue("PLS", typeof(PermissionListSet));
		}

		internal SafeCompressedStackHandle CompressedStackHandle
		{
			[SecurityCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.m_csHandle;
			}
			[SecurityCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			private set
			{
				this.m_csHandle = value;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static CompressedStack GetCompressedStack()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return CompressedStack.GetCompressedStack(ref stackCrawlMark);
		}

		[SecurityCritical]
		internal static CompressedStack GetCompressedStack(ref StackCrawlMark stackMark)
		{
			CompressedStack innerCS = null;
			CompressedStack compressedStack;
			if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				compressedStack = new CompressedStack(null);
				compressedStack.CanSkipEvaluation = true;
			}
			else if (CodeAccessSecurityEngine.AllDomainsHomogeneousWithNoStackModifiers())
			{
				compressedStack = new CompressedStack(CompressedStack.GetDelayedCompressedStack(ref stackMark, false));
				compressedStack.m_pls = PermissionListSet.CreateCompressedState_HG();
			}
			else
			{
				compressedStack = new CompressedStack(null);
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					compressedStack.CompressedStackHandle = CompressedStack.GetDelayedCompressedStack(ref stackMark, true);
					if (compressedStack.CompressedStackHandle != null && CompressedStack.IsImmediateCompletionCandidate(compressedStack.CompressedStackHandle, out innerCS))
					{
						try
						{
							compressedStack.CompleteConstruction(innerCS);
						}
						finally
						{
							CompressedStack.DestroyDCSList(compressedStack.CompressedStackHandle);
						}
					}
				}
			}
			return compressedStack;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static CompressedStack Capture()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return CompressedStack.GetCompressedStack(ref stackCrawlMark);
		}

		[SecurityCritical]
		public static void Run(CompressedStack compressedStack, ContextCallback callback, object state)
		{
			if (compressedStack == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamNull"), "compressedStack");
			}
			if (CompressedStack.cleanupCode == null)
			{
				CompressedStack.tryCode = new RuntimeHelpers.TryCode(CompressedStack.runTryCode);
				CompressedStack.cleanupCode = new RuntimeHelpers.CleanupCode(CompressedStack.runFinallyCode);
			}
			CompressedStack.CompressedStackRunData userData = new CompressedStack.CompressedStackRunData(compressedStack, callback, state);
			RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(CompressedStack.tryCode, CompressedStack.cleanupCode, userData);
		}

		[SecurityCritical]
		internal static void runTryCode(object userData)
		{
			CompressedStack.CompressedStackRunData compressedStackRunData = (CompressedStack.CompressedStackRunData)userData;
			compressedStackRunData.cssw = CompressedStack.SetCompressedStack(compressedStackRunData.cs, CompressedStack.GetCompressedStackThread());
			compressedStackRunData.callBack(compressedStackRunData.state);
		}

		[SecurityCritical]
		[PrePrepareMethod]
		internal static void runFinallyCode(object userData, bool exceptionThrown)
		{
			CompressedStack.CompressedStackRunData compressedStackRunData = (CompressedStack.CompressedStackRunData)userData;
			compressedStackRunData.cssw.Undo();
		}

		[SecurityCritical]
		[HandleProcessCorruptedStateExceptions]
		internal static CompressedStackSwitcher SetCompressedStack(CompressedStack cs, CompressedStack prevCS)
		{
			CompressedStackSwitcher result = default(CompressedStackSwitcher);
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					CompressedStack.SetCompressedStackThread(cs);
					result.prev_CS = prevCS;
					result.curr_CS = cs;
					result.prev_ADStack = CompressedStack.SetAppDomainStack(cs);
				}
			}
			catch
			{
				result.UndoNoThrow();
				throw;
			}
			return result;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public CompressedStack CreateCopy()
		{
			return new CompressedStack(this.m_csHandle, this.m_pls);
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static IntPtr SetAppDomainStack(CompressedStack cs)
		{
			return Thread.CurrentThread.SetAppDomainStack((cs == null) ? null : cs.CompressedStackHandle);
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static void RestoreAppDomainStack(IntPtr appDomainStack)
		{
			Thread.CurrentThread.RestoreAppDomainStack(appDomainStack);
		}

		[SecurityCritical]
		internal static CompressedStack GetCompressedStackThread()
		{
			return Thread.CurrentThread.GetExecutionContextReader().SecurityContext.CompressedStack;
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void SetCompressedStackThread(CompressedStack cs)
		{
			Thread currentThread = Thread.CurrentThread;
			if (currentThread.GetExecutionContextReader().SecurityContext.CompressedStack != cs)
			{
				ExecutionContext mutableExecutionContext = currentThread.GetMutableExecutionContext();
				if (mutableExecutionContext.SecurityContext != null)
				{
					mutableExecutionContext.SecurityContext.CompressedStack = cs;
					return;
				}
				if (cs != null)
				{
					mutableExecutionContext.SecurityContext = new SecurityContext
					{
						CompressedStack = cs
					};
				}
			}
		}

		[SecurityCritical]
		internal bool CheckDemand(CodeAccessPermission demand, PermissionToken permToken, RuntimeMethodHandleInternal rmh)
		{
			this.CompleteConstruction(null);
			if (this.PLS == null)
			{
				return false;
			}
			this.PLS.CheckDemand(demand, permToken, rmh);
			return false;
		}

		[SecurityCritical]
		internal bool CheckDemandNoHalt(CodeAccessPermission demand, PermissionToken permToken, RuntimeMethodHandleInternal rmh)
		{
			this.CompleteConstruction(null);
			return this.PLS == null || this.PLS.CheckDemand(demand, permToken, rmh);
		}

		[SecurityCritical]
		internal bool CheckSetDemand(PermissionSet pset, RuntimeMethodHandleInternal rmh)
		{
			this.CompleteConstruction(null);
			return this.PLS != null && this.PLS.CheckSetDemand(pset, rmh);
		}

		[SecurityCritical]
		internal bool CheckSetDemandWithModificationNoHalt(PermissionSet pset, out PermissionSet alteredDemandSet, RuntimeMethodHandleInternal rmh)
		{
			alteredDemandSet = null;
			this.CompleteConstruction(null);
			return this.PLS == null || this.PLS.CheckSetDemandWithModification(pset, out alteredDemandSet, rmh);
		}

		[SecurityCritical]
		internal void DemandFlagsOrGrantSet(int flags, PermissionSet grantSet)
		{
			this.CompleteConstruction(null);
			if (this.PLS == null)
			{
				return;
			}
			this.PLS.DemandFlagsOrGrantSet(flags, grantSet);
		}

		[SecurityCritical]
		internal void GetZoneAndOrigin(ArrayList zoneList, ArrayList originList, PermissionToken zoneToken, PermissionToken originToken)
		{
			this.CompleteConstruction(null);
			if (this.PLS != null)
			{
				this.PLS.GetZoneAndOrigin(zoneList, originList, zoneToken, originToken);
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal void CompleteConstruction(CompressedStack innerCS)
		{
			if (this.PLS != null)
			{
				return;
			}
			PermissionListSet pls = PermissionListSet.CreateCompressedState(this, innerCS);
			lock (this)
			{
				if (this.PLS == null)
				{
					this.m_pls = pls;
				}
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern SafeCompressedStackHandle GetDelayedCompressedStack(ref StackCrawlMark stackMark, bool walkStack);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyDelayedCompressedStack(IntPtr unmanagedCompressedStack);

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyDCSList(SafeCompressedStackHandle compressedStack);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetDCSCount(SafeCompressedStackHandle compressedStack);

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsImmediateCompletionCandidate(SafeCompressedStackHandle compressedStack, out CompressedStack innerCS);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern DomainCompressedStack GetDomainCompressedStack(SafeCompressedStackHandle compressedStack, int index);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetHomogeneousPLS(PermissionListSet hgPLS);

		private volatile PermissionListSet m_pls;

		[SecurityCritical]
		private volatile SafeCompressedStackHandle m_csHandle;

		private bool m_canSkipEvaluation;

		internal static volatile RuntimeHelpers.TryCode tryCode;

		internal static volatile RuntimeHelpers.CleanupCode cleanupCode;

		internal class CompressedStackRunData
		{
			internal CompressedStackRunData(CompressedStack cs, ContextCallback cb, object state)
			{
				this.cs = cs;
				this.callBack = cb;
				this.state = state;
				this.cssw = default(CompressedStackSwitcher);
			}

			internal CompressedStack cs;

			internal ContextCallback callBack;

			internal object state;

			internal CompressedStackSwitcher cssw;
		}
	}
}
