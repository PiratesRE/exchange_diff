using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal sealed class RegistryWatcher
	{
		public RegistryWatcher(string keyName, bool watchSubtree)
		{
			this.watchSubtree = watchSubtree;
			this.keyName = keyName;
		}

		public bool IsChanged()
		{
			return this.IsChanged(0, null);
		}

		public bool IsChanged(int timeout, WaitHandle cancelEvent)
		{
			if (this.registryKey.IsInvalid || this.registryKey.IsClosed)
			{
				SafeRegistryHandle safeRegistryHandle;
				DiagnosticsNativeMethods.ErrorCode errorCode = DiagnosticsNativeMethods.RegOpenKeyEx(SafeRegistryHandle.LocalMachine, this.keyName, 0, 131097, out safeRegistryHandle);
				if (DiagnosticsNativeMethods.ErrorCode.FileNotFound == errorCode)
				{
					return false;
				}
				if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
				{
					throw new Win32Exception((int)errorCode);
				}
				try
				{
					this.rwl.EnterWriteLock();
					if (this.registryKey.IsInvalid || this.registryKey.IsClosed)
					{
						this.registryKey = safeRegistryHandle;
						this.notifyEvent = new AutoResetEvent(false);
						errorCode = this.RegisterKeyChangeNotification(this.registryKey, this.watchSubtree, DiagnosticsNativeMethods.RegistryNotifications.ChangeName | DiagnosticsNativeMethods.RegistryNotifications.ChangeAttributes | DiagnosticsNativeMethods.RegistryNotifications.LastSet, this.notifyEvent, true);
						if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
						{
							this.notifyEvent.Dispose();
							this.notifyEvent = null;
							this.registryKey.Dispose();
							throw new Win32Exception((int)errorCode);
						}
						return true;
					}
					else
					{
						safeRegistryHandle.Dispose();
					}
				}
				finally
				{
					this.rwl.ExitWriteLock();
				}
			}
			try
			{
				this.rwl.EnterReadLock();
				bool flag = false;
				if (cancelEvent != null)
				{
					if (WaitHandle.WaitAny(new WaitHandle[]
					{
						this.notifyEvent,
						cancelEvent
					}, timeout) == 0)
					{
						flag = true;
					}
				}
				else if (this.notifyEvent != null)
				{
					flag = this.notifyEvent.WaitOne(timeout);
				}
				if (!flag)
				{
					return false;
				}
			}
			finally
			{
				try
				{
					this.rwl.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			try
			{
				this.rwl.EnterWriteLock();
				DiagnosticsNativeMethods.ErrorCode errorCode = this.RegisterKeyChangeNotification(this.registryKey, this.watchSubtree, DiagnosticsNativeMethods.RegistryNotifications.ChangeName | DiagnosticsNativeMethods.RegistryNotifications.ChangeAttributes | DiagnosticsNativeMethods.RegistryNotifications.LastSet, this.notifyEvent, true);
				if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
				{
					this.notifyEvent.Dispose();
					this.notifyEvent = null;
					this.registryKey.Dispose();
					if (DiagnosticsNativeMethods.ErrorCode.KeyDeleted != errorCode)
					{
						throw new Win32Exception((int)errorCode);
					}
					errorCode = DiagnosticsNativeMethods.RegOpenKeyEx(SafeRegistryHandle.LocalMachine, this.keyName, 0, 131097, out this.registryKey);
					if (DiagnosticsNativeMethods.ErrorCode.FileNotFound != errorCode && errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
					{
						throw new Win32Exception((int)errorCode);
					}
					if (!this.registryKey.IsInvalid && !this.registryKey.IsClosed)
					{
						this.notifyEvent = new AutoResetEvent(false);
						errorCode = this.RegisterKeyChangeNotification(this.registryKey, this.watchSubtree, DiagnosticsNativeMethods.RegistryNotifications.ChangeName | DiagnosticsNativeMethods.RegistryNotifications.ChangeAttributes | DiagnosticsNativeMethods.RegistryNotifications.LastSet, this.notifyEvent, true);
						if (errorCode != DiagnosticsNativeMethods.ErrorCode.Success)
						{
							this.notifyEvent.Dispose();
							this.notifyEvent = null;
							this.registryKey.Dispose();
							throw new Win32Exception((int)errorCode);
						}
					}
				}
			}
			finally
			{
				try
				{
					this.rwl.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return true;
		}

		public string KeyName
		{
			get
			{
				return this.keyName;
			}
		}

		private DiagnosticsNativeMethods.ErrorCode RegisterKeyChangeNotification(SafeRegistryHandle key, bool watchSubtree, DiagnosticsNativeMethods.RegistryNotifications notifyFilter, AutoResetEvent notifyEvent, bool asynchronous)
		{
			return DiagnosticsNativeMethods.RegNotifyChangeKeyValue(this.registryKey, watchSubtree, notifyFilter, notifyEvent.SafeWaitHandle, asynchronous);
		}

		private readonly bool watchSubtree;

		private readonly string keyName;

		private SafeRegistryHandle registryKey = new SafeRegistryHandle();

		private AutoResetEvent notifyEvent;

		private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
	}
}
