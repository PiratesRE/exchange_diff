using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using Microsoft.Win32;

namespace System.Security.AccessControl
{
	internal static class Win32
	{
		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		internal static int ConvertSdToSddl(byte[] binaryForm, int requestedRevision, SecurityInfos si, out string resultSddl)
		{
			uint num = 0U;
			IntPtr intPtr;
			if (1 == Win32Native.ConvertSdToStringSd(binaryForm, (uint)requestedRevision, (uint)si, out intPtr, ref num))
			{
				resultSddl = Marshal.PtrToStringUni(intPtr);
				Win32Native.LocalFree(intPtr);
				return 0;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			resultSddl = null;
			if (lastWin32Error == 8)
			{
				throw new OutOfMemoryException();
			}
			return lastWin32Error;
		}

		[SecurityCritical]
		[HandleProcessCorruptedStateExceptions]
		internal static int GetSecurityInfo(ResourceType resourceType, string name, SafeHandle handle, AccessControlSections accessControlSections, out RawSecurityDescriptor resultSd)
		{
			resultSd = null;
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
			SecurityInfos securityInfos = (SecurityInfos)0;
			Privilege privilege = null;
			if ((accessControlSections & AccessControlSections.Owner) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.Owner;
			}
			if ((accessControlSections & AccessControlSections.Group) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.Group;
			}
			if ((accessControlSections & AccessControlSections.Access) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.DiscretionaryAcl;
			}
			if ((accessControlSections & AccessControlSections.Audit) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.SystemAcl;
				privilege = new Privilege("SeSecurityPrivilege");
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			IntPtr intPtr5;
			int num;
			try
			{
				if (privilege != null)
				{
					try
					{
						privilege.Enable();
					}
					catch (PrivilegeNotHeldException)
					{
					}
				}
				if (name != null)
				{
					IntPtr intPtr;
					IntPtr intPtr2;
					IntPtr intPtr3;
					IntPtr intPtr4;
					num = (int)Win32Native.GetSecurityInfoByName(name, (uint)resourceType, (uint)securityInfos, out intPtr, out intPtr2, out intPtr3, out intPtr4, out intPtr5);
				}
				else
				{
					if (handle == null)
					{
						throw new SystemException();
					}
					if (handle.IsInvalid)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_InvalidSafeHandle"), "handle");
					}
					IntPtr intPtr;
					IntPtr intPtr2;
					IntPtr intPtr3;
					IntPtr intPtr4;
					num = (int)Win32Native.GetSecurityInfoByHandle(handle, (uint)resourceType, (uint)securityInfos, out intPtr, out intPtr2, out intPtr3, out intPtr4, out intPtr5);
				}
				if (num == 0 && IntPtr.Zero.Equals(intPtr5))
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NoSecurityDescriptor"));
				}
				if (num == 1300 || num == 1314)
				{
					throw new PrivilegeNotHeldException("SeSecurityPrivilege");
				}
				if (num == 5 || num == 1347)
				{
					throw new UnauthorizedAccessException();
				}
				if (num != 0)
				{
					goto IL_162;
				}
			}
			catch
			{
				if (privilege != null)
				{
					privilege.Revert();
				}
				throw;
			}
			finally
			{
				if (privilege != null)
				{
					privilege.Revert();
				}
			}
			uint securityDescriptorLength = Win32Native.GetSecurityDescriptorLength(intPtr5);
			byte[] array = new byte[securityDescriptorLength];
			Marshal.Copy(intPtr5, array, 0, (int)securityDescriptorLength);
			Win32Native.LocalFree(intPtr5);
			resultSd = new RawSecurityDescriptor(array, 0);
			return 0;
			IL_162:
			if (num == 8)
			{
				throw new OutOfMemoryException();
			}
			return num;
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		internal static int SetSecurityInfo(ResourceType type, string name, SafeHandle handle, SecurityInfos securityInformation, SecurityIdentifier owner, SecurityIdentifier group, GenericAcl sacl, GenericAcl dacl)
		{
			byte[] array = null;
			byte[] array2 = null;
			byte[] array3 = null;
			byte[] array4 = null;
			Privilege privilege = null;
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
			if (owner != null)
			{
				int binaryLength = owner.BinaryLength;
				array = new byte[binaryLength];
				owner.GetBinaryForm(array, 0);
			}
			if (group != null)
			{
				int binaryLength = group.BinaryLength;
				array2 = new byte[binaryLength];
				group.GetBinaryForm(array2, 0);
			}
			if (dacl != null)
			{
				int binaryLength = dacl.BinaryLength;
				array4 = new byte[binaryLength];
				dacl.GetBinaryForm(array4, 0);
			}
			if (sacl != null)
			{
				int binaryLength = sacl.BinaryLength;
				array3 = new byte[binaryLength];
				sacl.GetBinaryForm(array3, 0);
			}
			if ((securityInformation & SecurityInfos.SystemAcl) != (SecurityInfos)0)
			{
				privilege = new Privilege("SeSecurityPrivilege");
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			int num;
			try
			{
				if (privilege != null)
				{
					try
					{
						privilege.Enable();
					}
					catch (PrivilegeNotHeldException)
					{
					}
				}
				if (name != null)
				{
					num = (int)Win32Native.SetSecurityInfoByName(name, (uint)type, (uint)securityInformation, array, array2, array4, array3);
				}
				else
				{
					if (handle == null)
					{
						throw new InvalidProgramException();
					}
					if (handle.IsInvalid)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_InvalidSafeHandle"), "handle");
					}
					num = (int)Win32Native.SetSecurityInfoByHandle(handle, (uint)type, (uint)securityInformation, array, array2, array4, array3);
				}
				if (num == 1300 || num == 1314)
				{
					throw new PrivilegeNotHeldException("SeSecurityPrivilege");
				}
				if (num == 5 || num == 1347)
				{
					throw new UnauthorizedAccessException();
				}
				if (num != 0)
				{
					goto IL_159;
				}
			}
			catch
			{
				if (privilege != null)
				{
					privilege.Revert();
				}
				throw;
			}
			finally
			{
				if (privilege != null)
				{
					privilege.Revert();
				}
			}
			return 0;
			IL_159:
			if (num == 8)
			{
				throw new OutOfMemoryException();
			}
			return num;
		}

		internal const int TRUE = 1;
	}
}
