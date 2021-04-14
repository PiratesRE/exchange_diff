using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	internal sealed class Privilege : IDisposable
	{
		public Privilege(string privilegeName)
		{
			if (string.IsNullOrEmpty(privilegeName))
			{
				throw new ArgumentNullException(DiagnosticsResources.InvalidPrivilegeName);
			}
			this.luid = Privilege.LuidFromPrivilege(privilegeName);
		}

		public bool NeedToRevert
		{
			get
			{
				return this.needToRevert;
			}
		}

		public static void RunWithPrivilege(string privilege, bool enabled, PrivilegedHelper helper)
		{
			if (helper == null)
			{
				throw new ArgumentNullException();
			}
			using (Privilege privilege2 = new Privilege(privilege))
			{
				if (enabled)
				{
					privilege2.Enable();
				}
				else
				{
					privilege2.Disable();
				}
				helper();
			}
		}

		public void Enable()
		{
			this.ToggleState(true);
		}

		public void Disable()
		{
			this.ToggleState(false);
		}

		public void Dispose()
		{
			this.Revert();
		}

		public void Revert()
		{
			int num = 0;
			if (!this.currentThread.Equals(Thread.CurrentThread))
			{
				throw new InvalidOperationException(DiagnosticsResources.WrongThread);
			}
			if (!this.NeedToRevert)
			{
				return;
			}
			bool flag = true;
			try
			{
				if (this.stateWasChanged && (this.tlsContents.ReferenceCountValue > 1 || !this.tlsContents.IsImpersonating))
				{
					NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE = default(NativeMethods.TOKEN_PRIVILEGE);
					token_PRIVILEGE.PrivilegeCount = 1U;
					token_PRIVILEGE.Privilege.Luid = this.luid;
					token_PRIVILEGE.Privilege.Attributes = (this.initialState ? 2U : 0U);
					NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE2 = default(NativeMethods.TOKEN_PRIVILEGE);
					uint num2 = 0U;
					if (!NativeMethods.AdjustTokenPrivileges(this.tlsContents.ThreadHandle, false, ref token_PRIVILEGE, (uint)Marshal.SizeOf(token_PRIVILEGE2), ref token_PRIVILEGE2, ref num2))
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
			if (num == 8)
			{
				throw new OutOfMemoryException();
			}
			if (num == 5)
			{
				throw new UnauthorizedAccessException(DiagnosticsResources.UnauthorizedAccess);
			}
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		private static NativeMethods.LUID LuidFromPrivilege(string privilege)
		{
			NativeMethods.LUID luid;
			luid.LowPart = 0U;
			luid.HighPart = 0U;
			try
			{
				Privilege.privilegeLock.AcquireReaderLock(-1);
				if (Privilege.luids.Contains(privilege))
				{
					luid = (NativeMethods.LUID)Privilege.luids[privilege];
					Privilege.privilegeLock.ReleaseReaderLock();
				}
				else
				{
					Privilege.privilegeLock.ReleaseReaderLock();
					if (!NativeMethods.LookupPrivilegeValue(null, privilege, ref luid))
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error == 8)
						{
							throw new OutOfMemoryException();
						}
						if (lastWin32Error == 5)
						{
							throw new UnauthorizedAccessException(DiagnosticsResources.UnauthorizedAccess);
						}
						if (lastWin32Error == 1313)
						{
							throw new ArgumentException(DiagnosticsResources.InvalidPrivilegeName, privilege.ToString());
						}
						throw new Win32Exception(lastWin32Error);
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

		private void ToggleState(bool enable)
		{
			int num = 0;
			if (!this.currentThread.Equals(Thread.CurrentThread))
			{
				throw new InvalidOperationException(DiagnosticsResources.WrongThread);
			}
			if (this.NeedToRevert)
			{
				throw new InvalidOperationException(DiagnosticsResources.RevertPrivilege);
			}
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
				NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE = default(NativeMethods.TOKEN_PRIVILEGE);
				token_PRIVILEGE.PrivilegeCount = 1U;
				token_PRIVILEGE.Privilege.Luid = this.luid;
				token_PRIVILEGE.Privilege.Attributes = (enable ? 2U : 0U);
				NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE2 = default(NativeMethods.TOKEN_PRIVILEGE);
				uint num2 = 0U;
				if (!NativeMethods.AdjustTokenPrivileges(this.tlsContents.ThreadHandle, false, ref token_PRIVILEGE, (uint)Marshal.SizeOf(token_PRIVILEGE2), ref token_PRIVILEGE2, ref num2))
				{
					num = Marshal.GetLastWin32Error();
				}
				else if (1300 == Marshal.GetLastWin32Error())
				{
					num = 1300;
				}
				else
				{
					this.initialState = ((token_PRIVILEGE2.Privilege.Attributes & 2U) != 0U);
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
				throw new UnauthorizedAccessException(DiagnosticsResources.UnauthorizedAccess);
			}
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		private void Reset()
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

		internal const string Audit = "SeAuditPrivilege";

		private static LocalDataStoreSlot tlsSlot = Thread.AllocateDataSlot();

		private static HybridDictionary privileges = new HybridDictionary();

		private static HybridDictionary luids = new HybridDictionary();

		private static ReaderWriterLock privilegeLock = new ReaderWriterLock();

		private readonly Thread currentThread = Thread.CurrentThread;

		private bool needToRevert;

		private bool initialState;

		private bool stateWasChanged;

		private NativeMethods.LUID luid;

		private Privilege.TlsContents tlsContents;

		private sealed class TlsContents : DisposeTrackableBase
		{
			public TlsContents()
			{
				int num = 0;
				int num2 = 0;
				bool flag = true;
				if (Privilege.TlsContents.processHandle.IsInvalid)
				{
					lock (Privilege.TlsContents.syncRoot)
					{
						if (Privilege.TlsContents.processHandle.IsInvalid && !NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), TokenAccessLevels.Duplicate, ref Privilege.TlsContents.processHandle))
						{
							num2 = Marshal.GetLastWin32Error();
							flag = false;
						}
					}
				}
				try
				{
					if (!NativeMethods.OpenThreadToken(NativeMethods.GetCurrentThread(), TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, true, out this.threadHandle))
					{
						if (flag)
						{
							num = Marshal.GetLastWin32Error();
							if (num != 1008)
							{
								flag = false;
							}
							if (flag)
							{
								num = 0;
								if (!NativeMethods.DuplicateTokenEx(Privilege.TlsContents.processHandle, TokenAccessLevels.Impersonate | TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, IntPtr.Zero, SecurityImpersonationLevel.Impersonation, TokenType.Impersonation, ref this.threadHandle))
								{
									num = Marshal.GetLastWin32Error();
									flag = false;
								}
							}
							if (flag && !NativeMethods.SetThreadToken(IntPtr.Zero, this.threadHandle))
							{
								num = Marshal.GetLastWin32Error();
								flag = false;
							}
							if (flag)
							{
								this.impersonating = true;
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
				if (num == 8)
				{
					throw new OutOfMemoryException();
				}
				if (num == 5 || num == 1347)
				{
					throw new UnauthorizedAccessException(DiagnosticsResources.UnauthorizedAccess);
				}
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
			}

			public int ReferenceCountValue
			{
				get
				{
					return this.referenceCount;
				}
			}

			public SafeTokenHandle ThreadHandle
			{
				get
				{
					return this.threadHandle;
				}
			}

			public bool IsImpersonating
			{
				get
				{
					return this.impersonating;
				}
			}

			public void IncrementReferenceCount()
			{
				this.referenceCount++;
			}

			public int DecrementReferenceCount()
			{
				int num = --this.referenceCount;
				if (num == 0)
				{
					this.Dispose();
				}
				return num;
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<Privilege.TlsContents>(this);
			}

			protected override void InternalDispose(bool isDisposing)
			{
				if (this.threadHandle != null && !this.threadHandle.IsInvalid)
				{
					this.threadHandle.Close();
					this.threadHandle = null;
				}
				if (this.impersonating)
				{
					NativeMethods.RevertToSelf();
				}
			}

			private static readonly object syncRoot = new object();

			private static SafeTokenHandle processHandle = SafeTokenHandle.InvalidHandle;

			private int referenceCount = 1;

			private SafeTokenHandle threadHandle = SafeTokenHandle.InvalidHandle;

			private bool impersonating;
		}
	}
}
