using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Security.AccessControl
{
	internal sealed class Privilege
	{
		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		private static Win32Native.LUID LuidFromPrivilege(string privilege)
		{
			Win32Native.LUID luid;
			luid.LowPart = 0U;
			luid.HighPart = 0U;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				Privilege.privilegeLock.AcquireReaderLock(-1);
				if (Privilege.luids.Contains(privilege))
				{
					luid = (Win32Native.LUID)Privilege.luids[privilege];
					Privilege.privilegeLock.ReleaseReaderLock();
				}
				else
				{
					Privilege.privilegeLock.ReleaseReaderLock();
					if (!Win32Native.LookupPrivilegeValue(null, privilege, ref luid))
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error == 8)
						{
							throw new OutOfMemoryException();
						}
						if (lastWin32Error == 5)
						{
							throw new UnauthorizedAccessException();
						}
						if (lastWin32Error == 1313)
						{
							throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPrivilegeName", new object[]
							{
								privilege
							}));
						}
						throw new InvalidOperationException();
					}
					else
					{
						Privilege.privilegeLock.AcquireWriterLock(-1);
					}
				}
			}
			finally
			{
				if (Privilege.privilegeLock.IsReaderLockHeld)
				{
					Privilege.privilegeLock.ReleaseReaderLock();
				}
				if (Privilege.privilegeLock.IsWriterLockHeld)
				{
					if (!Privilege.luids.Contains(privilege))
					{
						Privilege.luids[privilege] = luid;
						Privilege.privileges[luid] = privilege;
					}
					Privilege.privilegeLock.ReleaseWriterLock();
				}
			}
			return luid;
		}

		[SecurityCritical]
		public Privilege(string privilegeName)
		{
			if (privilegeName == null)
			{
				throw new ArgumentNullException("privilegeName");
			}
			this.luid = Privilege.LuidFromPrivilege(privilegeName);
		}

		[SecuritySafeCritical]
		~Privilege()
		{
			if (this.needToRevert)
			{
				this.Revert();
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void Enable()
		{
			this.ToggleState(true);
		}

		public bool NeedToRevert
		{
			get
			{
				return this.needToRevert;
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		private void ToggleState(bool enable)
		{
			int num = 0;
			if (!this.currentThread.Equals(Thread.CurrentThread))
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_MustBeSameThread"));
			}
			if (this.needToRevert)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_MustRevertPrivilege"));
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				try
				{
					this.tlsContents = (Thread.GetData(Privilege.tlsSlot) as Privilege.TlsContents);
					if (this.tlsContents == null)
					{
						this.tlsContents = new Privilege.TlsContents();
						Thread.SetData(Privilege.tlsSlot, this.tlsContents);
					}
					else
					{
						this.tlsContents.IncrementReferenceCount();
					}
					Win32Native.TOKEN_PRIVILEGE token_PRIVILEGE = default(Win32Native.TOKEN_PRIVILEGE);
					token_PRIVILEGE.PrivilegeCount = 1U;
					token_PRIVILEGE.Privilege.Luid = this.luid;
					token_PRIVILEGE.Privilege.Attributes = (enable ? 2U : 0U);
					Win32Native.TOKEN_PRIVILEGE token_PRIVILEGE2 = default(Win32Native.TOKEN_PRIVILEGE);
					uint num2 = 0U;
					if (!Win32Native.AdjustTokenPrivileges(this.tlsContents.ThreadHandle, false, ref token_PRIVILEGE, (uint)Marshal.SizeOf<Win32Native.TOKEN_PRIVILEGE>(token_PRIVILEGE2), ref token_PRIVILEGE2, ref num2))
					{
						num = Marshal.GetLastWin32Error();
					}
					else if (1300 == Marshal.GetLastWin32Error())
					{
						num = 1300;
					}
					else
					{
						this.initialState = ((token_PRIVILEGE2.Privilege.Attributes & 2U) > 0U);
						this.stateWasChanged = (this.initialState != enable);
						this.needToRevert = (this.tlsContents.IsImpersonating || this.stateWasChanged);
					}
				}
				finally
				{
					if (!this.needToRevert)
					{
						this.Reset();
					}
				}
			}
			if (num == 1300)
			{
				throw new PrivilegeNotHeldException(Privilege.privileges[this.luid] as string);
			}
			if (num == 8)
			{
				throw new OutOfMemoryException();
			}
			if (num == 5 || num == 1347)
			{
				throw new UnauthorizedAccessException();
			}
			if (num != 0)
			{
				throw new InvalidOperationException();
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void Revert()
		{
			int num = 0;
			if (!this.currentThread.Equals(Thread.CurrentThread))
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_MustBeSameThread"));
			}
			if (!this.NeedToRevert)
			{
				return;
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				bool flag = true;
				try
				{
					if (this.stateWasChanged && (this.tlsContents.ReferenceCountValue > 1 || !this.tlsContents.IsImpersonating))
					{
						Win32Native.TOKEN_PRIVILEGE token_PRIVILEGE = default(Win32Native.TOKEN_PRIVILEGE);
						token_PRIVILEGE.PrivilegeCount = 1U;
						token_PRIVILEGE.Privilege.Luid = this.luid;
						token_PRIVILEGE.Privilege.Attributes = (this.initialState ? 2U : 0U);
						Win32Native.TOKEN_PRIVILEGE structure = default(Win32Native.TOKEN_PRIVILEGE);
						uint num2 = 0U;
						if (!Win32Native.AdjustTokenPrivileges(this.tlsContents.ThreadHandle, false, ref token_PRIVILEGE, (uint)Marshal.SizeOf<Win32Native.TOKEN_PRIVILEGE>(structure), ref structure, ref num2))
						{
							num = Marshal.GetLastWin32Error();
							flag = false;
						}
					}
				}
				finally
				{
					if (flag)
					{
						this.Reset();
					}
				}
			}
			if (num == 8)
			{
				throw new OutOfMemoryException();
			}
			if (num == 5)
			{
				throw new UnauthorizedAccessException();
			}
			if (num != 0)
			{
				throw new InvalidOperationException();
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private void Reset()
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				this.stateWasChanged = false;
				this.initialState = false;
				this.needToRevert = false;
				if (this.tlsContents != null && this.tlsContents.DecrementReferenceCount() == 0)
				{
					this.tlsContents = null;
					Thread.SetData(Privilege.tlsSlot, null);
				}
			}
		}

		private static LocalDataStoreSlot tlsSlot = Thread.AllocateDataSlot();

		private static Hashtable privileges = new Hashtable();

		private static Hashtable luids = new Hashtable();

		private static ReaderWriterLock privilegeLock = new ReaderWriterLock();

		private bool needToRevert;

		private bool initialState;

		private bool stateWasChanged;

		[SecurityCritical]
		private Win32Native.LUID luid;

		private readonly Thread currentThread = Thread.CurrentThread;

		private Privilege.TlsContents tlsContents;

		public const string CreateToken = "SeCreateTokenPrivilege";

		public const string AssignPrimaryToken = "SeAssignPrimaryTokenPrivilege";

		public const string LockMemory = "SeLockMemoryPrivilege";

		public const string IncreaseQuota = "SeIncreaseQuotaPrivilege";

		public const string UnsolicitedInput = "SeUnsolicitedInputPrivilege";

		public const string MachineAccount = "SeMachineAccountPrivilege";

		public const string TrustedComputingBase = "SeTcbPrivilege";

		public const string Security = "SeSecurityPrivilege";

		public const string TakeOwnership = "SeTakeOwnershipPrivilege";

		public const string LoadDriver = "SeLoadDriverPrivilege";

		public const string SystemProfile = "SeSystemProfilePrivilege";

		public const string SystemTime = "SeSystemtimePrivilege";

		public const string ProfileSingleProcess = "SeProfileSingleProcessPrivilege";

		public const string IncreaseBasePriority = "SeIncreaseBasePriorityPrivilege";

		public const string CreatePageFile = "SeCreatePagefilePrivilege";

		public const string CreatePermanent = "SeCreatePermanentPrivilege";

		public const string Backup = "SeBackupPrivilege";

		public const string Restore = "SeRestorePrivilege";

		public const string Shutdown = "SeShutdownPrivilege";

		public const string Debug = "SeDebugPrivilege";

		public const string Audit = "SeAuditPrivilege";

		public const string SystemEnvironment = "SeSystemEnvironmentPrivilege";

		public const string ChangeNotify = "SeChangeNotifyPrivilege";

		public const string RemoteShutdown = "SeRemoteShutdownPrivilege";

		public const string Undock = "SeUndockPrivilege";

		public const string SyncAgent = "SeSyncAgentPrivilege";

		public const string EnableDelegation = "SeEnableDelegationPrivilege";

		public const string ManageVolume = "SeManageVolumePrivilege";

		public const string Impersonate = "SeImpersonatePrivilege";

		public const string CreateGlobal = "SeCreateGlobalPrivilege";

		public const string TrustedCredentialManagerAccess = "SeTrustedCredManAccessPrivilege";

		public const string ReserveProcessor = "SeReserveProcessorPrivilege";

		private sealed class TlsContents : IDisposable
		{
			[SecurityCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			public TlsContents()
			{
				int num = 0;
				int num2 = 0;
				bool flag = true;
				if (Privilege.TlsContents.processHandle.IsInvalid)
				{
					object obj = Privilege.TlsContents.syncRoot;
					lock (obj)
					{
						if (Privilege.TlsContents.processHandle.IsInvalid)
						{
							SafeAccessTokenHandle safeAccessTokenHandle;
							if (!Win32Native.OpenProcessToken(Win32Native.GetCurrentProcess(), TokenAccessLevels.Duplicate, out safeAccessTokenHandle))
							{
								num2 = Marshal.GetLastWin32Error();
								flag = false;
							}
							Privilege.TlsContents.processHandle = safeAccessTokenHandle;
						}
					}
				}
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					try
					{
						SafeAccessTokenHandle safeAccessTokenHandle2 = this.threadHandle;
						num = Win32.OpenThreadToken(TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, WinSecurityContext.Process, out this.threadHandle);
						num &= 2147024895;
						if (num != 0)
						{
							if (flag)
							{
								this.threadHandle = safeAccessTokenHandle2;
								if (num != 1008)
								{
									flag = false;
								}
								if (flag)
								{
									num = 0;
									if (!Win32Native.DuplicateTokenEx(Privilege.TlsContents.processHandle, TokenAccessLevels.Impersonate | TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, IntPtr.Zero, Win32Native.SECURITY_IMPERSONATION_LEVEL.Impersonation, System.Security.Principal.TokenType.TokenImpersonation, ref this.threadHandle))
									{
										num = Marshal.GetLastWin32Error();
										flag = false;
									}
								}
								if (flag)
								{
									num = Win32.SetThreadToken(this.threadHandle);
									num &= 2147024895;
									if (num != 0)
									{
										flag = false;
									}
								}
								if (flag)
								{
									this.isImpersonating = true;
								}
							}
							else
							{
								num = num2;
							}
						}
						else
						{
							flag = true;
						}
					}
					finally
					{
						if (!flag)
						{
							this.Dispose();
						}
					}
				}
				if (num == 8)
				{
					throw new OutOfMemoryException();
				}
				if (num == 5 || num == 1347)
				{
					throw new UnauthorizedAccessException();
				}
				if (num != 0)
				{
					throw new InvalidOperationException();
				}
			}

			[SecuritySafeCritical]
			~TlsContents()
			{
				if (!this.disposed)
				{
					this.Dispose(false);
				}
			}

			[SecuritySafeCritical]
			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			[SecurityCritical]
			private void Dispose(bool disposing)
			{
				if (this.disposed)
				{
					return;
				}
				if (disposing && this.threadHandle != null)
				{
					this.threadHandle.Dispose();
					this.threadHandle = null;
				}
				if (this.isImpersonating)
				{
					Win32.RevertToSelf();
				}
				this.disposed = true;
			}

			public void IncrementReferenceCount()
			{
				this.referenceCount++;
			}

			[SecurityCritical]
			public int DecrementReferenceCount()
			{
				int num = this.referenceCount - 1;
				this.referenceCount = num;
				int num2 = num;
				if (num2 == 0)
				{
					this.Dispose();
				}
				return num2;
			}

			public int ReferenceCountValue
			{
				get
				{
					return this.referenceCount;
				}
			}

			public SafeAccessTokenHandle ThreadHandle
			{
				[SecurityCritical]
				get
				{
					return this.threadHandle;
				}
			}

			public bool IsImpersonating
			{
				get
				{
					return this.isImpersonating;
				}
			}

			private bool disposed;

			private int referenceCount = 1;

			[SecurityCritical]
			private SafeAccessTokenHandle threadHandle = new SafeAccessTokenHandle(IntPtr.Zero);

			private bool isImpersonating;

			[SecurityCritical]
			private static volatile SafeAccessTokenHandle processHandle = new SafeAccessTokenHandle(IntPtr.Zero);

			private static readonly object syncRoot = new object();
		}
	}
}
