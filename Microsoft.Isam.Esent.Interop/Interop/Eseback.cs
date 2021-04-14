using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class Eseback
	{
		public Eseback(ESEBACK_CALLBACKS callbacks)
		{
			this.managedEsebackCallbacks = callbacks;
			RuntimeHelpers.PrepareMethod(this.managedEsebackCallbacks.pfnPrepareInstance.Method.MethodHandle);
			RuntimeHelpers.PrepareMethod(this.managedEsebackCallbacks.pfnDoneWithInstance.Method.MethodHandle);
			RuntimeHelpers.PrepareMethod(this.managedEsebackCallbacks.pfnGetDatabasesInfo.Method.MethodHandle);
			RuntimeHelpers.PrepareMethod(this.managedEsebackCallbacks.pfnIsSGReplicated.Method.MethodHandle);
			RuntimeHelpers.PrepareMethod(this.managedEsebackCallbacks.pfnServerAccessCheck.Method.MethodHandle);
			RuntimeHelpers.PrepareMethod(this.managedEsebackCallbacks.pfnTrace.Method.MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackPrepareInstanceForBackup", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackDoneWithInstanceForBackup", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackGetDatabasesInfo", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackFreeDatabasesInfo", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackIsSGReplicated", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackFreeShipLogInfo", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackServerAccessCheck", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			RuntimeHelpers.PrepareMethod(typeof(Eseback).GetMethod("CallbackTrace", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			this.nativeCallbacks = new NATIVE_ESEBACK_CALLBACKS
			{
				pfnPrepareInstance = new NATIVE_PfnErrESECBPrepareInstanceForBackup(this.CallbackPrepareInstanceForBackup),
				pfnDoneWithInstance = new NATIVE_PfnErrESECBDoneWithInstanceForBackup(this.CallbackDoneWithInstance),
				pfnGetDatabasesInfo = new NATIVE_PfnErrESECBGetDatabasesInfo(this.CallbackGetDatabasesInfo),
				pfnFreeDatabasesInfo = new NATIVE_PfnErrESECBFreeDatabasesInfo(this.CallbackFreeDatabasesInfo),
				pfnIsSGReplicated = new NATIVE_PfnErrESECBIsSGReplicated(this.CallbackIsSGReplicated),
				pfnFreeShipLogInfo = new NATIVE_PfnErrESECBFreeShipLogInfo(this.CallbackFreeShipLogInfo),
				pfnServerAccessCheck = new NATIVE_PfnErrESECBServerAccessCheck(this.CallbackServerAccessCheck),
				pfnTrace = new NATIVE_PfnErrESECBTrace(this.CallbackTrace)
			};
			this.callbacksImpl = new NATIVE_ESEBACK_CALLBACKS_IMPL(ref this.nativeCallbacks);
		}

		public static int BackupRestoreUnregister()
		{
			return NativeMethods.HrESEBackupRestoreUnregister();
		}

		public JET_err CallbackDoneWithInstance(IntPtr backupContext, IntPtr instanceId, uint isComplete, IntPtr reserved)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				ESEBACK_CONTEXT eseback_CONTEXT = new ESEBACK_CONTEXT();
				eseback_CONTEXT.SetFromNativeEsebackContext(backupContext);
				JET_INSTANCE ulInstanceId = new JET_INSTANCE
				{
					Value = instanceId
				};
				result = this.managedEsebackCallbacks.pfnDoneWithInstance(eseback_CONTEXT, ulInstanceId, (BackupDone)isComplete);
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackDoneWithInstance");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		public unsafe int BackupRestoreRegister(string displayName, BackupRegisterFlags flags, string annotation, Guid crimsonPublisher)
		{
			int result;
			fixed (NATIVE_ESEBACK_CALLBACKS_IMPL* ptr = &this.callbacksImpl)
			{
				result = NativeMethods.HrESEBackupRestoreRegister3(displayName, (uint)flags, annotation, ptr, &crimsonPublisher);
			}
			return result;
		}

		internal static IntPtr ConvertStructArrayToIntPtr<T>(T[] nativeStructs)
		{
			IntPtr result = IntPtr.Zero;
			int num = (nativeStructs == null) ? 0 : nativeStructs.Length;
			if (num > 0)
			{
				int num2 = Marshal.SizeOf(nativeStructs[0]);
				int cb = num2 * num;
				result = Marshal.AllocHGlobal(cb);
				for (int i = 0; i < num; i++)
				{
					IntPtr ptr = new IntPtr(result.ToInt64() + (long)(num2 * i));
					Marshal.StructureToPtr(nativeStructs[i], ptr, false);
				}
			}
			return result;
		}

		internal JET_err CallbackPrepareInstanceForBackup(IntPtr backupContext, IntPtr instanceId, IntPtr reserved)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				ESEBACK_CONTEXT eseback_CONTEXT = new ESEBACK_CONTEXT();
				eseback_CONTEXT.SetFromNativeEsebackContext(backupContext);
				JET_INSTANCE ulInstanceId = new JET_INSTANCE
				{
					Value = instanceId
				};
				result = this.managedEsebackCallbacks.pfnPrepareInstance(eseback_CONTEXT, ulInstanceId);
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackPrepareInstanceForBackup");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		internal JET_err CallbackDoneWithInstanceForBackup(IntPtr backupContext, IntPtr instanceId, uint isComplete, IntPtr reserved)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				ESEBACK_CONTEXT eseback_CONTEXT = new ESEBACK_CONTEXT();
				eseback_CONTEXT.SetFromNativeEsebackContext(backupContext);
				JET_INSTANCE ulInstanceId = new JET_INSTANCE
				{
					Value = instanceId
				};
				result = this.managedEsebackCallbacks.pfnDoneWithInstance(eseback_CONTEXT, ulInstanceId, (BackupDone)isComplete);
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackDoneWithInstanceForBackup");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		internal JET_err CallbackGetDatabasesInfo(IntPtr backupContext, out uint backupInfoArrayLength, out IntPtr backupInfoArray, uint boolReserved)
		{
			backupInfoArrayLength = 0U;
			backupInfoArray = IntPtr.Zero;
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				ESEBACK_CONTEXT eseback_CONTEXT = new ESEBACK_CONTEXT();
				eseback_CONTEXT.SetFromNativeEsebackContext(backupContext);
				backupInfoArrayLength = 0U;
				INSTANCE_BACKUP_INFO[] instanceBackupInfos;
				JET_err jet_err = this.managedEsebackCallbacks.pfnGetDatabasesInfo(eseback_CONTEXT, out instanceBackupInfos);
				if (jet_err >= JET_err.Success)
				{
					NATIVE_INSTANCE_BACKUP_INFO[] array = Eseback.ConvertInstanceBackupInfosToNative(instanceBackupInfos);
					backupInfoArray = Eseback.ConvertStructArrayToIntPtr<NATIVE_INSTANCE_BACKUP_INFO>(array);
					if (array != null)
					{
						backupInfoArrayLength = (uint)array.Length;
					}
				}
				else
				{
					backupInfoArray = IntPtr.Zero;
				}
				result = jet_err;
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackGetDatabasesInfo");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		internal unsafe JET_err CallbackFreeDatabasesInfo(IntPtr backupContext, uint backupInfoArrayLength, IntPtr backupInfoArray)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				ESEBACK_CONTEXT eseback_CONTEXT = new ESEBACK_CONTEXT();
				eseback_CONTEXT.SetFromNativeEsebackContext(backupContext);
				NATIVE_INSTANCE_BACKUP_INFO* ptr = (NATIVE_INSTANCE_BACKUP_INFO*)((void*)backupInfoArray);
				int num = 0;
				while ((long)num < (long)((ulong)backupInfoArrayLength))
				{
					ptr[num].FreeNativeInstanceInfo();
					num++;
				}
				Marshal.FreeHGlobal(backupInfoArray);
				result = JET_err.Success;
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackFreeDatabasesInfo");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		internal JET_err CallbackIsSGReplicated(IntPtr context, IntPtr instanceId, out int isStorageGroupReplicated, uint lengthSGGuid, IntPtr stringGGuid, out uint backupInfoArrayLength, out IntPtr backupInfoArray)
		{
			isStorageGroupReplicated = 0;
			backupInfoArrayLength = 0U;
			backupInfoArray = IntPtr.Zero;
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				ESEBACK_CONTEXT eseback_CONTEXT = new ESEBACK_CONTEXT();
				eseback_CONTEXT.SetFromNativeEsebackContext(context);
				JET_INSTANCE jetinst = new JET_INSTANCE
				{
					Value = instanceId
				};
				LOGSHIP_INFO[] array = null;
				bool flag;
				Guid guid;
				JET_err jet_err = this.managedEsebackCallbacks.pfnIsSGReplicated(eseback_CONTEXT, jetinst, out flag, out guid, out array);
				if (jet_err < JET_err.Success)
				{
					result = jet_err;
				}
				else
				{
					isStorageGroupReplicated = (flag ? 1 : 0);
					if (flag)
					{
						backupInfoArrayLength = (uint)array.Length;
						NATIVE_LOGSHIP_INFO[] nativeStructs = Eseback.ConvertLogshipInfosToNative(array);
						backupInfoArray = Eseback.ConvertStructArrayToIntPtr<NATIVE_LOGSHIP_INFO>(nativeStructs);
					}
					if (lengthSGGuid > 0U && stringGGuid != IntPtr.Zero)
					{
						string text = guid.ToString();
						char[] array2 = text.ToCharArray();
						int val = (int)(lengthSGGuid / 2U);
						int length = Math.Min(array2.Length, val);
						Marshal.Copy(array2, 0, stringGGuid, length);
					}
					result = jet_err;
				}
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackIsSGReplicated");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		internal unsafe JET_err CallbackFreeShipLogInfo(uint infoCount, IntPtr prgInfo)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				NATIVE_LOGSHIP_INFO* ptr = (NATIVE_LOGSHIP_INFO*)((void*)prgInfo);
				int num = 0;
				while ((long)num < (long)((ulong)infoCount))
				{
					Marshal.FreeHGlobal(ptr[num].wszName);
					ptr[num].wszName = IntPtr.Zero;
					num++;
				}
				Marshal.FreeHGlobal(prgInfo);
				result = JET_err.Success;
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackFreeShipLogInfo");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		internal JET_err CallbackServerAccessCheck()
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				result = (JET_err)this.managedEsebackCallbacks.pfnServerAccessCheck();
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackServerAccessCheck");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		internal JET_err CallbackTrace(string formatString)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				result = this.managedEsebackCallbacks.pfnTrace(formatString);
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during CallbackTrace");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		private static NATIVE_LOGSHIP_INFO[] ConvertLogshipInfosToNative(LOGSHIP_INFO[] logshipInfos)
		{
			NATIVE_LOGSHIP_INFO[] array = new NATIVE_LOGSHIP_INFO[logshipInfos.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = logshipInfos[i].GetNativeLogshipInfo();
			}
			return array;
		}

		private static NATIVE_INSTANCE_BACKUP_INFO[] ConvertInstanceBackupInfosToNative(INSTANCE_BACKUP_INFO[] instanceBackupInfos)
		{
			NATIVE_INSTANCE_BACKUP_INFO[] array = new NATIVE_INSTANCE_BACKUP_INFO[instanceBackupInfos.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = instanceBackupInfos[i].GetNativeInstanceBackupInfo();
			}
			return array;
		}

		private static readonly TraceSwitch TraceSwitch = new TraceSwitch("ESENT StatusCallbackWrapper", "Wrapper around unmanaged ESENT status callback");

		private readonly ESEBACK_CALLBACKS managedEsebackCallbacks;

		private NATIVE_ESEBACK_CALLBACKS nativeCallbacks;

		private NATIVE_ESEBACK_CALLBACKS_IMPL callbacksImpl;
	}
}
