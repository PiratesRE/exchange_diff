using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.ReplicaSeeder
{
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class CReplicaSeederInterop
	{
		public unsafe static int OpenBackupContext(string serverName, string dbName, string transferAddress, Guid databaseGuid, out IntPtr backupContext)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Entering {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::OpenBackupContext");
			uint num = 0;
			_ESE_REGISTERED_INFO* ptr = null;
			int num2 = 0;
			void* ptr2 = null;
			_INSTANCE_BACKUP_INFO* ptr3 = null;
			uint num3 = 0;
			ushort* ptr4 = null;
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentNullException("serverName");
			}
			if (string.IsNullOrEmpty(dbName))
			{
				throw new ArgumentNullException("dbName");
			}
			ushort* ptr5 = (ushort*)Marshal.StringToHGlobalUni(serverName).ToPointer();
			ushort* ptr6 = (ushort*)Marshal.StringToHGlobalUni(dbName).ToPointer();
			ushort* ptr7 = (ushort*)Marshal.StringToHGlobalUni(transferAddress).ToPointer();
			int num4 = <Module>.HrESEBackupRestoreGetRegisteredEx(ptr5, null, 0, 30000U, &num, &ptr);
			if (0 != num4)
			{
				if (-939585531 != num4 && -939585532 != num4)
				{
					if (-939587625 == num4)
					{
						num2 = 10;
						goto IL_30C;
					}
					if (-939647163 == num4)
					{
						num2 = 11;
						goto IL_30C;
					}
					if (21 == num4)
					{
						num2 = 17;
						goto IL_30C;
					}
					if (-939587619 == num4)
					{
						num2 = 18;
						goto IL_30C;
					}
					num2 = num4;
				}
				else
				{
					num2 = Marshal.GetLastWin32Error();
				}
			}
			else
			{
				_ESE_REGISTERED_INFO* ptr8;
				if (num != 1)
				{
					ExTraceGlobals.CReplicaSeederTracer.TraceDebug<Guid>(0L, "HrESEBackupRestoreGetRegistered() returned multiple registered endpoints. Attempting to resolve for SG {0}", databaseGuid);
					ptr4 = (ushort*)Marshal.StringToHGlobalUni(databaseGuid.ToString()).ToPointer();
					uint num5 = 0;
					if (0 < num)
					{
						do
						{
							ulong num6 = (ulong)(*(long*)(ptr + num5 * 40UL / (ulong)sizeof(_ESE_REGISTERED_INFO) + 8L / (long)sizeof(_ESE_REGISTERED_INFO)));
							if (0UL != num6 && 0 == <Module>._wcsicmp((ushort*)ptr4, num6))
							{
								goto IL_165;
							}
							num5++;
						}
						while (num5 < num);
						goto IL_189;
						IL_165:
						ExTraceGlobals.CReplicaSeederTracer.TraceDebug<Guid>(0L, "Endpoint for SG {0} is resolved", databaseGuid);
						ptr8 = num5 * 40L + ptr / sizeof(_ESE_REGISTERED_INFO);
						if (null != ptr8)
						{
							goto IL_1B9;
						}
					}
					IL_189:
					ExTraceGlobals.CReplicaSeederTracer.TraceDebug<Guid, uint>(0L, "Unable to resolve an endpoint for SG {0}. Endpoints returned {1}", databaseGuid, num);
					num2 = 1;
					goto IL_30C;
				}
				ExTraceGlobals.CReplicaSeederTracer.TraceDebug(0L, "HrESEBackupRestoreGetRegistered() returned single endpoint. Assuming store is running in single process");
				ptr8 = ptr;
				IL_1B9:
				num4 = <Module>.HrESESeedPrepare(ptr5, *(long*)(ptr8 + 8L / (long)sizeof(_ESE_REGISTERED_INFO)), 0, 30000U, &num3, &ptr3, &ptr2);
				if (0 == num4)
				{
					uint num7 = 0;
					if (0 < num3)
					{
						while (0 != <Module>._wcsicmp(*(long*)(ptr3 + num7 * 48UL / (ulong)sizeof(_INSTANCE_BACKUP_INFO) + 8L / (long)sizeof(_INSTANCE_BACKUP_INFO)), (ushort*)ptr6))
						{
							num7++;
							if (num7 >= num3)
							{
								goto IL_303;
							}
						}
						long hInstanceId = (num7 * 48L)[ptr3 / 8];
						num4 = <Module>.EcDoHrESEBackupSetup(ptr2, hInstanceId, 1, ptr7);
						if (0 != num4)
						{
							if (-939585531 != num4 && -939585532 != num4)
							{
								if (-939587625 == num4)
								{
									num2 = 10;
								}
								else if (-939647163 == num4)
								{
									num2 = 11;
								}
								else if (21 == num4)
								{
									num2 = 17;
								}
								else
								{
									num2 = ((-939587619 == num4) ? 18 : num4);
								}
							}
							else
							{
								num2 = Marshal.GetLastWin32Error();
							}
							ExTraceGlobals.CReplicaSeederTracer.TraceDebug<int, int>(0L, "EcDoHrESEBackupSetup() failed with hr=0x{0:x}; ec={1}", num4, num2);
							goto IL_308;
						}
						<Module>.ESEBackupFreeInstanceInfo(num3, ptr3);
						num3 = 0;
						ptr3 = null;
						IntPtr intPtr = (IntPtr)ptr2;
						backupContext = intPtr;
						goto IL_335;
					}
					IL_303:
					num2 = 9;
					goto IL_30C;
				}
				if (-939585531 != num4 && -939585532 != num4)
				{
					if (-939587625 == num4)
					{
						num2 = 10;
						goto IL_30C;
					}
					if (-939647163 == num4)
					{
						num2 = 11;
						goto IL_30C;
					}
					if (21 == num4)
					{
						num2 = 17;
						goto IL_30C;
					}
					if (-939587619 == num4)
					{
						num2 = 18;
						goto IL_30C;
					}
					num2 = num4;
				}
				else
				{
					num2 = Marshal.GetLastWin32Error();
				}
			}
			IL_308:
			if (0 == num2)
			{
				goto IL_335;
			}
			IL_30C:
			if (null != ptr2)
			{
				<Module>.HrESEBackupInstanceEnd(ptr2, 0);
				<Module>.HrESEBackupEnd(ptr2);
				ptr2 = null;
			}
			if (num3 > 0)
			{
				<Module>.ESEBackupFreeInstanceInfo(num3, ptr3);
				ptr3 = null;
			}
			IL_335:
			if (num > 0)
			{
				<Module>.ESEBackupRestoreFreeRegisteredInfo(num, ptr);
			}
			if (ptr5 != null)
			{
				IntPtr hglobal = new IntPtr((void*)ptr5);
				Marshal.FreeHGlobal(hglobal);
			}
			if (ptr6 != null)
			{
				IntPtr hglobal2 = new IntPtr((void*)ptr6);
				Marshal.FreeHGlobal(hglobal2);
			}
			if (ptr7 != null)
			{
				IntPtr hglobal3 = new IntPtr((void*)ptr7);
				Marshal.FreeHGlobal(hglobal3);
			}
			if (ptr4 != null)
			{
				IntPtr hglobal4 = new IntPtr((void*)ptr4);
				Marshal.FreeHGlobal(hglobal4);
			}
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string, int>(0L, "Leaving {0}, returning {1}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::OpenBackupContext", num2);
			return num2;
		}

		public unsafe static int OpenBackupFileHandle(IntPtr backupContext, string sourceFileToBackupFullPath, uint cbReadHintSize, out IntPtr sourceBackupFileHandle, out long sourceFileSizeBytes)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Entering {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::OpenBackupFileHandle");
			int num = 0;
			long num2 = 0L;
			void* hccxBackupContext = (void*)backupContext;
			void* ptr = -1L;
			if (string.IsNullOrEmpty(sourceFileToBackupFullPath))
			{
				throw new ArgumentNullException("sourceFileToBackupFullPath");
			}
			if (backupContext == IntPtr.Zero)
			{
				throw new ArgumentNullException("backupContext");
			}
			ushort* ptr2 = (ushort*)Marshal.StringToHGlobalUni(sourceFileToBackupFullPath).ToPointer();
			int num3 = <Module>.HrESEBackupOpenFile(hccxBackupContext, ptr2, cbReadHintSize, 1, &ptr, &num2);
			if (0 != num3)
			{
				if (-939585531 != num3 && -939585532 != num3)
				{
					if (-939587625 == num3)
					{
						num = 10;
					}
					else if (-939647163 == num3)
					{
						num = 11;
					}
					else if (21 == num3)
					{
						num = 17;
					}
					else
					{
						num = ((-939587619 == num3) ? 18 : num3);
					}
				}
				else
				{
					num = Marshal.GetLastWin32Error();
				}
				ExTraceGlobals.CReplicaSeederTracer.TraceError<string, int>(0L, "Error: HrESEBackupOpenFile() failed for '{0}',  ec={1}.", sourceFileToBackupFullPath, num);
				if (num != null)
				{
					<Module>.HrESEBackupCloseFile(hccxBackupContext, ptr);
				}
			}
			else
			{
				IntPtr intPtr = (IntPtr)ptr;
				sourceBackupFileHandle = intPtr;
				sourceFileSizeBytes = num2;
			}
			if (ptr2 != null)
			{
				IntPtr hglobal = new IntPtr((void*)ptr2);
				Marshal.FreeHGlobal(hglobal);
			}
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Leaving {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::OpenBackupFileHandle");
			return num;
		}

		public unsafe static int PerformDatabaseRead(IntPtr backupContext, IntPtr sourceBackupFileHandle, long sourceFileOffset, byte[] clrBuffer, ref int sourceBytesRead)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Entering {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::PerformDatabaseRead");
			void* hccxBackupContext = (void*)backupContext;
			void* ptr = (void*)sourceBackupFileHandle;
			if (backupContext == IntPtr.Zero)
			{
				throw new ArgumentNullException("backupContext");
			}
			if (sourceBackupFileHandle == IntPtr.Zero)
			{
				throw new ArgumentNullException("sourceBackupFileHandle");
			}
			ref byte byte& = ref clrBuffer[0];
			byte* pbOutputBuffer = ref byte&;
			uint cbSourceFileToRead = clrBuffer.Length;
			uint num = 0;
			int num2 = <Module>.EcDoBackupReadFileEx(hccxBackupContext, ptr, sourceFileOffset, pbOutputBuffer, cbSourceFileToRead, &num);
			if (num2 != null)
			{
				if (-939585531 != num2 && -939585532 != num2)
				{
					if (-939587625 == num2)
					{
						num2 = 10;
					}
					else if (-939647163 == num2)
					{
						num2 = 11;
					}
					else if (21 == num2)
					{
						num2 = 17;
					}
					else if (-939587619 == num2)
					{
						num2 = 18;
					}
				}
				else
				{
					num2 = Marshal.GetLastWin32Error();
				}
				object[] args = new object[]
				{
					ptr,
					sourceFileOffset,
					num,
					num2
				};
				ExTraceGlobals.CReplicaSeederTracer.TraceError(0L, "Error: EcDoBackupReadFileEx() failed for '{0}' at {1} bytes, read {2} bytes. ec={3}", args);
			}
			else
			{
				ExTraceGlobals.CReplicaSeederTracer.TraceDebug<long, long, int>(0L, "EcDoBackupReadFileEx() succeeded for '{0}' at offset {1}, read {2} bytes.", ptr, sourceFileOffset, num);
			}
			sourceBytesRead = num;
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Leaving {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::PerformDatabaseRead");
			return num2;
		}

		public unsafe static int CloseBackupFileHandle(IntPtr backupContext, IntPtr sourceBackupFileHandle)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Entering {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::CloseBackupFileHandle");
			int result = 0;
			void* hccxBackupContext = (void*)backupContext;
			void* hFile = (void*)sourceBackupFileHandle;
			int num = <Module>.HrESEBackupCloseFile(hccxBackupContext, hFile);
			if (0 != num)
			{
				if (-939585531 != num && -939585532 != num)
				{
					if (-939587625 == num)
					{
						result = 10;
					}
					else if (-939647163 == num)
					{
						result = 11;
					}
					else if (21 == num)
					{
						result = 17;
					}
					else
					{
						result = ((-939587619 == num) ? 18 : num);
					}
				}
				else
				{
					result = Marshal.GetLastWin32Error();
				}
				ExTraceGlobals.CReplicaSeederTracer.TraceError(0L, "Error: HrESEBackupCloseFile() failed.");
			}
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Leaving {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::CloseBackupFileHandle");
			return result;
		}

		public unsafe static int OnlineGetDatabasePages(IntPtr backupContext, string databaseName, ulong pgnoStart, ulong cpg, ulong cbPage, out byte[] pageOut, out long pageRead, out long lowGen, out long highGen)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Entering {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::OnlineGetDatabasePages");
			int result = 0;
			uint pgnoStart2 = (uint)pgnoStart;
			uint cpg2 = (uint)cpg;
			uint num = 0;
			uint num2 = 0;
			uint num3 = 0;
			void* hccxBackupContext = (void*)backupContext;
			if (string.IsNullOrEmpty(databaseName))
			{
				throw new ArgumentNullException("databasePath");
			}
			if (backupContext == IntPtr.Zero)
			{
				throw new ArgumentNullException("backupContext");
			}
			if (pgnoStart < 1UL)
			{
				throw new ArgumentOutOfRangeException("pgnoStart", pgnoStart, "expect page number to be greater than 0");
			}
			if (cpg < 1UL)
			{
				throw new ArgumentOutOfRangeException("cpg", cpg, "expect request at least one page");
			}
			if (cbPage < 1UL)
			{
				throw new ArgumentOutOfRangeException("cbPage", cbPage, "page size should be > 0 bytes");
			}
			if (cpg > 2147483647UL)
			{
				throw new ArgumentOutOfRangeException("cpg", cpg, "number of pages requested is too high");
			}
			uint num4 = (uint)(cpg * cbPage);
			ushort* ptr = (ushort*)Marshal.StringToHGlobalUni(databaseName).ToPointer();
			byte* ptr2 = <Module>.VirtualAlloc(null, num4, 12288, 4);
			if (ptr2 == null)
			{
				if (ptr != null)
				{
					IntPtr hglobal = new IntPtr((void*)ptr);
					Marshal.FreeHGlobal(hglobal);
				}
				throw new EsentOutOfMemoryException();
			}
			int num5 = <Module>.HrESESeedReadPages(hccxBackupContext, ptr, pgnoStart2, cpg2, (void*)ptr2, num4, &num, 0, &num2, &num3);
			if (0 != num5)
			{
				if (-939585531 != num5 && -939585532 != num5)
				{
					if (-939587625 == num5)
					{
						result = 10;
					}
					else if (-939647163 == num5)
					{
						result = 11;
					}
					else if (21 == num5)
					{
						result = 17;
					}
					else
					{
						result = ((-939587619 == num5) ? 18 : num5);
					}
				}
				else
				{
					result = Marshal.GetLastWin32Error();
				}
			}
			else
			{
				int length = num;
				byte[] array = new byte[num];
				if (num > 0)
				{
					Marshal.Copy((IntPtr)((void*)ptr2), array, 0, length);
				}
				pageOut = array;
				pageRead = (long)num;
				lowGen = (long)num2;
				highGen = (long)num3;
			}
			<Module>.VirtualFree((void*)ptr2, 0UL, 32768);
			if (ptr != null)
			{
				IntPtr hglobal2 = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal2);
			}
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Leaving {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::OnlineGetDatabasePages");
			return result;
		}

		public unsafe static int ForceNewLog(IntPtr backupContext)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Enter {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::ForceNewLog");
			if (backupContext == IntPtr.Zero)
			{
				throw new ArgumentNullException("backupContext");
			}
			void* hccxBackupContext = (void*)backupContext;
			int num = 0;
			int num2 = <Module>.HrESESeedForceNewLog(hccxBackupContext);
			if (0 != num2)
			{
				if (-939585531 != num2 && -939585532 != num2)
				{
					if (-939587625 == num2)
					{
						num = 10;
					}
					else if (-939647163 == num2)
					{
						num = 11;
					}
					else if (21 == num2)
					{
						num = 17;
					}
					else
					{
						num = ((-939587619 == num2) ? 18 : num2);
					}
				}
				else
				{
					num = Marshal.GetLastWin32Error();
				}
				ExTraceGlobals.CReplicaSeederTracer.TraceError<int>(0L, "failed to HrESESeedForceNewLog {0:X}", num);
			}
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Leaving {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::ForceNewLog");
			return num;
		}

		public unsafe static int CloseBackupContext(IntPtr backupContext, int ecLast)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Enter {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::CloseBackupContext");
			void* hccxBackupContext = (void*)backupContext;
			int num = 0;
			int fFlags = (ecLast != 0) ? 0 : 1;
			int num2 = <Module>.HrESEBackupInstanceEnd(hccxBackupContext, fFlags);
			if (0 != num2)
			{
				if (-939585531 != num2 && -939585532 != num2)
				{
					if (-939587625 == num2)
					{
						num = 10;
					}
					else if (-939647163 == num2)
					{
						num = 11;
					}
					else if (21 == num2)
					{
						num = 17;
					}
					else
					{
						num = ((-939587619 == num2) ? 18 : num2);
					}
				}
				else
				{
					num = Marshal.GetLastWin32Error();
				}
				ExTraceGlobals.CReplicaSeederTracer.TraceError<ulong>(0L, "failed to HrESEBackupInstanceEnd {0:X16}", (ulong)num);
			}
			int num3 = <Module>.HrESEBackupEnd(hccxBackupContext);
			if (0 != num3)
			{
				if (-939585531 != num3 && -939585532 != num3)
				{
					if (-939587625 == num3)
					{
						num = 10;
					}
					else if (-939647163 == num3)
					{
						num = 11;
					}
					else if (21 == num3)
					{
						num = 17;
					}
					else
					{
						num = ((-939587619 == num3) ? 18 : num3);
					}
				}
				else
				{
					num = Marshal.GetLastWin32Error();
				}
				ExTraceGlobals.CReplicaSeederTracer.TraceError<ulong>(0L, "failed to HrESEBackupEnd {0:X16}", (ulong)num);
			}
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Leaving {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::CloseBackupContext");
			return num;
		}

		public unsafe static int GetDatabaseInfo(IntPtr backupContext, string databaseName, out JET_DBINFOMISC databaseInfo)
		{
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Enter {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::GetDatabaseInfo");
			void* hccxBackupContext = (void*)backupContext;
			uint num = 0;
			int result = 0;
			if (string.IsNullOrEmpty(databaseName))
			{
				throw new ArgumentNullException("databaseName");
			}
			if (backupContext == IntPtr.Zero)
			{
				throw new ArgumentNullException("backupContext");
			}
			ushort* ptr = (ushort*)Marshal.StringToHGlobalUni(databaseName).ToPointer();
			JET_DBINFOMISC6 jet_DBINFOMISC;
			int num2 = <Module>.HrESESeedGetDatabaseInfo(hccxBackupContext, ptr, (void*)(&jet_DBINFOMISC), 400, &num, 0);
			if (0 != num2)
			{
				if (-939585531 != num2 && -939585532 != num2)
				{
					if (-939587625 == num2)
					{
						result = 10;
					}
					else if (-939647163 == num2)
					{
						result = 11;
					}
					else if (21 == num2)
					{
						result = 17;
					}
					else
					{
						result = ((-939587619 == num2) ? 18 : num2);
					}
				}
				else
				{
					result = Marshal.GetLastWin32Error();
				}
			}
			else if (num != 400)
			{
				result = 1;
			}
			else
			{
				databaseInfo = new JET_DBINFOMISC();
				NATIVE_DBINFOMISC6 native_DBINFOMISC = (NATIVE_DBINFOMISC6)Marshal.PtrToStructure((IntPtr)((void*)(&jet_DBINFOMISC)), typeof(NATIVE_DBINFOMISC6));
				databaseInfo.SetFromNativeDbinfoMisc6(ref native_DBINFOMISC);
			}
			if (ptr != null)
			{
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
			}
			ExTraceGlobals.CReplicaSeederTracer.TraceDebug<string>(0L, "Leaving {0}", "Microsoft::Exchange::Cluster::ReplicaSeeder::CReplicaSeederInterop::GetDatabaseInfo");
			return result;
		}

		public static void SetupNativeLogger()
		{
			<Module>.SetupLogger();
		}

		public static void CleanupNativeLogger()
		{
			<Module>.CleanupLogger();
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe static bool IsSpaceSufficient(ulong edbSize, ulong existingEdbSize, string databasePath)
		{
			ushort* ptr = (ushort*)Marshal.StringToHGlobalUni(databasePath).ToPointer();
			bool result = <Module>.IsSpaceEnough((long)edbSize, (long)existingEdbSize, ptr) != null;
			if (ptr != null)
			{
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
			}
			return result;
		}

		private CReplicaSeederInterop()
		{
		}
	}
}
