using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public sealed class RegistryKey : MarshalByRefObject, IDisposable
	{
		[SecurityCritical]
		private RegistryKey(SafeRegistryHandle hkey, bool writable, RegistryView view) : this(hkey, writable, false, false, false, view)
		{
		}

		[SecurityCritical]
		private RegistryKey(SafeRegistryHandle hkey, bool writable, bool systemkey, bool remoteKey, bool isPerfData, RegistryView view)
		{
			this.hkey = hkey;
			this.keyName = "";
			this.remoteKey = remoteKey;
			this.regView = view;
			if (systemkey)
			{
				this.state |= 2;
			}
			if (writable)
			{
				this.state |= 4;
			}
			if (isPerfData)
			{
				this.state |= 8;
			}
			RegistryKey.ValidateKeyView(view);
		}

		public void Close()
		{
			this.Dispose(true);
		}

		[SecuritySafeCritical]
		private void Dispose(bool disposing)
		{
			if (this.hkey != null)
			{
				if (!this.IsSystemKey())
				{
					try
					{
						this.hkey.Dispose();
						return;
					}
					catch (IOException)
					{
						return;
					}
					finally
					{
						this.hkey = null;
					}
				}
				if (disposing && this.IsPerfDataKey())
				{
					SafeRegistryHandle.RegCloseKey(RegistryKey.HKEY_PERFORMANCE_DATA);
				}
			}
		}

		[SecuritySafeCritical]
		public void Flush()
		{
			if (this.hkey != null && this.IsDirty())
			{
				Win32Native.RegFlushKey(this.hkey);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public RegistryKey CreateSubKey(string subkey)
		{
			return this.CreateSubKey(subkey, this.checkMode);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, null, RegistryOptions.None);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions options)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, null, options);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, bool writable)
		{
			return this.CreateSubKeyInternal(subkey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree, null, RegistryOptions.None);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, bool writable, RegistryOptions options)
		{
			return this.CreateSubKeyInternal(subkey, writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree, null, options);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, registrySecurity, RegistryOptions.None);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions, RegistrySecurity registrySecurity)
		{
			return this.CreateSubKeyInternal(subkey, permissionCheck, registrySecurity, registryOptions);
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		private unsafe RegistryKey CreateSubKeyInternal(string subkey, RegistryKeyPermissionCheck permissionCheck, object registrySecurityObj, RegistryOptions registryOptions)
		{
			RegistryKey.ValidateKeyOptions(registryOptions);
			RegistryKey.ValidateKeyName(subkey);
			RegistryKey.ValidateKeyMode(permissionCheck);
			this.EnsureWriteable();
			subkey = RegistryKey.FixupName(subkey);
			if (!this.remoteKey)
			{
				RegistryKey registryKey = this.InternalOpenSubKey(subkey, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree);
				if (registryKey != null)
				{
					this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckSubKeyWritePermission, subkey, false, RegistryKeyPermissionCheck.Default);
					this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckSubTreePermission, subkey, false, permissionCheck);
					registryKey.checkMode = permissionCheck;
					return registryKey;
				}
			}
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckSubKeyCreatePermission, subkey, false, RegistryKeyPermissionCheck.Default);
			Win32Native.SECURITY_ATTRIBUTES security_ATTRIBUTES = null;
			RegistrySecurity registrySecurity = (RegistrySecurity)registrySecurityObj;
			if (registrySecurity != null)
			{
				security_ATTRIBUTES = new Win32Native.SECURITY_ATTRIBUTES();
				security_ATTRIBUTES.nLength = Marshal.SizeOf<Win32Native.SECURITY_ATTRIBUTES>(security_ATTRIBUTES);
				byte[] securityDescriptorBinaryForm = registrySecurity.GetSecurityDescriptorBinaryForm();
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)securityDescriptorBinaryForm.Length) * 1)];
				Buffer.Memcpy(ptr, 0, securityDescriptorBinaryForm, 0, securityDescriptorBinaryForm.Length);
				security_ATTRIBUTES.pSecurityDescriptor = ptr;
			}
			int num = 0;
			SafeRegistryHandle safeRegistryHandle = null;
			int num2 = Win32Native.RegCreateKeyEx(this.hkey, subkey, 0, null, (int)registryOptions, RegistryKey.GetRegistryKeyAccess(permissionCheck != RegistryKeyPermissionCheck.ReadSubTree) | (int)this.regView, security_ATTRIBUTES, out safeRegistryHandle, out num);
			if (num2 == 0 && !safeRegistryHandle.IsInvalid)
			{
				RegistryKey registryKey2 = new RegistryKey(safeRegistryHandle, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree, false, this.remoteKey, false, this.regView);
				this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckSubTreePermission, subkey, false, permissionCheck);
				registryKey2.checkMode = permissionCheck;
				if (subkey.Length == 0)
				{
					registryKey2.keyName = this.keyName;
				}
				else
				{
					registryKey2.keyName = this.keyName + "\\" + subkey;
				}
				return registryKey2;
			}
			if (num2 != 0)
			{
				this.Win32Error(num2, this.keyName + "\\" + subkey);
			}
			return null;
		}

		public void DeleteSubKey(string subkey)
		{
			this.DeleteSubKey(subkey, true);
		}

		[SecuritySafeCritical]
		public void DeleteSubKey(string subkey, bool throwOnMissingSubKey)
		{
			RegistryKey.ValidateKeyName(subkey);
			this.EnsureWriteable();
			subkey = RegistryKey.FixupName(subkey);
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckSubKeyWritePermission, subkey, false, RegistryKeyPermissionCheck.Default);
			RegistryKey registryKey = this.InternalOpenSubKey(subkey, false);
			if (registryKey != null)
			{
				try
				{
					if (registryKey.InternalSubKeyCount() > 0)
					{
						ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_RegRemoveSubKey);
					}
				}
				finally
				{
					registryKey.Close();
				}
				int num;
				try
				{
					num = Win32Native.RegDeleteKeyEx(this.hkey, subkey, (int)this.regView, 0);
				}
				catch (EntryPointNotFoundException)
				{
					num = Win32Native.RegDeleteKey(this.hkey, subkey);
				}
				if (num != 0)
				{
					if (num != 2)
					{
						this.Win32Error(num, null);
						return;
					}
					if (throwOnMissingSubKey)
					{
						ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
						return;
					}
				}
			}
			else if (throwOnMissingSubKey)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
			}
		}

		public void DeleteSubKeyTree(string subkey)
		{
			this.DeleteSubKeyTree(subkey, true);
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey)
		{
			RegistryKey.ValidateKeyName(subkey);
			if (subkey.Length == 0 && this.IsSystemKey())
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegKeyDelHive);
			}
			this.EnsureWriteable();
			subkey = RegistryKey.FixupName(subkey);
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckSubTreeWritePermission, subkey, false, RegistryKeyPermissionCheck.Default);
			RegistryKey registryKey = this.InternalOpenSubKey(subkey, true);
			if (registryKey != null)
			{
				try
				{
					if (registryKey.InternalSubKeyCount() > 0)
					{
						string[] array = registryKey.InternalGetSubKeyNames();
						for (int i = 0; i < array.Length; i++)
						{
							registryKey.DeleteSubKeyTreeInternal(array[i]);
						}
					}
				}
				finally
				{
					registryKey.Close();
				}
				int num;
				try
				{
					num = Win32Native.RegDeleteKeyEx(this.hkey, subkey, (int)this.regView, 0);
				}
				catch (EntryPointNotFoundException)
				{
					num = Win32Native.RegDeleteKey(this.hkey, subkey);
				}
				if (num != 0)
				{
					this.Win32Error(num, null);
					return;
				}
			}
			else if (throwOnMissingSubKey)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
			}
		}

		[SecurityCritical]
		private void DeleteSubKeyTreeInternal(string subkey)
		{
			RegistryKey registryKey = this.InternalOpenSubKey(subkey, true);
			if (registryKey != null)
			{
				try
				{
					if (registryKey.InternalSubKeyCount() > 0)
					{
						string[] array = registryKey.InternalGetSubKeyNames();
						for (int i = 0; i < array.Length; i++)
						{
							registryKey.DeleteSubKeyTreeInternal(array[i]);
						}
					}
				}
				finally
				{
					registryKey.Close();
				}
				int num;
				try
				{
					num = Win32Native.RegDeleteKeyEx(this.hkey, subkey, (int)this.regView, 0);
				}
				catch (EntryPointNotFoundException)
				{
					num = Win32Native.RegDeleteKey(this.hkey, subkey);
				}
				if (num != 0)
				{
					this.Win32Error(num, null);
					return;
				}
			}
			else
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
			}
		}

		public void DeleteValue(string name)
		{
			this.DeleteValue(name, true);
		}

		[SecuritySafeCritical]
		public void DeleteValue(string name, bool throwOnMissingValue)
		{
			this.EnsureWriteable();
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckValueWritePermission, name, false, RegistryKeyPermissionCheck.Default);
			int num = Win32Native.RegDeleteValue(this.hkey, name);
			if ((num == 2 || num == 206) && throwOnMissingValue)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyValueAbsent);
			}
		}

		[SecurityCritical]
		internal static RegistryKey GetBaseKey(IntPtr hKey)
		{
			return RegistryKey.GetBaseKey(hKey, RegistryView.Default);
		}

		[SecurityCritical]
		internal static RegistryKey GetBaseKey(IntPtr hKey, RegistryView view)
		{
			int num = (int)hKey & 268435455;
			bool flag = hKey == RegistryKey.HKEY_PERFORMANCE_DATA;
			SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(hKey, flag);
			return new RegistryKey(safeRegistryHandle, true, true, false, flag, view)
			{
				checkMode = RegistryKeyPermissionCheck.Default,
				keyName = RegistryKey.hkeyNames[num]
			};
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public static RegistryKey OpenBaseKey(RegistryHive hKey, RegistryView view)
		{
			RegistryKey.ValidateKeyView(view);
			RegistryKey.CheckUnmanagedCodePermission();
			return RegistryKey.GetBaseKey((IntPtr)((int)hKey), view);
		}

		public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
		{
			return RegistryKey.OpenRemoteBaseKey(hKey, machineName, RegistryView.Default);
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName, RegistryView view)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			int num = (int)(hKey & (RegistryHive)268435455);
			if (num < 0 || num >= RegistryKey.hkeyNames.Length || ((long)hKey & (long)((ulong)-16)) != (long)((ulong)-2147483648))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_RegKeyOutOfRange"));
			}
			RegistryKey.ValidateKeyView(view);
			RegistryKey.CheckUnmanagedCodePermission();
			SafeRegistryHandle safeRegistryHandle = null;
			int num2 = Win32Native.RegConnectRegistry(machineName, new SafeRegistryHandle(new IntPtr((int)hKey), false), out safeRegistryHandle);
			if (num2 == 1114)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DllInitFailure"));
			}
			if (num2 != 0)
			{
				RegistryKey.Win32ErrorStatic(num2, null);
			}
			if (safeRegistryHandle.IsInvalid)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_RegKeyNoRemoteConnect", new object[]
				{
					machineName
				}));
			}
			return new RegistryKey(safeRegistryHandle, true, false, true, (IntPtr)((int)hKey) == RegistryKey.HKEY_PERFORMANCE_DATA, view)
			{
				checkMode = RegistryKeyPermissionCheck.Default,
				keyName = RegistryKey.hkeyNames[num]
			};
		}

		[SecuritySafeCritical]
		public RegistryKey OpenSubKey(string name, bool writable)
		{
			RegistryKey.ValidateKeyName(name);
			this.EnsureNotDisposed();
			name = RegistryKey.FixupName(name);
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckOpenSubKeyWithWritablePermission, name, writable, RegistryKeyPermissionCheck.Default);
			SafeRegistryHandle safeRegistryHandle = null;
			int num = Win32Native.RegOpenKeyEx(this.hkey, name, 0, RegistryKey.GetRegistryKeyAccess(writable) | (int)this.regView, out safeRegistryHandle);
			if (num == 0 && !safeRegistryHandle.IsInvalid)
			{
				return new RegistryKey(safeRegistryHandle, writable, false, this.remoteKey, false, this.regView)
				{
					checkMode = this.GetSubKeyPermissonCheck(writable),
					keyName = this.keyName + "\\" + name
				};
			}
			if (num == 5 || num == 1346)
			{
				ThrowHelper.ThrowSecurityException(ExceptionResource.Security_RegistryPermission);
			}
			return null;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
		{
			RegistryKey.ValidateKeyMode(permissionCheck);
			return this.InternalOpenSubKey(name, permissionCheck, RegistryKey.GetRegistryKeyAccess(permissionCheck));
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryRights rights)
		{
			return this.InternalOpenSubKey(name, this.checkMode, (int)rights);
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights)
		{
			return this.InternalOpenSubKey(name, permissionCheck, (int)rights);
		}

		[SecurityCritical]
		private RegistryKey InternalOpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, int rights)
		{
			RegistryKey.ValidateKeyName(name);
			RegistryKey.ValidateKeyMode(permissionCheck);
			RegistryKey.ValidateKeyRights(rights);
			this.EnsureNotDisposed();
			name = RegistryKey.FixupName(name);
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckOpenSubKeyPermission, name, false, permissionCheck);
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckSubTreePermission, name, false, permissionCheck);
			SafeRegistryHandle safeRegistryHandle = null;
			int num = Win32Native.RegOpenKeyEx(this.hkey, name, 0, rights | (int)this.regView, out safeRegistryHandle);
			if (num == 0 && !safeRegistryHandle.IsInvalid)
			{
				return new RegistryKey(safeRegistryHandle, permissionCheck == RegistryKeyPermissionCheck.ReadWriteSubTree, false, this.remoteKey, false, this.regView)
				{
					keyName = this.keyName + "\\" + name,
					checkMode = permissionCheck
				};
			}
			if (num == 5 || num == 1346)
			{
				ThrowHelper.ThrowSecurityException(ExceptionResource.Security_RegistryPermission);
			}
			return null;
		}

		[SecurityCritical]
		internal RegistryKey InternalOpenSubKey(string name, bool writable)
		{
			RegistryKey.ValidateKeyName(name);
			this.EnsureNotDisposed();
			SafeRegistryHandle safeRegistryHandle = null;
			if (Win32Native.RegOpenKeyEx(this.hkey, name, 0, RegistryKey.GetRegistryKeyAccess(writable) | (int)this.regView, out safeRegistryHandle) == 0 && !safeRegistryHandle.IsInvalid)
			{
				return new RegistryKey(safeRegistryHandle, writable, false, this.remoteKey, false, this.regView)
				{
					keyName = this.keyName + "\\" + name
				};
			}
			return null;
		}

		public RegistryKey OpenSubKey(string name)
		{
			return this.OpenSubKey(name, false);
		}

		public int SubKeyCount
		{
			[SecuritySafeCritical]
			get
			{
				this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckKeyReadPermission, null, false, RegistryKeyPermissionCheck.Default);
				return this.InternalSubKeyCount();
			}
		}

		[ComVisible(false)]
		public RegistryView View
		{
			[SecuritySafeCritical]
			get
			{
				this.EnsureNotDisposed();
				return this.regView;
			}
		}

		[ComVisible(false)]
		public SafeRegistryHandle Handle
		{
			[SecurityCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				this.EnsureNotDisposed();
				int num = 6;
				if (!this.IsSystemKey())
				{
					return this.hkey;
				}
				IntPtr hKey = (IntPtr)0;
				string text = this.keyName;
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num2 <= 1097425318U)
				{
					if (num2 != 126972219U)
					{
						if (num2 != 457190004U)
						{
							if (num2 == 1097425318U)
							{
								if (text == "HKEY_CLASSES_ROOT")
								{
									hKey = RegistryKey.HKEY_CLASSES_ROOT;
									goto IL_13D;
								}
							}
						}
						else if (text == "HKEY_LOCAL_MACHINE")
						{
							hKey = RegistryKey.HKEY_LOCAL_MACHINE;
							goto IL_13D;
						}
					}
					else if (text == "HKEY_CURRENT_CONFIG")
					{
						hKey = RegistryKey.HKEY_CURRENT_CONFIG;
						goto IL_13D;
					}
				}
				else if (num2 <= 1568329430U)
				{
					if (num2 != 1198714601U)
					{
						if (num2 == 1568329430U)
						{
							if (text == "HKEY_CURRENT_USER")
							{
								hKey = RegistryKey.HKEY_CURRENT_USER;
								goto IL_13D;
							}
						}
					}
					else if (text == "HKEY_USERS")
					{
						hKey = RegistryKey.HKEY_USERS;
						goto IL_13D;
					}
				}
				else if (num2 != 2823865611U)
				{
					if (num2 == 3554990456U)
					{
						if (text == "HKEY_PERFORMANCE_DATA")
						{
							hKey = RegistryKey.HKEY_PERFORMANCE_DATA;
							goto IL_13D;
						}
					}
				}
				else if (text == "HKEY_DYN_DATA")
				{
					hKey = RegistryKey.HKEY_DYN_DATA;
					goto IL_13D;
				}
				this.Win32Error(num, null);
				IL_13D:
				SafeRegistryHandle safeRegistryHandle;
				num = Win32Native.RegOpenKeyEx(hKey, null, 0, RegistryKey.GetRegistryKeyAccess(this.IsWritable()) | (int)this.regView, out safeRegistryHandle);
				if (num == 0 && !safeRegistryHandle.IsInvalid)
				{
					return safeRegistryHandle;
				}
				this.Win32Error(num, null);
				throw new IOException(Win32Native.GetMessage(num), num);
			}
		}

		[SecurityCritical]
		[ComVisible(false)]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static RegistryKey FromHandle(SafeRegistryHandle handle)
		{
			return RegistryKey.FromHandle(handle, RegistryView.Default);
		}

		[SecurityCritical]
		[ComVisible(false)]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static RegistryKey FromHandle(SafeRegistryHandle handle, RegistryView view)
		{
			if (handle == null)
			{
				throw new ArgumentNullException("handle");
			}
			RegistryKey.ValidateKeyView(view);
			return new RegistryKey(handle, true, view);
		}

		[SecurityCritical]
		internal int InternalSubKeyCount()
		{
			this.EnsureNotDisposed();
			int result = 0;
			int num = 0;
			int num2 = Win32Native.RegQueryInfoKey(this.hkey, null, null, IntPtr.Zero, ref result, null, null, ref num, null, null, null, null);
			if (num2 != 0)
			{
				this.Win32Error(num2, null);
			}
			return result;
		}

		[SecuritySafeCritical]
		public string[] GetSubKeyNames()
		{
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckKeyReadPermission, null, false, RegistryKeyPermissionCheck.Default);
			return this.InternalGetSubKeyNames();
		}

		[SecurityCritical]
		internal unsafe string[] InternalGetSubKeyNames()
		{
			this.EnsureNotDisposed();
			int num = this.InternalSubKeyCount();
			string[] array = new string[num];
			if (num > 0)
			{
				char[] array2 = new char[256];
				fixed (char* ptr = &array2[0])
				{
					for (int i = 0; i < num; i++)
					{
						int num2 = array2.Length;
						int num3 = Win32Native.RegEnumKeyEx(this.hkey, i, ptr, ref num2, null, null, null, null);
						if (num3 != 0)
						{
							this.Win32Error(num3, null);
						}
						array[i] = new string(ptr);
					}
				}
			}
			return array;
		}

		public int ValueCount
		{
			[SecuritySafeCritical]
			get
			{
				this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckKeyReadPermission, null, false, RegistryKeyPermissionCheck.Default);
				return this.InternalValueCount();
			}
		}

		[SecurityCritical]
		internal int InternalValueCount()
		{
			this.EnsureNotDisposed();
			int result = 0;
			int num = 0;
			int num2 = Win32Native.RegQueryInfoKey(this.hkey, null, null, IntPtr.Zero, ref num, null, null, ref result, null, null, null, null);
			if (num2 != 0)
			{
				this.Win32Error(num2, null);
			}
			return result;
		}

		[SecuritySafeCritical]
		public unsafe string[] GetValueNames()
		{
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckKeyReadPermission, null, false, RegistryKeyPermissionCheck.Default);
			this.EnsureNotDisposed();
			int num = this.InternalValueCount();
			string[] array = new string[num];
			if (num > 0)
			{
				char[] array2 = new char[16384];
				fixed (char* ptr = &array2[0])
				{
					for (int i = 0; i < num; i++)
					{
						int num2 = array2.Length;
						int num3 = Win32Native.RegEnumValue(this.hkey, i, ptr, ref num2, IntPtr.Zero, null, null, null);
						if (num3 != 0 && (!this.IsPerfDataKey() || num3 != 234))
						{
							this.Win32Error(num3, null);
						}
						array[i] = new string(ptr);
					}
				}
			}
			return array;
		}

		[SecuritySafeCritical]
		public object GetValue(string name)
		{
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckValueReadPermission, name, false, RegistryKeyPermissionCheck.Default);
			return this.InternalGetValue(name, null, false, true);
		}

		[SecuritySafeCritical]
		public object GetValue(string name, object defaultValue)
		{
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckValueReadPermission, name, false, RegistryKeyPermissionCheck.Default);
			return this.InternalGetValue(name, defaultValue, false, true);
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public object GetValue(string name, object defaultValue, RegistryValueOptions options)
		{
			if (options < RegistryValueOptions.None || options > RegistryValueOptions.DoNotExpandEnvironmentNames)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
				{
					(int)options
				}), "options");
			}
			bool doNotExpand = options == RegistryValueOptions.DoNotExpandEnvironmentNames;
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckValueReadPermission, name, false, RegistryKeyPermissionCheck.Default);
			return this.InternalGetValue(name, defaultValue, doNotExpand, true);
		}

		[SecurityCritical]
		internal object InternalGetValue(string name, object defaultValue, bool doNotExpand, bool checkSecurity)
		{
			if (checkSecurity)
			{
				this.EnsureNotDisposed();
			}
			object obj = defaultValue;
			int num = 0;
			int num2 = 0;
			int num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, null, ref num2);
			if (num3 != 0)
			{
				if (this.IsPerfDataKey())
				{
					int num4 = 65000;
					int num5 = num4;
					byte[] array = new byte[num4];
					int num6;
					while (234 == (num6 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, array, ref num5)))
					{
						if (num4 == 2147483647)
						{
							this.Win32Error(num6, name);
						}
						else if (num4 > 1073741823)
						{
							num4 = int.MaxValue;
						}
						else
						{
							num4 *= 2;
						}
						num5 = num4;
						array = new byte[num4];
					}
					if (num6 != 0)
					{
						this.Win32Error(num6, name);
					}
					return array;
				}
				if (num3 != 234)
				{
					return obj;
				}
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			switch (num)
			{
			case 0:
			case 3:
			case 5:
				break;
			case 1:
			{
				char[] array2;
				checked
				{
					if (num2 % 2 == 1)
					{
						try
						{
							num2++;
						}
						catch (OverflowException innerException)
						{
							throw new IOException(Environment.GetResourceString("Arg_RegGetOverflowBug"), innerException);
						}
					}
					array2 = new char[num2 / 2];
					num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, array2, ref num2);
				}
				if (array2.Length != 0 && array2[array2.Length - 1] == '\0')
				{
					return new string(array2, 0, array2.Length - 1);
				}
				return new string(array2);
			}
			case 2:
			{
				char[] array3;
				checked
				{
					if (num2 % 2 == 1)
					{
						try
						{
							num2++;
						}
						catch (OverflowException innerException2)
						{
							throw new IOException(Environment.GetResourceString("Arg_RegGetOverflowBug"), innerException2);
						}
					}
					array3 = new char[num2 / 2];
					num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, array3, ref num2);
				}
				if (array3.Length != 0 && array3[array3.Length - 1] == '\0')
				{
					obj = new string(array3, 0, array3.Length - 1);
				}
				else
				{
					obj = new string(array3);
				}
				if (!doNotExpand)
				{
					return Environment.ExpandEnvironmentVariables((string)obj);
				}
				return obj;
			}
			case 4:
				if (num2 <= 4)
				{
					int num7 = 0;
					num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, ref num7, ref num2);
					return num7;
				}
				goto IL_122;
			case 6:
			case 8:
			case 9:
			case 10:
				return obj;
			case 7:
			{
				char[] array4;
				checked
				{
					if (num2 % 2 == 1)
					{
						try
						{
							num2++;
						}
						catch (OverflowException innerException3)
						{
							throw new IOException(Environment.GetResourceString("Arg_RegGetOverflowBug"), innerException3);
						}
					}
					array4 = new char[num2 / 2];
					num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, array4, ref num2);
				}
				if (array4.Length != 0 && array4[array4.Length - 1] != '\0')
				{
					try
					{
						char[] array5 = new char[checked(array4.Length + 1)];
						for (int i = 0; i < array4.Length; i++)
						{
							array5[i] = array4[i];
						}
						array5[array5.Length - 1] = '\0';
						array4 = array5;
					}
					catch (OverflowException innerException4)
					{
						throw new IOException(Environment.GetResourceString("Arg_RegGetOverflowBug"), innerException4);
					}
					array4[array4.Length - 1] = '\0';
				}
				IList<string> list = new List<string>();
				int num8 = 0;
				int num9 = array4.Length;
				while (num3 == 0 && num8 < num9)
				{
					int num10 = num8;
					while (num10 < num9 && array4[num10] != '\0')
					{
						num10++;
					}
					if (num10 < num9)
					{
						if (num10 - num8 > 0)
						{
							list.Add(new string(array4, num8, num10 - num8));
						}
						else if (num10 != num9 - 1)
						{
							list.Add(string.Empty);
						}
					}
					else
					{
						list.Add(new string(array4, num8, num9 - num8));
					}
					num8 = num10 + 1;
				}
				obj = new string[list.Count];
				list.CopyTo((string[])obj, 0);
				return obj;
			}
			case 11:
				goto IL_122;
			default:
				return obj;
			}
			IL_FC:
			byte[] array6 = new byte[num2];
			num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, array6, ref num2);
			return array6;
			IL_122:
			if (num2 > 8)
			{
				goto IL_FC;
			}
			long num11 = 0L;
			num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, ref num11, ref num2);
			obj = num11;
			return obj;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public RegistryValueKind GetValueKind(string name)
		{
			this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckValueReadPermission, name, false, RegistryKeyPermissionCheck.Default);
			this.EnsureNotDisposed();
			int num = 0;
			int num2 = 0;
			int num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, null, ref num2);
			if (num3 != 0)
			{
				this.Win32Error(num3, null);
			}
			if (num == 0)
			{
				return RegistryValueKind.None;
			}
			if (!Enum.IsDefined(typeof(RegistryValueKind), num))
			{
				return RegistryValueKind.Unknown;
			}
			return (RegistryValueKind)num;
		}

		private bool IsDirty()
		{
			return (this.state & 1) != 0;
		}

		private bool IsSystemKey()
		{
			return (this.state & 2) != 0;
		}

		private bool IsWritable()
		{
			return (this.state & 4) != 0;
		}

		private bool IsPerfDataKey()
		{
			return (this.state & 8) != 0;
		}

		public string Name
		{
			[SecuritySafeCritical]
			get
			{
				this.EnsureNotDisposed();
				return this.keyName;
			}
		}

		private void SetDirty()
		{
			this.state |= 1;
		}

		public void SetValue(string name, object value)
		{
			this.SetValue(name, value, RegistryValueKind.Unknown);
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public unsafe void SetValue(string name, object value, RegistryValueKind valueKind)
		{
			if (value == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
			}
			if (name != null && name.Length > 16383)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_RegValStrLenBug"));
			}
			if (!Enum.IsDefined(typeof(RegistryValueKind), valueKind))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_RegBadKeyKind"), "valueKind");
			}
			this.EnsureWriteable();
			if (!this.remoteKey && this.ContainsRegistryValue(name))
			{
				this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckValueWritePermission, name, false, RegistryKeyPermissionCheck.Default);
			}
			else
			{
				this.CheckPermission(RegistryKey.RegistryInternalCheck.CheckValueCreatePermission, name, false, RegistryKeyPermissionCheck.Default);
			}
			if (valueKind == RegistryValueKind.Unknown)
			{
				valueKind = this.CalculateValueKind(value);
			}
			int num = 0;
			try
			{
				switch (valueKind)
				{
				case RegistryValueKind.None:
				case RegistryValueKind.Binary:
					break;
				case RegistryValueKind.Unknown:
				case (RegistryValueKind)5:
				case (RegistryValueKind)6:
				case (RegistryValueKind)8:
				case (RegistryValueKind)9:
				case (RegistryValueKind)10:
					goto IL_274;
				case RegistryValueKind.String:
				case RegistryValueKind.ExpandString:
				{
					string text = value.ToString();
					num = Win32Native.RegSetValueEx(this.hkey, name, 0, valueKind, text, checked(text.Length * 2 + 2));
					goto IL_274;
				}
				case RegistryValueKind.DWord:
				{
					int num2 = Convert.ToInt32(value, CultureInfo.InvariantCulture);
					num = Win32Native.RegSetValueEx(this.hkey, name, 0, RegistryValueKind.DWord, ref num2, 4);
					goto IL_274;
				}
				case RegistryValueKind.MultiString:
				{
					string[] array = (string[])((string[])value).Clone();
					int num3 = 0;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] == null)
						{
							ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetStrArrNull);
						}
						checked
						{
							num3 += (array[i].Length + 1) * 2;
						}
					}
					byte[] array2;
					checked
					{
						num3 += 2;
						array2 = new byte[num3];
					}
					try
					{
						fixed (byte* ptr = array2)
						{
							IntPtr intPtr = new IntPtr((void*)ptr);
							for (int j = 0; j < array.Length; j++)
							{
								string.InternalCopy(array[j], intPtr, checked(array[j].Length * 2));
								intPtr = new IntPtr((long)intPtr + (long)(checked(array[j].Length * 2)));
								*(short*)intPtr.ToPointer() = 0;
								intPtr = new IntPtr((long)intPtr + 2L);
							}
							*(short*)intPtr.ToPointer() = 0;
							intPtr = new IntPtr((long)intPtr + 2L);
							num = Win32Native.RegSetValueEx(this.hkey, name, 0, RegistryValueKind.MultiString, array2, num3);
							goto IL_274;
						}
					}
					finally
					{
						byte* ptr = null;
					}
					break;
				}
				case RegistryValueKind.QWord:
				{
					long num4 = Convert.ToInt64(value, CultureInfo.InvariantCulture);
					num = Win32Native.RegSetValueEx(this.hkey, name, 0, RegistryValueKind.QWord, ref num4, 8);
					goto IL_274;
				}
				default:
					goto IL_274;
				}
				byte[] array3 = (byte[])value;
				num = Win32Native.RegSetValueEx(this.hkey, name, 0, (valueKind == RegistryValueKind.None) ? RegistryValueKind.Unknown : RegistryValueKind.Binary, array3, array3.Length);
				IL_274:;
			}
			catch (OverflowException)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
			}
			catch (InvalidOperationException)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
			}
			catch (FormatException)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
			}
			catch (InvalidCastException)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
			}
			if (num == 0)
			{
				this.SetDirty();
				return;
			}
			this.Win32Error(num, null);
		}

		private RegistryValueKind CalculateValueKind(object value)
		{
			if (value is int)
			{
				return RegistryValueKind.DWord;
			}
			if (!(value is Array))
			{
				return RegistryValueKind.String;
			}
			if (value is byte[])
			{
				return RegistryValueKind.Binary;
			}
			if (value is string[])
			{
				return RegistryValueKind.MultiString;
			}
			throw new ArgumentException(Environment.GetResourceString("Arg_RegSetBadArrType", new object[]
			{
				value.GetType().Name
			}));
		}

		[SecuritySafeCritical]
		public override string ToString()
		{
			this.EnsureNotDisposed();
			return this.keyName;
		}

		public RegistrySecurity GetAccessControl()
		{
			return this.GetAccessControl(AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		[SecuritySafeCritical]
		public RegistrySecurity GetAccessControl(AccessControlSections includeSections)
		{
			this.EnsureNotDisposed();
			return new RegistrySecurity(this.hkey, this.keyName, includeSections);
		}

		[SecuritySafeCritical]
		public void SetAccessControl(RegistrySecurity registrySecurity)
		{
			this.EnsureWriteable();
			if (registrySecurity == null)
			{
				throw new ArgumentNullException("registrySecurity");
			}
			registrySecurity.Persist(this.hkey, this.keyName);
		}

		[SecuritySafeCritical]
		internal void Win32Error(int errorCode, string str)
		{
			switch (errorCode)
			{
			case 2:
				throw new IOException(Environment.GetResourceString("Arg_RegKeyNotFound"), errorCode);
			case 5:
				if (str != null)
				{
					throw new UnauthorizedAccessException(Environment.GetResourceString("UnauthorizedAccess_RegistryKeyGeneric_Key", new object[]
					{
						str
					}));
				}
				throw new UnauthorizedAccessException();
			case 6:
				if (!this.IsPerfDataKey())
				{
					this.hkey.SetHandleAsInvalid();
					this.hkey = null;
				}
				break;
			}
			throw new IOException(Win32Native.GetMessage(errorCode), errorCode);
		}

		[SecuritySafeCritical]
		internal static void Win32ErrorStatic(int errorCode, string str)
		{
			if (errorCode != 5)
			{
				throw new IOException(Win32Native.GetMessage(errorCode), errorCode);
			}
			if (str != null)
			{
				throw new UnauthorizedAccessException(Environment.GetResourceString("UnauthorizedAccess_RegistryKeyGeneric_Key", new object[]
				{
					str
				}));
			}
			throw new UnauthorizedAccessException();
		}

		internal static string FixupName(string name)
		{
			if (name.IndexOf('\\') == -1)
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder(name);
			RegistryKey.FixupPath(stringBuilder);
			int num = stringBuilder.Length - 1;
			if (num >= 0 && stringBuilder[num] == '\\')
			{
				stringBuilder.Length = num;
			}
			return stringBuilder.ToString();
		}

		private static void FixupPath(StringBuilder path)
		{
			int length = path.Length;
			bool flag = false;
			char maxValue = char.MaxValue;
			for (int i = 1; i < length - 1; i++)
			{
				if (path[i] == '\\')
				{
					i++;
					while (i < length && path[i] == '\\')
					{
						path[i] = maxValue;
						i++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				int i = 0;
				int num = 0;
				while (i < length)
				{
					if (path[i] == maxValue)
					{
						i++;
					}
					else
					{
						path[num] = path[i];
						i++;
						num++;
					}
				}
				path.Length += num - i;
			}
		}

		private void GetSubKeyReadPermission(string subkeyName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Read;
			path = this.keyName + "\\" + subkeyName + "\\.";
		}

		private void GetSubKeyWritePermission(string subkeyName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Write;
			path = this.keyName + "\\" + subkeyName + "\\.";
		}

		private void GetSubKeyCreatePermission(string subkeyName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Create;
			path = this.keyName + "\\" + subkeyName + "\\.";
		}

		private void GetSubTreeReadPermission(string subkeyName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Read;
			path = this.keyName + "\\" + subkeyName + "\\";
		}

		private void GetSubTreeWritePermission(string subkeyName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Write;
			path = this.keyName + "\\" + subkeyName + "\\";
		}

		private void GetSubTreeReadWritePermission(string subkeyName, out RegistryPermissionAccess access, out string path)
		{
			access = (RegistryPermissionAccess.Read | RegistryPermissionAccess.Write);
			path = this.keyName + "\\" + subkeyName;
		}

		private void GetValueReadPermission(string valueName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Read;
			path = this.keyName + "\\" + valueName;
		}

		private void GetValueWritePermission(string valueName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Write;
			path = this.keyName + "\\" + valueName;
		}

		private void GetValueCreatePermission(string valueName, out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Create;
			path = this.keyName + "\\" + valueName;
		}

		private void GetKeyReadPermission(out RegistryPermissionAccess access, out string path)
		{
			access = RegistryPermissionAccess.Read;
			path = this.keyName + "\\.";
		}

		[SecurityCritical]
		private void CheckPermission(RegistryKey.RegistryInternalCheck check, string item, bool subKeyWritable, RegistryKeyPermissionCheck subKeyCheck)
		{
			bool flag = false;
			RegistryPermissionAccess access = RegistryPermissionAccess.NoAccess;
			string pathList = null;
			if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				return;
			}
			switch (check)
			{
			case RegistryKey.RegistryInternalCheck.CheckSubKeyWritePermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetSubKeyWritePermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckSubKeyReadPermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else
				{
					flag = true;
					this.GetSubKeyReadPermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckSubKeyCreatePermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetSubKeyCreatePermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckSubTreeReadPermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetSubTreeReadPermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckSubTreeWritePermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetSubTreeWritePermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckSubTreeReadWritePermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else
				{
					flag = true;
					this.GetSubTreeReadWritePermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckValueWritePermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetValueWritePermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckValueCreatePermission:
				if (this.remoteKey)
				{
					RegistryKey.CheckUnmanagedCodePermission();
				}
				else if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetValueCreatePermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckValueReadPermission:
				if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetValueReadPermission(item, out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckKeyReadPermission:
				if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					flag = true;
					this.GetKeyReadPermission(out access, out pathList);
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckSubTreePermission:
				if (subKeyCheck == RegistryKeyPermissionCheck.ReadSubTree)
				{
					if (this.checkMode == RegistryKeyPermissionCheck.Default)
					{
						if (this.remoteKey)
						{
							RegistryKey.CheckUnmanagedCodePermission();
						}
						else
						{
							flag = true;
							this.GetSubTreeReadPermission(item, out access, out pathList);
						}
					}
				}
				else if (subKeyCheck == RegistryKeyPermissionCheck.ReadWriteSubTree && this.checkMode != RegistryKeyPermissionCheck.ReadWriteSubTree)
				{
					if (this.remoteKey)
					{
						RegistryKey.CheckUnmanagedCodePermission();
					}
					else
					{
						flag = true;
						this.GetSubTreeReadWritePermission(item, out access, out pathList);
					}
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckOpenSubKeyWithWritablePermission:
				if (this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					if (this.remoteKey)
					{
						RegistryKey.CheckUnmanagedCodePermission();
					}
					else
					{
						flag = true;
						this.GetSubKeyReadPermission(item, out access, out pathList);
					}
				}
				else if (subKeyWritable && this.checkMode == RegistryKeyPermissionCheck.ReadSubTree)
				{
					if (this.remoteKey)
					{
						RegistryKey.CheckUnmanagedCodePermission();
					}
					else
					{
						flag = true;
						this.GetSubTreeReadWritePermission(item, out access, out pathList);
					}
				}
				break;
			case RegistryKey.RegistryInternalCheck.CheckOpenSubKeyPermission:
				if (subKeyCheck == RegistryKeyPermissionCheck.Default && this.checkMode == RegistryKeyPermissionCheck.Default)
				{
					if (this.remoteKey)
					{
						RegistryKey.CheckUnmanagedCodePermission();
					}
					else
					{
						flag = true;
						this.GetSubKeyReadPermission(item, out access, out pathList);
					}
				}
				break;
			}
			if (flag)
			{
				new RegistryPermission(access, pathList).Demand();
			}
		}

		[SecurityCritical]
		private static void CheckUnmanagedCodePermission()
		{
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
		}

		[SecurityCritical]
		private bool ContainsRegistryValue(string name)
		{
			int num = 0;
			int num2 = 0;
			int num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref num, null, ref num2);
			return num3 == 0;
		}

		[SecurityCritical]
		private void EnsureNotDisposed()
		{
			if (this.hkey == null)
			{
				ThrowHelper.ThrowObjectDisposedException(this.keyName, ExceptionResource.ObjectDisposed_RegKeyClosed);
			}
		}

		[SecurityCritical]
		private void EnsureWriteable()
		{
			this.EnsureNotDisposed();
			if (!this.IsWritable())
			{
				ThrowHelper.ThrowUnauthorizedAccessException(ExceptionResource.UnauthorizedAccess_RegistryNoWrite);
			}
		}

		private static int GetRegistryKeyAccess(bool isWritable)
		{
			int result;
			if (!isWritable)
			{
				result = 131097;
			}
			else
			{
				result = 131103;
			}
			return result;
		}

		private static int GetRegistryKeyAccess(RegistryKeyPermissionCheck mode)
		{
			int result = 0;
			if (mode > RegistryKeyPermissionCheck.ReadSubTree)
			{
				if (mode == RegistryKeyPermissionCheck.ReadWriteSubTree)
				{
					result = 131103;
				}
			}
			else
			{
				result = 131097;
			}
			return result;
		}

		private RegistryKeyPermissionCheck GetSubKeyPermissonCheck(bool subkeyWritable)
		{
			if (this.checkMode == RegistryKeyPermissionCheck.Default)
			{
				return this.checkMode;
			}
			if (subkeyWritable)
			{
				return RegistryKeyPermissionCheck.ReadWriteSubTree;
			}
			return RegistryKeyPermissionCheck.ReadSubTree;
		}

		private static void ValidateKeyName(string name)
		{
			if (name == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.name);
			}
			int num = name.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
			int num2 = 0;
			while (num != -1)
			{
				if (num - num2 > 255)
				{
					ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegKeyStrLenBug);
				}
				num2 = num + 1;
				num = name.IndexOf("\\", num2, StringComparison.OrdinalIgnoreCase);
			}
			if (name.Length - num2 > 255)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegKeyStrLenBug);
			}
		}

		private static void ValidateKeyMode(RegistryKeyPermissionCheck mode)
		{
			if (mode < RegistryKeyPermissionCheck.Default || mode > RegistryKeyPermissionCheck.ReadWriteSubTree)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidRegistryKeyPermissionCheck, ExceptionArgument.mode);
			}
		}

		private static void ValidateKeyOptions(RegistryOptions options)
		{
			if (options < RegistryOptions.None || options > RegistryOptions.Volatile)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidRegistryOptionsCheck, ExceptionArgument.options);
			}
		}

		private static void ValidateKeyView(RegistryView view)
		{
			if (view != RegistryView.Default && view != RegistryView.Registry32 && view != RegistryView.Registry64)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidRegistryViewCheck, ExceptionArgument.view);
			}
		}

		private static void ValidateKeyRights(int rights)
		{
			if ((rights & -983104) != 0)
			{
				ThrowHelper.ThrowSecurityException(ExceptionResource.Security_RegistryPermission);
			}
		}

		internal static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);

		internal static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);

		internal static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);

		internal static readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);

		internal static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);

		internal static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);

		internal static readonly IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);

		private const int STATE_DIRTY = 1;

		private const int STATE_SYSTEMKEY = 2;

		private const int STATE_WRITEACCESS = 4;

		private const int STATE_PERF_DATA = 8;

		private static readonly string[] hkeyNames = new string[]
		{
			"HKEY_CLASSES_ROOT",
			"HKEY_CURRENT_USER",
			"HKEY_LOCAL_MACHINE",
			"HKEY_USERS",
			"HKEY_PERFORMANCE_DATA",
			"HKEY_CURRENT_CONFIG",
			"HKEY_DYN_DATA"
		};

		private const int MaxKeyLength = 255;

		private const int MaxValueLength = 16383;

		[SecurityCritical]
		private volatile SafeRegistryHandle hkey;

		private volatile int state;

		private volatile string keyName;

		private volatile bool remoteKey;

		private volatile RegistryKeyPermissionCheck checkMode;

		private volatile RegistryView regView;

		private const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

		private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

		private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

		private enum RegistryInternalCheck
		{
			CheckSubKeyWritePermission,
			CheckSubKeyReadPermission,
			CheckSubKeyCreatePermission,
			CheckSubTreeReadPermission,
			CheckSubTreeWritePermission,
			CheckSubTreeReadWritePermission,
			CheckValueWritePermission,
			CheckValueCreatePermission,
			CheckValueReadPermission,
			CheckKeyReadPermission,
			CheckSubTreePermission,
			CheckOpenSubKeyWithWritablePermission,
			CheckOpenSubKeyPermission
		}
	}
}
