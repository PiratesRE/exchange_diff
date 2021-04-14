using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.AdminRpc
{
	internal class AdminRpcClient : RpcClientBase
	{
		public AdminRpcClient(string machineName, ValueType clientObjectGuid) : base(machineName, null, null, null, RpcClientFlags.AllowImpersonation | RpcClientFlags.UseEncryptedConnection | RpcClientFlags.ExplicitEndpointLookup, <Module>.mdbadmin20_v2_0_s_ifspec, clientObjectGuid, null, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void AdminGetIFVersion(out ushort major, out ushort minor)
		{
			try
			{
				ushort num = 0;
				ushort num2 = 0;
				<Module>.cli_AdminGetIFVersion(base.BindingHandle, 0, &num, &num2);
				major = num;
				minor = num2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_AdminGetIFVersion");
			}
		}

		public IRpcAsyncResult BeginEcListAllMdbStatus50([MarshalAs(UnmanagedType.U1)] bool fBasicInformation, byte[] auxIn, AsyncCallback callback, object state)
		{
			int cbAuxIn = 0;
			byte ptr;
			if (auxIn != null)
			{
				int num = auxIn.Length;
				if (num > 0)
				{
					cbAuxIn = num;
					ptr = ref auxIn[0];
					goto IL_25;
				}
			}
			ptr = ref (new byte[1])[0];
			IL_25:
			ref byte pbAuxIn = ref ptr;
			return this.InternalBeginEcListAllMdbStatus50(fBasicInformation, cbAuxIn, ref pbAuxIn, callback, state);
		}

		public int EndEcListAllMdbStatus50(IRpcAsyncResult asyncResult, out uint databaseCount, out byte[] mdbStatus, out byte[] auxOut)
		{
			if (null == asyncResult)
			{
				throw new ArgumentNullException("asyncResult");
			}
			EcListAllMdbStatus50AsyncResult ecListAllMdbStatus50AsyncResult = asyncResult as EcListAllMdbStatus50AsyncResult;
			if (null == ecListAllMdbStatus50AsyncResult)
			{
				throw new ArgumentException("Invalid type.", "asyncResult");
			}
			int result;
			try
			{
				ecListAllMdbStatus50AsyncResult.AsyncWaitHandle.WaitOne();
				result = ecListAllMdbStatus50AsyncResult.Complete(out databaseCount, out mdbStatus, out auxOut);
			}
			finally
			{
				if (ecListAllMdbStatus50AsyncResult != null)
				{
					((IDisposable)ecListAllMdbStatus50AsyncResult).Dispose();
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcListMdbStatus50(Guid[] databases, out uint[] status, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			status = null;
			uint* ptr3 = null;
			_GUID* ptr4 = null;
			try
			{
				int num2;
				if (databases != null)
				{
					num2 = databases.Length;
				}
				else
				{
					num2 = 0;
				}
				uint num3 = num2;
				if (num2 != 0)
				{
					ulong num4 = (ulong)num2;
					ptr4 = <Module>.MIDL_user_allocate(num4 * 16UL);
					if (null == ptr4)
					{
						throw new OutOfMemoryException();
					}
					ptr3 = <Module>.MIDL_user_allocate(num4 * 4UL);
					if (null == ptr3)
					{
						throw new OutOfMemoryException();
					}
					for (uint num5 = 0; num5 < num3; num5++)
					{
						Guid guid = databases[num5];
						_GUID guid2 = <Module>.ToGUID(ref guid);
						long num6 = (long)num5;
						cpblk(num6 * 16L / (long)sizeof(_GUID) + ptr4, ref guid2, 16);
						*(int*)(num6 * 4L / (long)sizeof(uint) + ptr3) = 0;
					}
					int num7 = 0;
					ptr = <Module>.MToUBytesClient(auxIn, &num7);
					uint num8 = 0;
					num = <Module>.cli_EcListMdbStatus50(base.BindingHandle, num3, ptr4, ptr3, num7, ptr, &num8, &ptr2);
					byte[] array;
					if (num8 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr2);
						array = <Module>.UToMBytes(num8, uPtrData);
					}
					else
					{
						array = null;
					}
					auxOut = array;
					if (0 == num)
					{
						status = new uint[num3];
						for (uint num9 = 0; num9 < num3; num9++)
						{
							status[num9] = (uint)(num9 * 4L)[ptr3 / 4];
						}
					}
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcListMdbStatus40");
			}
			finally
			{
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcGetDatabaseSizeEx50(Guid database, out uint dbTotalPages, out uint dbAvailablePages, out uint pageSize, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			dbTotalPages = 0U;
			dbAvailablePages = 0U;
			pageSize = 0U;
			try
			{
				uint num = 0;
				uint num2 = 0;
				uint num3 = 0;
				_GUID guid = <Module>.ToGUID(ref database);
				int num4 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num4);
				uint num5 = 0;
				result = <Module>.cli_EcGetDatabaseSizeEx50(base.BindingHandle, &guid, &num, &num2, &num3, num4, ptr, &num5, &ptr2);
				byte[] array;
				if (num5 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num5, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				dbTotalPages = num;
				dbAvailablePages = num2;
				pageSize = num3;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcGetDatabaseSizeEx50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetCnctTable50(Guid database, int lParam, out byte[] result, uint[] propertyTags, uint cpid, out uint numberOfRows, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			byte* ptr3 = null;
			uint* ptr4 = null;
			result = null;
			numberOfRows = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propertyTags != null)
				{
					num4 = propertyTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr4 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr4)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr4 / 4] = (int)propertyTags[num6];
				}
				_GUID guid = <Module>.ToGUID(ref database);
				int num7 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetCnctTable50(base.BindingHandle, &guid, lParam, &ptr3, &num2, ptr4, num5, cpid, &num3, num7, ptr, &num8, &ptr2);
				byte[] array;
				if (num8 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num8, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				if (0 == num)
				{
					numberOfRows = num3;
					byte[] array2;
					if (num2 > 0)
					{
						IntPtr uPtrData2 = new IntPtr((void*)ptr3);
						array2 = <Module>.UToMBytes(num2, uPtrData2);
					}
					else
					{
						array2 = null;
					}
					result = array2;
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetCnctTable40");
			}
			finally
			{
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcGetLastBackupTimes50(Guid database, out long lastCompleteBackupTime, out long lastIncrementalBackupTime, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			lastCompleteBackupTime = 0L;
			lastIncrementalBackupTime = 0L;
			try
			{
				_FILETIME filetime = 0;
				initblk(ref filetime + 4, 0, 4L);
				_FILETIME filetime2 = 0;
				initblk(ref filetime2 + 4, 0, 4L);
				_GUID guid = <Module>.ToGUID(ref database);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcGetLastBackupTimesEx50(base.BindingHandle, &guid, &filetime, &filetime2, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				lastCompleteBackupTime = filetime;
				lastIncrementalBackupTime = filetime2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcGetLastBackupTimes50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcClearAbsentInDsFlagOnMailbox50(Guid database, Guid mailbox, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcClearAbsentInDsFlagOnMailbox50(base.BindingHandle, &guid, &guid2, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcClearAbsentInDsFlagOnMailbox40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcPurgeCachedMailboxObject50(Guid mailbox, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mailbox);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcPurgeCachedMailboxObject50(base.BindingHandle, &guid, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcPurgeCachedMailboxObject40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcSyncMailboxesWithDS50(Guid database, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcSyncMailboxesWithDS50(base.BindingHandle, &guid, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcSyncMailboxesWithDS40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminDeletePrivateMailbox50(Guid database, Guid mailbox, uint flags, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcAdminDeletePrivateMailbox50(base.BindingHandle, &guid, &guid2, flags, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminDeletePrivateMailbox40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcSetMailboxSecurityDescriptor50(Guid database, Guid mailbox, byte[] securityDescriptor, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			byte* ptr3 = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num = 0;
				ptr3 = <Module>.MToUBytesClient(securityDescriptor, &num);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				result = <Module>.cli_EcSetMailboxSecurityDescriptor50(base.BindingHandle, &guid, &guid2, ptr3, num, num2, ptr, &num3, &ptr2);
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcSetMailboxSecurityDescriptor40");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcGetMailboxSecurityDescriptor50(Guid database, Guid mailbox, out byte[] securityDescriptor, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			byte* ptr3 = null;
			securityDescriptor = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				uint num2 = 0;
				int num3 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num3);
				uint num4 = 0;
				num = <Module>.cli_EcGetMailboxSecurityDescriptor50(base.BindingHandle, &guid, &guid2, &ptr3, &num2, num3, ptr, &num4, &ptr2);
				byte[] array;
				if (num4 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num4, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				if (0 == num && num2 > 0)
				{
					securityDescriptor = <Module>.UToMBytes(num2, ptr3);
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcGetMailboxSecurityDescriptor40");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetLogonTable50(Guid database, int lParam, out byte[] result, uint[] propertyTags, uint cpid, out uint numberOfRows, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			byte* ptr3 = null;
			uint* ptr4 = null;
			result = null;
			numberOfRows = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propertyTags != null)
				{
					num4 = propertyTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr4 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr4)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr4 / 4] = (int)propertyTags[num6];
				}
				_GUID guid = <Module>.ToGUID(ref database);
				int num7 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetLogonTable50(base.BindingHandle, &guid, lParam, &ptr3, &num2, ptr4, num5, cpid, &num3, num7, ptr, &num8, &ptr2);
				byte[] array;
				if (num8 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num8, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				if (0 == num)
				{
					numberOfRows = num3;
					byte[] array2;
					if (num2 > 0)
					{
						IntPtr uPtrData2 = new IntPtr((void*)ptr3);
						array2 = <Module>.UToMBytes(num2, uPtrData2);
					}
					else
					{
						array2 = null;
					}
					result = array2;
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetLogonTable40");
			}
			finally
			{
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcMountDatabase50(Guid database, uint flags, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				Guid empty = Guid.Empty;
				_GUID guid = <Module>.ToGUID(ref empty);
				_GUID guid2 = <Module>.ToGUID(ref database);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcMountDatabase50(base.BindingHandle, &guid, &guid2, flags, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcMountDatabase40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcUnmountDatabase50(Guid database, uint flags, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				Guid empty = Guid.Empty;
				_GUID guid = <Module>.ToGUID(ref empty);
				_GUID guid2 = <Module>.ToGUID(ref database);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcUnmountDatabase50(base.BindingHandle, &guid, &guid2, (int)flags, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcUnmountDatabase40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcStartBlockModeReplicationToPassive50(Guid dbGuid, string passiveName, uint firstGenToSend, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			ushort* ptr3 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref dbGuid);
				ptr3 = <Module>.StringToUnmanaged(passiveName);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcStartBlockModeReplicationToPassive50(base.BindingHandle, &guid, ptr3, firstGenToSend, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcStartBlockModeReplicationToPassive50");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminSetMailboxBasicInfo50(Guid database, Guid mailbox, byte[] mailboxInfo, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			byte* ptr3 = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num = 0;
				ptr3 = <Module>.MToUBytesClient(mailboxInfo, &num);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				result = <Module>.cli_EcAdminSetMailboxBasicInfo50(base.BindingHandle, &guid, &guid2, ptr3, num, num2, ptr, &num3, &ptr2);
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetMailboxBasicInfo40");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcPurgeCachedMdbObject50(Guid database, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcPurgeCachedMdbObject50(base.BindingHandle, &guid, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcPurgeCachedMdbObject40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetMailboxTable50(Guid? database, int lParam, out byte[] result, uint[] propertyTags, uint cpid, out uint numberOfRows, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			byte* ptr3 = null;
			uint* ptr4 = null;
			result = null;
			numberOfRows = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				_GUID* ptr5 = null;
				if (database != null)
				{
					Guid value = database.Value;
					_GUID guid = <Module>.ToGUID(ref value);
					ptr5 = &guid;
				}
				int num4;
				if (propertyTags != null)
				{
					num4 = propertyTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr4 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr4)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr4 / 4] = (int)propertyTags[num6];
				}
				int num7 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetMailboxTable50(base.BindingHandle, ptr5, lParam, &ptr3, &num2, ptr4, num5, cpid, &num3, num7, ptr, &num8, &ptr2);
				byte[] array;
				if (num8 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num8, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				if (0 == num)
				{
					numberOfRows = num3;
					byte[] array2;
					if (num2 > 0)
					{
						IntPtr uPtrData2 = new IntPtr((void*)ptr3);
						array2 = <Module>.UToMBytes(num2, uPtrData2);
					}
					else
					{
						array2 = null;
					}
					result = array2;
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetMailboxTable40");
			}
			finally
			{
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminNotifyOnDSChange50(Guid database, Guid mailbox, uint objectType, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcAdminNotifyOnDSChange50(base.BindingHandle, &guid, &guid2, objectType, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminNotifyOnDSChange40");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcReadMdbEvents50(Guid database, [In] [Out] ref Guid databaseVersion, byte[] request, out byte[] response, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			response = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				Guid guid2 = databaseVersion;
				_GUID guid3 = <Module>.ToGUID(ref guid2);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(request, &num2);
				int num3 = 0;
				ptr3 = <Module>.MToUBytesClient(auxIn, &num3);
				uint num4 = 0;
				uint num5 = 0;
				num = <Module>.cli_EcReadMdbEvents50(base.BindingHandle, &guid, &guid3, num2, ptr, &num4, &ptr2, num3, ptr3, &num5, &ptr4);
				if (0 == num)
				{
					Guid guid4 = <Module>.FromGUID(ref guid3);
					databaseVersion = guid4;
					byte[] array;
					if (num4 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr2);
						array = <Module>.UToMBytes(num4, uPtrData);
					}
					else
					{
						array = null;
					}
					response = array;
				}
				byte[] array2;
				if (num5 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array2 = <Module>.UToMBytes(num5, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcReadMdbEvents50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcSyncMailboxWithDS50(Guid database, Guid mailbox, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcSyncMailboxWithDS50(base.BindingHandle, &guid, &guid2, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcSyncMailboxWithDS50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcDeleteMdbWatermarksForConsumer50(Guid database, [In] [Out] ref Guid databaseVersion, Guid? mailbox, Guid consumer, out uint deletedCount, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			deletedCount = 0U;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				Guid guid2 = databaseVersion;
				_GUID guid3 = <Module>.ToGUID(ref guid2);
				_GUID guid4 = <Module>.ToGUID(ref consumer);
				_GUID* ptr3 = null;
				if (mailbox != null)
				{
					Guid value = mailbox.Value;
					_GUID guid5 = <Module>.ToGUID(ref value);
					ptr3 = &guid5;
				}
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				uint num4 = 0;
				num = <Module>.cli_EcDeleteMdbWatermarksForConsumer50(base.BindingHandle, &guid, &guid3, ptr3, &guid4, &num4, num2, ptr, &num3, &ptr2);
				if (0 == num)
				{
					Guid guid6 = <Module>.FromGUID(ref guid3);
					databaseVersion = guid6;
					deletedCount = num4;
				}
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcDeleteMdbWatermarksForConsumer50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcDeleteMdbWatermarksForMailbox50(Guid database, [In] [Out] ref Guid databaseVersion, Guid mailbox, out uint deletedCount, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			deletedCount = 0U;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				Guid guid2 = databaseVersion;
				_GUID guid3 = <Module>.ToGUID(ref guid2);
				_GUID guid4 = <Module>.ToGUID(ref mailbox);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				uint num4 = 0;
				num = <Module>.cli_EcDeleteMdbWatermarksForMailbox50(base.BindingHandle, &guid, &guid3, &guid4, &num4, num2, ptr, &num3, &ptr2);
				if (0 == num)
				{
					Guid guid5 = <Module>.FromGUID(ref guid3);
					databaseVersion = guid5;
					deletedCount = num4;
				}
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcDeleteMdbWatermarksForMailbox50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcSaveMdbWatermarks50(Guid database, [In] [Out] ref Guid databaseVersion, MDBEVENTWM[] watermarks, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			__MIDL_mdbadmin50_0002* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				Guid guid2 = databaseVersion;
				_GUID guid3 = <Module>.ToGUID(ref guid2);
				int num2;
				if (watermarks != null)
				{
					num2 = watermarks.Length;
				}
				else
				{
					num2 = 0;
				}
				uint num3 = num2;
				ptr = <Module>.MIDL_user_allocate((ulong)num2 * 40UL);
				if (null == ptr)
				{
					throw new OutOfMemoryException();
				}
				for (uint num4 = 0; num4 < num3; num4++)
				{
					MDBEVENTWM mdbeventwm = watermarks[num4];
					_GUID guid4 = <Module>.ToGUID(ref mdbeventwm.MailboxGuid);
					__MIDL_mdbadmin50_0002* ptr4 = num4 * 40L + ptr / sizeof(__MIDL_mdbadmin50_0002);
					cpblk(ptr4, ref guid4, 16);
					_GUID guid5 = <Module>.ToGUID(ref mdbeventwm.ConsumerGuid);
					cpblk(ptr4 + 16L / (long)sizeof(__MIDL_mdbadmin50_0002), ref guid5, 16);
					*(long*)(ptr4 + 32L / (long)sizeof(__MIDL_mdbadmin50_0002)) = mdbeventwm.EventCounter;
				}
				int num5 = 0;
				ptr2 = <Module>.MToUBytesClient(auxIn, &num5);
				uint num6 = 0;
				num = <Module>.cli_EcSaveMdbWatermarks50(base.BindingHandle, &guid, &guid3, num3, ptr, num5, ptr2, &num6, &ptr3);
				if (0 == num)
				{
					Guid guid6 = <Module>.FromGUID(ref guid3);
					databaseVersion = guid6;
				}
				byte[] array;
				if (num6 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr3);
					array = <Module>.UToMBytes(num6, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcSaveMdbWatermarks50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcGetMdbWatermarksForConsumer50(Guid database, [In] [Out] ref Guid databaseVersion, Guid? mailbox, Guid consumer, out MDBEVENTWM[] watermarks, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			__MIDL_mdbadmin50_0002* ptr3 = null;
			watermarks = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				Guid guid2 = databaseVersion;
				_GUID guid3 = <Module>.ToGUID(ref guid2);
				_GUID guid4 = <Module>.ToGUID(ref consumer);
				_GUID* ptr4 = null;
				if (mailbox != null)
				{
					Guid value = mailbox.Value;
					_GUID guid5 = <Module>.ToGUID(ref value);
					ptr4 = &guid5;
				}
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				uint num4 = 0;
				num = <Module>.cli_EcGetMdbWatermarksForConsumer50(base.BindingHandle, &guid, &guid3, ptr4, &guid4, &num4, &ptr3, num2, ptr, &num3, &ptr2);
				if (0 == num)
				{
					Guid guid6 = <Module>.FromGUID(ref guid3);
					databaseVersion = guid6;
					if (num4 > 0)
					{
						watermarks = new MDBEVENTWM[num4];
						for (uint num5 = 0; num5 < num4; num5++)
						{
							MDBEVENTWM mdbeventwm = default(MDBEVENTWM);
							long num6 = (long)(num5 * 40UL);
							Guid mailboxGuid = <Module>.FromGUID(num6 / (long)sizeof(__MIDL_mdbadmin50_0002) + ptr3);
							mdbeventwm.MailboxGuid = mailboxGuid;
							Guid consumerGuid = <Module>.FromGUID(ptr3 + num6 / (long)sizeof(__MIDL_mdbadmin50_0002) + 16L / (long)sizeof(__MIDL_mdbadmin50_0002));
							mdbeventwm.ConsumerGuid = consumerGuid;
							mdbeventwm.EventCounter = *(long*)(ptr3 + num6 / (long)sizeof(__MIDL_mdbadmin50_0002) + 32L / (long)sizeof(__MIDL_mdbadmin50_0002));
							watermarks[num5] = mdbeventwm;
						}
					}
				}
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcGetMdbWatermarksForConsumer50");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcGetMdbWatermarksForMailbox50(Guid database, [In] [Out] ref Guid databaseVersion, Guid mailbox, out MDBEVENTWM[] watermarks, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			__MIDL_mdbadmin50_0002* ptr3 = null;
			watermarks = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				Guid guid2 = databaseVersion;
				_GUID guid3 = <Module>.ToGUID(ref guid2);
				_GUID guid4 = <Module>.ToGUID(ref mailbox);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				uint num4 = 0;
				num = <Module>.cli_EcGetMdbWatermarksForMailbox50(base.BindingHandle, &guid, &guid3, &guid4, &num4, &ptr3, num2, ptr, &num3, &ptr2);
				if (0 == num)
				{
					Guid guid5 = <Module>.FromGUID(ref guid3);
					databaseVersion = guid5;
					if (num4 > 0)
					{
						watermarks = new MDBEVENTWM[num4];
						for (uint num5 = 0; num5 < num4; num5++)
						{
							MDBEVENTWM mdbeventwm = default(MDBEVENTWM);
							long num6 = (long)(num5 * 40UL);
							Guid mailboxGuid = <Module>.FromGUID(num6 / (long)sizeof(__MIDL_mdbadmin50_0002) + ptr3);
							mdbeventwm.MailboxGuid = mailboxGuid;
							Guid consumerGuid = <Module>.FromGUID(ptr3 + num6 / (long)sizeof(__MIDL_mdbadmin50_0002) + 16L / (long)sizeof(__MIDL_mdbadmin50_0002));
							mdbeventwm.ConsumerGuid = consumerGuid;
							mdbeventwm.EventCounter = *(long*)(ptr3 + num6 / (long)sizeof(__MIDL_mdbadmin50_0002) + 32L / (long)sizeof(__MIDL_mdbadmin50_0002));
							watermarks[num5] = mdbeventwm;
						}
					}
				}
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcGetMdbWatermarksForMailbox50");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcWriteMdbEvents50(Guid database, [In] [Out] ref Guid databaseVersion, byte[] request, out byte[] response, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			response = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				Guid guid2 = databaseVersion;
				_GUID guid3 = <Module>.ToGUID(ref guid2);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(request, &num2);
				int num3 = 0;
				ptr3 = <Module>.MToUBytesClient(auxIn, &num3);
				uint num4 = 0;
				uint num5 = 0;
				num = <Module>.cli_EcWriteMdbEvents50(base.BindingHandle, &guid, &guid3, num2, ptr, &num4, &ptr2, num3, ptr3, &num5, &ptr4);
				if (0 == num)
				{
					Guid guid4 = <Module>.FromGUID(ref guid3);
					databaseVersion = guid4;
					byte[] array;
					if (num4 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr2);
						array = <Module>.UToMBytes(num4, uPtrData);
					}
					else
					{
						array = null;
					}
					response = array;
				}
				byte[] array2;
				if (num5 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array2 = <Module>.UToMBytes(num5, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcWriteMdbEvents50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcDoMaintenanceTask50(Guid database, uint task, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcDoMaintenanceTask50(base.BindingHandle, &guid, task, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcDoMaintenanceTask50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcGetLastBackupInfo50(Guid database, out long lastCompleteBackupTime, out long lastIncrementalBackupTime, out long lastDifferentialBackupTime, out long lastCopyBackupTime, out int snapFull, out int snapIncremental, out int snapDifferential, out int snapCopy, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			lastCompleteBackupTime = 0L;
			lastIncrementalBackupTime = 0L;
			lastDifferentialBackupTime = 0L;
			lastCopyBackupTime = 0L;
			snapFull = 0;
			snapIncremental = 0;
			snapDifferential = 0;
			snapCopy = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_FILETIME filetime = 0;
				initblk(ref filetime + 4, 0, 4L);
				_FILETIME filetime2 = 0;
				initblk(ref filetime2 + 4, 0, 4L);
				_FILETIME filetime3 = 0;
				initblk(ref filetime3 + 4, 0, 4L);
				_FILETIME filetime4 = 0;
				initblk(ref filetime4 + 4, 0, 4L);
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				_GUID guid = <Module>.ToGUID(ref database);
				int num5 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num5);
				uint num6 = 0;
				result = <Module>.cli_EcGetLastBackupInfoEx50(base.BindingHandle, &guid, &filetime, &filetime2, &filetime3, &filetime4, &num, &num2, &num3, &num4, num5, ptr, &num6, &ptr2);
				byte[] array;
				if (num6 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num6, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				lastCompleteBackupTime = filetime;
				lastIncrementalBackupTime = filetime2;
				lastDifferentialBackupTime = filetime3;
				lastCopyBackupTime = filetime4;
				snapFull = num;
				snapIncremental = num2;
				snapDifferential = num3;
				snapCopy = num4;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcGetLastBackupInfo50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetMailboxTableEntry50(Guid database, Guid mailbox, uint[] propertyTags, uint cpid, out byte[] result, out uint numberOfRows, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			auxOut = null;
			result = null;
			numberOfRows = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propertyTags != null)
				{
					num4 = propertyTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr2 / 4] = (int)propertyTags[num6];
				}
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num7 = 0;
				ptr3 = <Module>.MToUBytesClient(auxIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetMailboxTableEntry50(base.BindingHandle, &guid, &guid2, &ptr, &num2, ptr2, num5, cpid, &num3, num7, ptr3, &num8, &ptr4);
				if (0 == num)
				{
					numberOfRows = num3;
					byte[] array;
					if (num2 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array = <Module>.UToMBytes(num2, uPtrData);
					}
					else
					{
						array = null;
					}
					result = array;
				}
				byte[] array2;
				if (num8 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array2 = <Module>.UToMBytes(num8, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetMailboxTableEntry50");
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetMailboxTableEntryFlags50(Guid database, Guid mailbox, uint flags, uint[] propertyTags, uint cpid, out byte[] result, out uint numberOfRows, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			auxOut = null;
			result = null;
			numberOfRows = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propertyTags != null)
				{
					num4 = propertyTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr2 / 4] = (int)propertyTags[num6];
				}
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num7 = 0;
				ptr3 = <Module>.MToUBytesClient(auxIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetMailboxTableEntryFlags50(base.BindingHandle, &guid, &guid2, flags, &ptr, &num2, ptr2, num5, cpid, &num3, num7, ptr3, &num8, &ptr4);
				if (0 == num)
				{
					numberOfRows = num3;
					byte[] array;
					if (num2 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array = <Module>.UToMBytes(num2, uPtrData);
					}
					else
					{
						array = null;
					}
					result = array;
				}
				byte[] array2;
				if (num8 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array2 = <Module>.UToMBytes(num8, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetMailboxTableEntryFlags50");
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetRestrictionTable50(Guid database, int lParam, byte[] entryId, out byte[] result, uint[] propertyTags, out uint numberOfRows)
		{
			int num = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			result = null;
			numberOfRows = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4 = 0;
				ptr3 = <Module>.MToUBytesClient(entryId, &num4);
				int num5;
				if (propertyTags != null)
				{
					num5 = propertyTags.Length;
				}
				else
				{
					num5 = 0;
				}
				uint num6 = num5;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num5 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num7 = 0; num7 < num6; num7++)
				{
					(num7 * 4L)[ptr2 / 4] = (int)propertyTags[num7];
				}
				_GUID guid = <Module>.ToGUID(ref database);
				num = <Module>.cli_EcAdminGetRestrictionTable50(base.BindingHandle, &guid, lParam, num4, ptr3, &ptr, &num2, ptr2, num6, &num3);
				if (0 == num)
				{
					numberOfRows = num3;
					byte[] array;
					if (num2 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array = <Module>.UToMBytes(num2, uPtrData);
					}
					else
					{
						array = null;
					}
					result = array;
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetRestrictionTable50");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcLogReplayRequest2(Guid database, uint logReplayMax, uint logReplayFlags, out uint logReplayNext, out byte[] databaseInfo, out uint patchPageNumber, out byte[] patchToken, out byte[] patchData, out uint[] corruptPages, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			uint num2 = 0;
			uint num3 = 0;
			byte* ptr3 = null;
			uint num4 = 0;
			uint num5 = 0;
			byte* ptr4 = null;
			uint num6 = 0;
			byte* ptr5 = null;
			uint num7 = 0;
			uint* ptr6 = null;
			logReplayNext = 0U;
			databaseInfo = null;
			patchPageNumber = 0U;
			patchToken = null;
			patchData = null;
			corruptPages = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				int num8 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num8);
				uint num9 = 0;
				num = <Module>.cli_EcLogReplayRequestEx2(base.BindingHandle, &guid, logReplayMax, logReplayFlags, &num2, &num3, &ptr3, &num4, &num5, &ptr4, &num6, &ptr5, &num7, &ptr6, num8, ptr, &num9, &ptr2);
				byte[] array;
				if (num9 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num9, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				if (0 == num)
				{
					logReplayNext = num2;
					patchPageNumber = num4;
					if (num3 > 0)
					{
						IntPtr uPtrData2 = new IntPtr((void*)ptr3);
						databaseInfo = <Module>.UToMBytes(num3, uPtrData2);
					}
					if (num5 > 0)
					{
						IntPtr uPtrData3 = new IntPtr((void*)ptr4);
						patchToken = <Module>.UToMBytes(num5, uPtrData3);
					}
					if (num6 > 0)
					{
						IntPtr uPtrData4 = new IntPtr((void*)ptr5);
						patchData = <Module>.UToMBytes(num6, uPtrData4);
					}
					if (num7 > 0)
					{
						corruptPages = <Module>.UToMUInt32(num7, ptr6);
					}
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcLogReplayRequest2");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr5 != null)
				{
					<Module>.MIDL_user_free((void*)ptr5);
				}
				if (ptr6 != null)
				{
					<Module>.MIDL_user_free((void*)ptr6);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetViewsTableEx50(Guid mdbGuid, Guid mailboxGuid, LTID folderLTID, uint[] propTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int num = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			auxiliaryOut = null;
			result = null;
			rowCount = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propTags != null)
				{
					num4 = propTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr2 / 4] = (int)propTags[num6];
				}
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref mailboxGuid);
				_GUID guid3 = <Module>.ToGUID(ref folderLTID.guid);
				_ltid ltid;
				cpblk(ref ltid, ref guid3, 16);
				ulong globCount = folderLTID.globCount;
				*(ref ltid + 16) = (byte)((uint)(globCount >> 40));
				*(ref ltid + 17) = (byte)((uint)(globCount >> 32));
				*(ref ltid + 18) = (byte)((uint)(globCount >> 24));
				*(ref ltid + 19) = (byte)((uint)(globCount >> 16));
				*(ref ltid + 20) = (byte)((uint)(globCount >> 8));
				*(ref ltid + 21) = (byte)((uint)globCount);
				int num7 = 0;
				ptr3 = <Module>.MToUBytesClient(auxiliaryIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetViewsTableEx50(base.BindingHandle, &guid, &guid2, 0, ltid, &ptr, &num2, ptr2, num5, &num3, num7, ptr3, &num8, &ptr4);
				if (0 == num)
				{
					rowCount = num3;
					byte[] array;
					if (num2 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array = <Module>.UToMBytes(num2, uPtrData);
					}
					else
					{
						array = null;
					}
					result = array;
				}
				byte[] array2;
				if (num8 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array2 = <Module>.UToMBytes(num8, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxiliaryOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetViewsTableEx50");
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetRestrictionTableEx50(Guid mdbGuid, Guid mailboxGuid, LTID folderLTID, uint[] propTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int num = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			auxiliaryOut = null;
			result = null;
			rowCount = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propTags != null)
				{
					num4 = propTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr2 / 4] = (int)propTags[num6];
				}
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref mailboxGuid);
				_GUID guid3 = <Module>.ToGUID(ref folderLTID.guid);
				_ltid ltid;
				cpblk(ref ltid, ref guid3, 16);
				ulong globCount = folderLTID.globCount;
				*(ref ltid + 16) = (byte)((uint)(globCount >> 40));
				*(ref ltid + 17) = (byte)((uint)(globCount >> 32));
				*(ref ltid + 18) = (byte)((uint)(globCount >> 24));
				*(ref ltid + 19) = (byte)((uint)(globCount >> 16));
				*(ref ltid + 20) = (byte)((uint)(globCount >> 8));
				*(ref ltid + 21) = (byte)((uint)globCount);
				int num7 = 0;
				ptr3 = <Module>.MToUBytesClient(auxiliaryIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetRestrictionTableEx50(base.BindingHandle, &guid, &guid2, 0, ltid, &ptr, &num2, ptr2, num5, &num3, num7, ptr3, &num8, &ptr4);
				if (0 == num)
				{
					rowCount = num3;
					byte[] array;
					if (num2 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array = <Module>.UToMBytes(num2, uPtrData);
					}
					else
					{
						array = null;
					}
					result = array;
				}
				byte[] array2;
				if (num8 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array2 = <Module>.UToMBytes(num8, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxiliaryOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetRestrictionTableEx50");
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminExecuteTask50(Guid mdbGuid, Guid taskClassGuid, int lTaskId, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxiliaryOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref taskClassGuid);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxiliaryIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcAdminExecuteTask50(base.BindingHandle, &guid, &guid2, lTaskId, 0, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxiliaryOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminExecuteTask50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetFeatureVersion50(uint feature, ref uint version, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				uint num = 0;
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				result = <Module>.cli_EcAdminGetFeatureVersionEx50(base.BindingHandle, feature, &num, num2, ptr, &num3, &ptr2);
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				version = num;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetFeatureVersion50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetMailboxSignature50(Guid database, Guid mailbox, uint flags, out byte[] mailboxSignature, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			mailboxSignature = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				uint num2 = 0;
				int num3 = 0;
				ptr2 = <Module>.MToUBytesClient(auxIn, &num3);
				uint num4 = 0;
				num = <Module>.cli_EcAdminGetMailboxSignatureBasicInfo50(base.BindingHandle, &guid, &guid2, flags, &num2, &ptr, num3, ptr2, &num4, &ptr3);
				if (0 == num)
				{
					byte[] array;
					if (num2 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array = <Module>.UToMBytes(num2, uPtrData);
					}
					else
					{
						array = null;
					}
					mailboxSignature = array;
				}
				byte[] array2;
				if (num4 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr3);
					array2 = <Module>.UToMBytes(num4, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetMailboxSignature50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminISIntegCheck50(Guid database, Guid mailbox, uint flags, uint[] taskIds, out string requestId, [Out] byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			uint* ptr3 = null;
			byte* ptr4 = null;
			auxOut = null;
			try
			{
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num2);
				_GUID guid = <Module>.ToGUID(ref database);
				_GUID guid2 = <Module>.ToGUID(ref mailbox);
				int num3 = 0;
				<Module>.MToUUint32(taskIds, &num3, &ptr3);
				uint num4 = 0;
				num = <Module>.cli_EcAdminISIntegCheck50(base.BindingHandle, &guid, &guid2, flags, num3, ptr3, &ptr4, num2, ptr, &num4, &ptr2);
				if (0 == num)
				{
					requestId = new string((sbyte*)ptr4);
				}
				byte[] array;
				if (num4 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num4, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminISIntegCheck50");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcMultiMailboxSearch(Guid mdbGuid, byte[] searchRequest, out byte[] searchResponse, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			try
			{
				if (searchRequest == null)
				{
					num = 47368;
					return 47368;
				}
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxiliaryIn, &num2);
				int num3 = 0;
				ptr3 = <Module>.MToUBytesClient(searchRequest, &num3);
				uint num4 = 0;
				uint num5 = 0;
				num = <Module>.cli_EcMultiMailboxSearch(base.BindingHandle, &guid, num3, ptr3, &num5, &ptr4, num2, ptr, &num4, &ptr2);
				if (0 == num)
				{
					byte[] array;
					if (num4 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr2);
						array = <Module>.UToMBytes(num4, uPtrData);
					}
					else
					{
						array = null;
					}
					auxiliaryOut = array;
					byte[] array2;
					if (num5 > 0)
					{
						IntPtr uPtrData2 = new IntPtr((void*)ptr4);
						array2 = <Module>.UToMBytes(num5, uPtrData2);
					}
					else
					{
						array2 = null;
					}
					searchResponse = array2;
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcMultiMailboxSearch");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcGetMultiMailboxSearchKeywordStats(Guid mdbGuid, byte[] searchRequest, out byte[] searchResponse, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			try
			{
				if (searchRequest == null)
				{
					num = 63752;
					return 63752;
				}
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(auxiliaryIn, &num2);
				int num3 = 0;
				ptr3 = <Module>.MToUBytesClient(searchRequest, &num3);
				uint num4 = 0;
				uint num5 = 0;
				num = <Module>.cli_EcGetMultiMailboxSearchKeywordStats(base.BindingHandle, &guid, num3, ptr3, &num5, &ptr4, num2, ptr, &num4, &ptr2);
				if (0 == num)
				{
					byte[] array;
					if (num4 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr2);
						array = <Module>.UToMBytes(num4, uPtrData);
					}
					else
					{
						array = null;
					}
					auxiliaryOut = array;
					byte[] array2;
					if (num5 > 0)
					{
						IntPtr uPtrData2 = new IntPtr((void*)ptr4);
						array2 = <Module>.UToMBytes(num5, uPtrData2);
					}
					else
					{
						array2 = null;
					}
					searchResponse = array2;
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcGetMultiMailboxSearchKeywordStats");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetResourceMonitorDigest50(Guid mdbGuid, uint[] propertyTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int num = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			result = null;
			rowCount = 0U;
			auxiliaryOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propertyTags != null)
				{
					num4 = propertyTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr2 / 4] = (int)propertyTags[num6];
				}
				int num7 = 0;
				ptr3 = <Module>.MToUBytesClient(auxiliaryIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetResourceMonitorDigest50(base.BindingHandle, &guid, &ptr, &num2, ptr2, num5, &num3, num7, ptr3, &num8, &ptr4);
				byte[] array;
				if (num8 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr4);
					array = <Module>.UToMBytes(num8, uPtrData);
				}
				else
				{
					array = null;
				}
				auxiliaryOut = array;
				if (0 == num)
				{
					rowCount = num3;
					byte[] array2;
					if (num2 > 0)
					{
						IntPtr uPtrData2 = new IntPtr((void*)ptr);
						array2 = <Module>.UToMBytes(num2, uPtrData2);
					}
					else
					{
						array2 = null;
					}
					result = array2;
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetResourceMonitorDigest50");
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetDatabaseProcessInfo50(Guid mdbGuid, uint[] propertyTags, out byte[] result, out uint rowCount, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			uint* ptr4 = null;
			auxiliaryOut = null;
			result = null;
			rowCount = 0U;
			try
			{
				uint num2 = 0;
				uint num3 = 0;
				int num4;
				if (propertyTags != null)
				{
					num4 = propertyTags.Length;
				}
				else
				{
					num4 = 0;
				}
				uint num5 = num4;
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				ptr4 = <Module>.MIDL_user_allocate((ulong)num4 * 4UL);
				if (null == ptr4)
				{
					throw new OutOfMemoryException();
				}
				for (uint num6 = 0; num6 < num5; num6++)
				{
					(num6 * 4L)[ptr4 / 4] = (int)propertyTags[num6];
				}
				int num7 = 0;
				ptr2 = <Module>.MToUBytesClient(auxiliaryIn, &num7);
				uint num8 = 0;
				num = <Module>.cli_EcAdminGetDatabaseProcessInfo50(base.BindingHandle, &guid, num5, ptr4, &ptr, &num2, &num3, num7, ptr2, &num8, &ptr3);
				if (0 == num)
				{
					rowCount = num3;
					byte[] array;
					if (num2 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array = <Module>.UToMBytes(num2, uPtrData);
					}
					else
					{
						array = null;
					}
					result = array;
				}
				byte[] array2;
				if (num8 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr3);
					array2 = <Module>.UToMBytes(num8, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxiliaryOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetDatabaseProcessInfo50");
			}
			finally
			{
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminProcessSnapshotOperation50(Guid mdbGuid, [In] uint operationCode, [In] uint flags, byte[] auxiliaryIn, out byte[] auxiliaryOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxiliaryOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxiliaryIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcAdminProcessSnapshotOperation50(base.BindingHandle, &guid, operationCode, flags, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxiliaryOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetDatabaseProcessInfo50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminGetPhysicalDatabaseInformation50(Guid database, out byte[] databaseInfo, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			uint num2 = 0;
			byte* ptr3 = null;
			databaseInfo = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref database);
				int num3 = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num3);
				uint num4 = 0;
				num = <Module>.cli_EcAdminGetPhysicalDatabaseInformation50(base.BindingHandle, &guid, &num2, &ptr3, num3, ptr, &num4, &ptr2);
				byte[] array;
				if (num4 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num4, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
				if (0 == num && num2 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr3);
					databaseInfo = <Module>.UToMBytes(num2, uPtrData2);
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminGetPhysicalDatabaseInformation50");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminPrePopulateCacheEx50(Guid mdbGuid, Guid mailboxGuid, byte[] partitionHint, string dcName, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref mailboxGuid);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				int num3 = 0;
				ptr3 = <Module>.MToUBytesClient(partitionHint, &num3);
				if (dcName != null)
				{
					ptr4 = (byte*)<Module>.StringToUnmanagedMultiByte(dcName, 0U);
				}
				result = <Module>.cli_EcAdminPrePopulateCacheEx50(base.BindingHandle, &guid, &guid2, num3, ptr3, ptr4, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcAdminPrePopulateCacheEx50");
			}
			finally
			{
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcForceNewLog50(Guid mdbGuid, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcForceNewLog50(base.BindingHandle, &guid, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcForceNewLog50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcAdminIntegrityCheckEx50(Guid mdbGuid, Guid mailboxGuid, uint operation, byte[] request, out byte[] response, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			response = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref mailboxGuid);
				int num2 = 0;
				ptr = <Module>.MToUBytesClient(request, &num2);
				int num3 = 0;
				ptr3 = <Module>.MToUBytesClient(auxIn, &num3);
				uint num4 = 0;
				uint num5 = 0;
				num = <Module>.cli_EcAdminIntegrityCheckEx50(base.BindingHandle, &guid, &guid2, operation, num2, ptr, &num4, &ptr2, num3, ptr3, &num5, &ptr4);
				if (0 == num)
				{
					byte[] array;
					if (num4 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr2);
						array = <Module>.UToMBytes(num4, uPtrData);
					}
					else
					{
						array = null;
					}
					response = array;
				}
				byte[] array2;
				if (num5 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array2 = <Module>.UToMBytes(num5, uPtrData2);
				}
				else
				{
					array2 = null;
				}
				auxOut = array2;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcReadMdbEvents50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcCreateUserInfo50(Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] properties, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref userInfoGuid);
				int num = 0;
				ptr = <Module>.MToUBytesClient(properties, &num);
				int num2 = 0;
				ptr2 = <Module>.MToUBytesClient(auxIn, &num2);
				uint num3 = 0;
				result = <Module>.cli_EcCreateUserInfo50(base.BindingHandle, &guid, &guid2, flags, num, ptr, num2, ptr2, &num3, &ptr3);
				byte[] array;
				if (num3 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr3);
					array = <Module>.UToMBytes(num3, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcCreateUserInfo50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcReadUserInfo50(Guid mdbGuid, Guid userInfoGuid, uint flags, uint[] propertyTags, out ArraySegment<byte> result, byte[] auxIn, out byte[] auxOut)
		{
			int num = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			auxOut = null;
			ArraySegment<byte> arraySegment = new ArraySegment<byte>(new byte[0]);
			result = arraySegment;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref userInfoGuid);
				int num2;
				if (propertyTags != null)
				{
					num2 = propertyTags.Length;
				}
				else
				{
					num2 = 0;
				}
				uint num3 = num2;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num2 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num4 = 0; num4 < num3; num4++)
				{
					(num4 * 4L)[ptr2 / 4] = (int)propertyTags[num4];
				}
				int num5 = 0;
				ptr3 = <Module>.MToUBytesClient(auxIn, &num5);
				uint num6 = 0;
				uint num7 = 0;
				num = <Module>.cli_EcReadUserInfo50(base.BindingHandle, &guid, &guid2, flags, num3, ptr2, &num7, &ptr, num5, ptr3, &num6, &ptr4);
				if (0 == num && ptr != null && num7 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr);
					ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(<Module>.UToMBytes(num7, uPtrData));
					result = arraySegment2;
				}
				byte[] array;
				if (num6 > 0)
				{
					IntPtr uPtrData2 = new IntPtr((void*)ptr4);
					array = <Module>.UToMBytes(num6, uPtrData2);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcReadUserInfo50");
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcUpdateUserInfo50(Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] properties, uint[] deletePropertyTags, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			uint* ptr2 = null;
			byte* ptr3 = null;
			byte* ptr4 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref userInfoGuid);
				int num = 0;
				ptr = <Module>.MToUBytesClient(properties, &num);
				int num2;
				if (deletePropertyTags != null)
				{
					num2 = deletePropertyTags.Length;
				}
				else
				{
					num2 = 0;
				}
				uint num3 = num2;
				ptr2 = <Module>.MIDL_user_allocate((ulong)num2 * 4UL);
				if (null == ptr2)
				{
					throw new OutOfMemoryException();
				}
				for (uint num4 = 0; num4 < num3; num4++)
				{
					(num4 * 4L)[ptr2 / 4] = (int)deletePropertyTags[num4];
				}
				int num5 = 0;
				ptr3 = <Module>.MToUBytesClient(auxIn, &num5);
				uint num6 = 0;
				result = <Module>.cli_EcUpdateUserInfo50(base.BindingHandle, &guid, &guid2, flags, num, ptr, num3, ptr2, num5, ptr3, &num6, &ptr4);
				byte[] array;
				if (num6 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr4);
					array = <Module>.UToMBytes(num6, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcUpdateUserInfo50");
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcDeleteUserInfo50(Guid mdbGuid, Guid userInfoGuid, uint flags, byte[] auxIn, out byte[] auxOut)
		{
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			auxOut = null;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mdbGuid);
				_GUID guid2 = <Module>.ToGUID(ref userInfoGuid);
				int num = 0;
				ptr = <Module>.MToUBytesClient(auxIn, &num);
				uint num2 = 0;
				result = <Module>.cli_EcDeleteUserInfo50(base.BindingHandle, &guid, &guid2, flags, num, ptr, &num2, &ptr2);
				byte[] array;
				if (num2 > 0)
				{
					IntPtr uPtrData = new IntPtr((void*)ptr2);
					array = <Module>.UToMBytes(num2, uPtrData);
				}
				else
				{
					array = null;
				}
				auxOut = array;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EcDeleteUserInfo50");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe IRpcAsyncResult InternalBeginEcListAllMdbStatus50([MarshalAs(UnmanagedType.U1)] bool fBasicInformation, int cbAuxIn, byte* pbAuxIn, AsyncCallback callback, object state)
		{
			EcListAllMdbStatus50AsyncResult ecListAllMdbStatus50AsyncResult = null;
			EcListAllMdbStatus50AsyncResult result = null;
			GCHandle value = default(GCHandle);
			bool flag = false;
			try
			{
				ecListAllMdbStatus50AsyncResult = new EcListAllMdbStatus50AsyncResult(callback, state);
				value = GCHandle.Alloc(ecListAllMdbStatus50AsyncResult);
				flag = true;
				int num = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)ecListAllMdbStatus50AsyncResult.NativeState(), 112U);
				if (num != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcListAllMdbStatus50, RpcAsyncInitializeHandle");
				}
				*(long*)(ecListAllMdbStatus50AsyncResult.NativeState() + 24L / (long)sizeof(EcListAllMdbStatus50AsyncState)) = 0L;
				*(int*)(ecListAllMdbStatus50AsyncResult.NativeState() + 44L / (long)sizeof(EcListAllMdbStatus50AsyncState)) = 1;
				IntPtr handle = ecListAllMdbStatus50AsyncResult.AsyncWaitHandle.Handle;
				*(long*)(ecListAllMdbStatus50AsyncResult.NativeState() + 48L / (long)sizeof(EcListAllMdbStatus50AsyncState)) = handle.ToPointer();
				IntPtr rootedAsyncState = GCHandle.ToIntPtr(value);
				bool flag2 = ecListAllMdbStatus50AsyncResult.RegisterWait(rootedAsyncState);
				try
				{
					<Module>.cli_EcListAllMdbStatus50((_RPC_ASYNC_STATE*)ecListAllMdbStatus50AsyncResult.NativeState(), base.BindingHandle, fBasicInformation ? 1 : 0, (uint*)(ecListAllMdbStatus50AsyncResult.NativeState() + 112L / (long)sizeof(EcListAllMdbStatus50AsyncState)), (uint*)(ecListAllMdbStatus50AsyncResult.NativeState() + 116L / (long)sizeof(EcListAllMdbStatus50AsyncState)), (byte**)(ecListAllMdbStatus50AsyncResult.NativeState() + 120L / (long)sizeof(EcListAllMdbStatus50AsyncState)), cbAuxIn, pbAuxIn, (uint*)(ecListAllMdbStatus50AsyncResult.NativeState() + 128L / (long)sizeof(EcListAllMdbStatus50AsyncState)), (byte**)(ecListAllMdbStatus50AsyncResult.NativeState() + 136L / (long)sizeof(EcListAllMdbStatus50AsyncState)));
					result = ecListAllMdbStatus50AsyncResult;
					ecListAllMdbStatus50AsyncResult = null;
					flag = (((!flag2) ? 1 : 0) != 0);
				}
				catch when (endfilter(true))
				{
					num = Marshal.GetExceptionCode();
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcListAllMdbStatus50");
				}
			}
			finally
			{
				if (null != ecListAllMdbStatus50AsyncResult)
				{
					((IDisposable)ecListAllMdbStatus50AsyncResult).Dispose();
				}
				if (flag)
				{
					value.Free();
				}
			}
			return result;
		}
	}
}
